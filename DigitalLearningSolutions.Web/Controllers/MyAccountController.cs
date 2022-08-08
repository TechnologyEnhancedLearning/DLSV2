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

            var centreId = User.GetCentreId();
            var userId = User.GetUserIdKnownNotNull();
            var userEntity = userService.GetUserById(userId);

            var delegateAccount = GetDelegateAccountIfActive(userEntity, centreId);

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

            if (!ModelState.IsValid)
            {
                return ReturnToEditDetailsViewWithErrors(formData, userId, centreId, dlsSubApplication);
            }

            if (userDataService.PrimaryEmailIsInUseByOtherUser(formData.Email!, userId))
            {
                ModelState.AddModelError(
                    nameof(MyAccountEditDetailsFormData.Email),
                    CommonValidationErrorMessages.EmailAlreadyInUse
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

            if (!ModelState.IsValid)
            {
                return ReturnToEditDetailsViewWithErrors(formData, userId, centreId, dlsSubApplication);
            }

            var (accountDetailsData, delegateDetailsData) = AccountDetailsDataHelper.MapToEditAccountDetailsData(
                formData,
                userId,
                delegateAccount?.Id
            );

            var verifiedModifiedEmails = new List<(string, int?)>();
            var unverifiedModifiedEmails = new List<(string, int?)>();

            if (userDataService.IsPrimaryEmailBeingChangedForUser(userId, accountDetailsData.Email))
            {
                if (emailVerificationService.AccountEmailRequiresVerification(userId, accountDetailsData.Email))
                {
                    unverifiedModifiedEmails.Add((accountDetailsData.Email, null));
                }
                else
                {
                    verifiedModifiedEmails.Add((accountDetailsData.Email, null));
                }
            }

            if (centreId.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(formData.CentreSpecificEmail) &&
                    userDataService.IsCentreEmailBeingChangedForUserAtCentre(
                        userId,
                        centreId.Value,
                        formData.CentreSpecificEmail
                    ))
                {
                    if (emailVerificationService.AccountEmailRequiresVerification(
                            userId,
                            formData.CentreSpecificEmail
                        ))
                    {
                        unverifiedModifiedEmails.Add((formData.CentreSpecificEmail, centreId.Value));
                    }
                    else
                    {
                        verifiedModifiedEmails.Add((formData.CentreSpecificEmail, centreId.Value));
                    }
                }

                userService.UpdateUserDetailsAndCentreSpecificDetails(
                    accountDetailsData,
                    delegateDetailsData,
                    formData.CentreSpecificEmail,
                    centreId.Value,
                    true
                );
                emailVerificationService.SendVerificationEmails(
                    userEntity!.UserAccount,
                    verifiedModifiedEmails,
                    unverifiedModifiedEmails
                );
            }
            else
            {
                foreach (var (centre, email) in formData.CentreSpecificEmailsByCentreId)
                {
                    if (!string.IsNullOrWhiteSpace(email) &&
                        userDataService.IsCentreEmailBeingChangedForUserAtCentre(userId, centre, email))
                    {
                        if (emailVerificationService.AccountEmailRequiresVerification(userId, email))
                        {
                            unverifiedModifiedEmails.Add((email, centre));
                        }
                        else
                        {
                            verifiedModifiedEmails.Add((email, centre));
                        }
                    }
                }

                userService.UpdateUserDetails(accountDetailsData, true);
                userService.SetCentreEmails(userId, formData.CentreSpecificEmailsByCentreId);
                emailVerificationService.SendVerificationEmails(
                    userEntity!.UserAccount,
                    verifiedModifiedEmails,
                    unverifiedModifiedEmails
                );
            }

            if (unverifiedModifiedEmails.Any())
            {
                if (centreId != null)
                {
                    var claim = ((ClaimsIdentity)User.Identity).FindFirst("IsPersistent");
                    var isPersistent = claim != null && Convert.ToBoolean(claim.Value);
                    await HttpContext.Logout();
                    await this.CentrelessLogInAsync(userEntity.UserAccount, isPersistent);
                }

                var emailVerificationReason = EmailVerificationReason.EmailChanged;
                return RedirectToAction("Index", "VerifyYourEmail", new { emailVerificationReason });
            }

            return this.RedirectToReturnUrl(formData.ReturnUrl, logger) ?? RedirectToAction(
                "Index",
                new { dlsSubApplication = dlsSubApplication.UrlSegment }
            );
        }

        private void ValidateSingleCentreEmail(string? email, int centreId, int userId)
        {
            if (IsCentreSpecificEmailAlreadyInUse(email, centreId, userId))
            {
                ModelState.AddModelError(
                    nameof(MyAccountEditDetailsFormData.CentreSpecificEmail),
                    CommonValidationErrorMessages.CentreEmailAlreadyInUse
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
                        CommonValidationErrorMessages.CentreEmailAlreadyInUse
                    );
                }
            }
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
