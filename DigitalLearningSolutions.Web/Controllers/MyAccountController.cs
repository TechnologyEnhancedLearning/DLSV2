namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Route("/{dlsSubApplication}/MyAccount", Order = 1)]
    [Route("/MyAccount", Order = 2)]
    [TypeFilter(typeof(ValidateAllowedDlsSubApplication))]
    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.MyAccount))]
    [Authorize(Policy = CustomPolicies.BasicUser)]
    public class MyAccountController : Controller
    {
        private const string SwitchCentreReturnUrl = "/Home/Welcome";
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
        private readonly IConfiguration config;
        private readonly IImageResizeService imageResizeService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly ILogger<MyAccountController> logger;
        private readonly PromptsService promptsService;
        private readonly IUserDataService userDataService;
        private readonly IUserService userService;
        private readonly IEmailVerificationService emailVerificationService;

        public MyAccountController(
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            IUserService userService,
            IUserDataService userDataService,
            IImageResizeService imageResizeService,
            IJobGroupsDataService jobGroupsDataService,
            IEmailVerificationService emailVerificationService,
            PromptsService registrationPromptsService,
            ILogger<MyAccountController> logger,
            IConfiguration config
        )
        {
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.userService = userService;
            this.userDataService = userDataService;
            this.imageResizeService = imageResizeService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.emailVerificationService = emailVerificationService;
            promptsService = registrationPromptsService;
            this.logger = logger;
            this.config = config;
        }

        [NoCaching]
        public IActionResult Index(DlsSubApplication dlsSubApplication)
        {
            var centreId = User.GetCentreId();
            var userId = User.GetUserIdKnownNotNull();
            var userEntity = userService.GetUserById(userId);

            var adminAccount = userEntity!.GetCentreAccountSet(centreId)?.AdminAccount;
            var delegateAccount = GetDelegateAccountIfActive(userEntity, centreId);

            var customPrompts =
                centreRegistrationPromptsService.GetCentreRegistrationPromptsWithAnswersByCentreIdAndDelegateAccount(
                    centreId,
                    delegateAccount
                );

            var allCentreSpecificEmails = centreId == null
                ? userService.GetAllCentreEmailsForUser(userId).ToList()
                : new List<(int centreId, string centreName, string? centreSpecificEmail)>();

            var (_, unverifiedCentreEmails) = userService.GetUnverifiedEmailsForUser(userEntity.UserAccount.Id);

            var switchCentreReturnUrl = StringHelper.GetLocalRedirectUrl(config, SwitchCentreReturnUrl);

            var model = new MyAccountViewModel(
                userEntity.UserAccount,
                delegateAccount,
                centreId,
                adminAccount?.CentreName ?? delegateAccount?.CentreName,
                centreId != null ? userService.GetCentreEmail(userId, centreId.Value) : null,
                customPrompts,
                allCentreSpecificEmails,
                unverifiedCentreEmails,
                dlsSubApplication,
                switchCentreReturnUrl
            );

            return View(model);
        }

        [NoCaching]
        [HttpGet("EditDetails")]
        public IActionResult EditDetails(
            DlsSubApplication dlsSubApplication,
            string? returnUrl = null,
            bool isCheckDetailsRedirect = false
        )
        {
            var centreId = User.GetCentreId();
            var userId = User.GetUserIdKnownNotNull();
            var userEntity = userService.GetUserById(userId);
            var delegateAccount = GetDelegateAccountIfActive(userEntity, centreId);

            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();

            var customPrompts =
                centreId != null
                    ? promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(
                        delegateAccount,
                        centreId.Value
                    )
                    : new List<EditDelegateRegistrationPromptViewModel>();

            var allCentreSpecificEmails = centreId == null
                ? userService.GetAllCentreEmailsForUser(userId).ToList()
                : new List<(int centreId, string centreName, string? centreSpecificEmail)>();

            var model = new MyAccountEditDetailsViewModel(
                userEntity!.UserAccount,
                delegateAccount,
                centreId,
                jobGroups,
                centreId != null ? userService.GetCentreEmail(userId, centreId.Value) : null,
                customPrompts,
                allCentreSpecificEmails,
                dlsSubApplication,
                returnUrl,
                isCheckDetailsRedirect
            );

            return View(model);
        }

        [NoCaching]
        [HttpPost("EditDetails")]
        public async Task<IActionResult> EditDetails(
            MyAccountEditDetailsFormData formData,
            string action,
            DlsSubApplication dlsSubApplication
        )
        {
            return action switch
            {
                "save" => await EditDetailsPostSave(formData, dlsSubApplication),
                "previewImage" => EditDetailsPostPreviewImage(formData, dlsSubApplication),
                "removeImage" => EditDetailsPostRemoveImage(formData, dlsSubApplication),
                _ => new StatusCodeResult(500),
            };
        }

        private async Task<IActionResult> EditDetailsPostSave(
            MyAccountEditDetailsFormData formData,
            DlsSubApplication dlsSubApplication
        )
        {
            var centreId = User.GetCentreId();
            var userId = User.GetUserIdKnownNotNull();
            var userEntity = userService.GetUserById(userId);

            var delegateAccount = GetDelegateAccountIfActive(userEntity, centreId);

            ValidateEditDetailsData(formData, delegateAccount, centreId);

            if (!ModelState.IsValid)
            {
                return ReturnToEditDetailsViewWithErrors(formData, userId, centreId, dlsSubApplication);
            }

            ValidateEmailUniqueness(formData, userId, centreId);

            if (!ModelState.IsValid)
            {
                return ReturnToEditDetailsViewWithErrors(formData, userId, centreId, dlsSubApplication);
            }

            var (accountDetailsData, delegateDetailsData) = AccountDetailsDataHelper.MapToEditAccountDetailsData(
                formData,
                userId,
                delegateAccount?.Id
            );

            var unverifiedModifiedEmails = GetUnverifiedModifiedEmails(
                userId,
                centreId,
                accountDetailsData.Email,
                formData
            );

            SaveUserDetails(
                userEntity!.UserAccount,
                accountDetailsData,
                delegateDetailsData,
                formData,
                centreId,
                unverifiedModifiedEmails
            );

            var shouldRedirectToVerifyYourEmail = unverifiedModifiedEmails.Any();

            return await GetRedirectLocation(
                userEntity.UserAccount,
                shouldRedirectToVerifyYourEmail,
                shouldRedirectToVerifyYourEmail && centreId.HasValue,
                formData.ReturnUrl,
                dlsSubApplication
            );
        }

        private void ValidateEditDetailsData(
            MyAccountEditDetailsFormData formData,
            DelegateAccount? delegateAccount,
            int? centreId
        )
        {
            // Custom Validate functions are not called if the ModelState is invalid due to attribute validation.
            // This form potentially (if the user is not logged in to a centre) contains the ability to edit all the user's centre-specific emails,
            // which are validated by a Validate function, so in order to display error messages for them if some other field is ALSO invalid,
            // we must manually call formData.Validate() here.
            if (!ModelState.IsValid)
            {
                var validationResults = formData.Validate(new ValidationContext(formData));

                foreach (var error in validationResults)
                {
                    foreach (var memberName in error.MemberNames)
                    {
                        ModelState.AddModelError(memberName, error.ErrorMessage);
                    }
                }
            }

            if (delegateAccount != null)
            {
                promptsService.ValidateCentreRegistrationPrompts(formData, centreId!.Value, ModelState);
            }

            if (formData.ProfileImageFile != null)
            {
                ModelState.AddModelError(
                    nameof(MyAccountEditDetailsFormData.ProfileImageFile),
                    "Preview your new profile picture before saving"
                );
            }

            ProfessionalRegistrationNumberHelper.ValidateProfessionalRegistrationNumber(
                ModelState,
                formData.HasProfessionalRegistrationNumber,
                formData.ProfessionalRegistrationNumber
            );
        }

        private void ValidateEmailUniqueness(
            MyAccountEditDetailsFormData formData,
            int userId,
            int? centreId
        )
        {
            if (userDataService.PrimaryEmailIsInUseByOtherUser(formData.Email!, userId))
            {
                ModelState.AddModelError(
                    nameof(MyAccountEditDetailsFormData.Email),
                    CommonValidationErrorMessages.EmailInUse
                );
            }

            if (centreId.HasValue)
            {
                ValidateSingleCentreEmail(formData.CentreSpecificEmail, centreId.Value, userId);
            }
            else
            {
                ValidateCentreEmailsDictionary(formData.CentreSpecificEmailsByCentreId, userId);
            }
        }

        private void ValidateSingleCentreEmail(string? email, int centreId, int userId)
        {
            if (IsCentreSpecificEmailAlreadyInUse(email, centreId, userId))
            {
                ModelState.AddModelError(
                    nameof(MyAccountEditDetailsFormData.CentreSpecificEmail),
                    CommonValidationErrorMessages.EmailInUseAtCentre
                );
            }
        }

        private void ValidateCentreEmailsDictionary(Dictionary<int, string?> centreEmailsDictionary, int userId)
        {
            foreach (var (centreId, centreEmail) in centreEmailsDictionary)
            {
                if (IsCentreSpecificEmailAlreadyInUse(centreEmail, centreId, userId))
                {
                    ModelState.AddModelError(
                        $"{nameof(MyAccountEditDetailsFormData.AllCentreSpecificEmailsDictionary)}_{centreId}",
                        CommonValidationErrorMessages.EmailInUseAtCentre
                    );
                }
            }
        }

        private List<(string, int?)> GetUnverifiedModifiedEmails(
            int userId,
            int? centreId,
            string primaryEmail,
            MyAccountEditDetailsFormData formData
        )
        {
            var unverifiedModifiedEmails = new List<(string, int?)>();

            if (userDataService.IsPrimaryEmailBeingChangedForUser(userId, primaryEmail) &&
                emailVerificationService.AccountEmailRequiresVerification(userId, primaryEmail))
            {
                unverifiedModifiedEmails.Add((primaryEmail, null));
            }

            if (centreId.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(formData.CentreSpecificEmail) &&
                    userDataService.IsCentreEmailBeingChangedForUserAtCentre(
                        userId,
                        centreId.Value,
                        formData.CentreSpecificEmail
                    ) &&
                    emailVerificationService.AccountEmailRequiresVerification(
                        userId,
                        formData.CentreSpecificEmail
                    ))
                {
                    unverifiedModifiedEmails.Add((formData.CentreSpecificEmail, centreId.Value));
                }
            }
            else
            {
                foreach (var (centre, email) in formData.CentreSpecificEmailsByCentreId)
                {
                    if (!string.IsNullOrWhiteSpace(email) &&
                        userDataService.IsCentreEmailBeingChangedForUserAtCentre(userId, centre, email) &&
                        emailVerificationService.AccountEmailRequiresVerification(userId, email))
                    {
                        unverifiedModifiedEmails.Add((email, centre));
                    }
                }
            }

            return unverifiedModifiedEmails;
        }

        private void SaveUserDetails(
            UserAccount userAccount,
            EditAccountDetailsData accountDetailsData,
            DelegateDetailsData? delegateDetailsData,
            MyAccountEditDetailsFormData formData,
            int? centreId,
            IEnumerable<(string, int?)> unverifiedModifiedEmails
        )
        {
            if (centreId.HasValue)
            {
                userService.UpdateUserDetailsAndCentreSpecificDetails(
                    accountDetailsData,
                    delegateDetailsData,
                    formData.CentreSpecificEmail,
                    centreId.Value,
                    true
                );
                emailVerificationService.SendVerificationEmails(userAccount, unverifiedModifiedEmails);
            }
            else
            {
                userService.UpdateUserDetails(accountDetailsData, true);
                userService.SetCentreEmails(userAccount.Id, formData.CentreSpecificEmailsByCentreId);
                emailVerificationService.SendVerificationEmails(userAccount, unverifiedModifiedEmails);
            }
        }

        private async Task<IActionResult> GetRedirectLocation(
            UserAccount userAccount,
            bool shouldRedirectToVerifyYourEmail,
            bool shouldLogIntoCentrelessAccount,
            string? returnUrl,
            DlsSubApplication dlsSubApplication
        )
        {
            if (shouldRedirectToVerifyYourEmail)
            {
                if (shouldLogIntoCentrelessAccount)
                {
                    var claim = ((ClaimsIdentity)User.Identity).FindFirst("IsPersistent");
                    var isPersistent = claim != null && Convert.ToBoolean(claim.Value);
                    await HttpContext.Logout();
                    await this.CentrelessLogInAsync(userAccount, isPersistent);
                }
                var emailVerificationReason = EmailVerificationReason.EmailChanged;
                return RedirectToAction("Index", "VerifyYourEmail", new { emailVerificationReason });
            }

            return this.RedirectToReturnUrl(returnUrl, logger) ?? RedirectToAction(
                "Index",
                new { dlsSubApplication = dlsSubApplication.UrlSegment }
            );
        }

        private bool IsCentreSpecificEmailAlreadyInUse(string? email, int centreId, int userId)
        {
            return !string.IsNullOrWhiteSpace(email) &&
                   userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(email, centreId, userId);
        }

        private IActionResult ReturnToEditDetailsViewWithErrors(
            MyAccountEditDetailsFormData formData,
            int userId,
            int? centreId,
            DlsSubApplication dlsSubApplication
        )
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            var customPrompts = centreId != null
                ? promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(formData, centreId.Value)
                : new List<EditDelegateRegistrationPromptViewModel>();

            var allCentreSpecificEmails = centreId == null
                ? userService.GetAllCentreEmailsForUser(userId).Select(
                    row =>
                    {
                        string? email = null;

                        formData.AllCentreSpecificEmailsDictionary?.TryGetValue(
                            row.centreId.ToString(),
                            out email
                        );

                        return (row.centreId, row.centreName, email);
                    }
                ).ToList()
                : new List<(int centreId, string centreName, string? centreSpecificEmail)>();

            var model = new MyAccountEditDetailsViewModel(
                formData,
                centreId,
                jobGroups,
                customPrompts,
                allCentreSpecificEmails,
                dlsSubApplication
            );

            return View(model);
        }

        private IActionResult EditDetailsPostPreviewImage(
            MyAccountEditDetailsFormData formData,
            DlsSubApplication dlsSubApplication
        )
        {
            // We don't want to display validation errors on other fields in this case
            ModelState.ClearErrorsForAllFieldsExcept(nameof(MyAccountEditDetailsViewModel.ProfileImageFile));

            var userId = User.GetUserIdKnownNotNull();
            var centreId = User.GetCentreId();
            var userEntity = userService.GetUserById(userId);
            var delegateAccount = GetDelegateAccountIfActive(userEntity, centreId);

            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();

            var customPrompts = centreId != null
                ? promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(delegateAccount, centreId.Value)
                : new List<EditDelegateRegistrationPromptViewModel>();

            var allCentreSpecificEmails = centreId == null
                ? userService.GetAllCentreEmailsForUser(userId).ToList()
                : new List<(int centreId, string centreName, string? centreSpecificEmail)>();

            if (ModelState.IsValid && formData.ProfileImageFile != null)
            {
                ModelState.Remove(nameof(MyAccountEditDetailsFormData.ProfileImage));
                formData.ProfileImage = imageResizeService.ResizeProfilePicture(formData.ProfileImageFile);
            }

            var model = new MyAccountEditDetailsViewModel(
                formData,
                centreId,
                jobGroups,
                customPrompts,
                allCentreSpecificEmails,
                dlsSubApplication
            );
            return View(model);
        }

        private IActionResult EditDetailsPostRemoveImage(
            MyAccountEditDetailsFormData formData,
            DlsSubApplication dlsSubApplication
        )
        {
            // We don't want to display validation errors on other fields in this case
            ModelState.ClearAllErrors();

            ModelState.Remove(nameof(MyAccountEditDetailsFormData.ProfileImage));
            formData.ProfileImage = null;

            var userId = User.GetUserIdKnownNotNull();
            var centreId = User.GetCentreId();
            var userEntity = userService.GetUserById(userId);
            var delegateAccount = GetDelegateAccountIfActive(userEntity, centreId);

            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();

            var customPrompts = centreId != null
                ? promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(delegateAccount, centreId.Value)
                : new List<EditDelegateRegistrationPromptViewModel>();

            var allCentreSpecificEmails = centreId == null
                ? userService.GetAllCentreEmailsForUser(userId).ToList()
                : new List<(int centreId, string centreName, string? centreSpecificEmail)>();

            var model = new MyAccountEditDetailsViewModel(
                formData,
                centreId,
                jobGroups,
                customPrompts,
                allCentreSpecificEmails,
                dlsSubApplication
            );
            return View(model);
        }

        private static DelegateAccount? GetDelegateAccountIfActive(UserEntity? user, int? centreId)
        {
            var delegateAccount = user?.GetCentreAccountSet(centreId)?.DelegateAccount;

            return delegateAccount is { Active: true } ? delegateAccount : null;
        }
    }
}
