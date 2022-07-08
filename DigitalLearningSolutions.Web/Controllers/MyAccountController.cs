namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
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
        private readonly IUserService userService;

        public MyAccountController(
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            IUserService userService,
            IImageResizeService imageResizeService,
            IJobGroupsDataService jobGroupsDataService,
            PromptsService registrationPromptsService,
            ILogger<MyAccountController> logger,
            IConfiguration config
        )
        {
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.userService = userService;
            this.imageResizeService = imageResizeService;
            this.jobGroupsDataService = jobGroupsDataService;
            promptsService = registrationPromptsService;
            this.logger = logger;
            this.config = config;
        }

        // TODO HEEDLS-993 Sort out my account page for centreless user, only the minimum has been done to allow it to load
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

            var switchCentreReturnUrl = StringHelper.GetLocalRedirectUrl(config, SwitchCentreReturnUrl);

            var model = new MyAccountViewModel(
                userEntity.UserAccount,
                delegateAccount,
                adminAccount?.CentreName ?? delegateAccount?.CentreName,
                centreId != null ? userService.GetCentreEmail(userId, centreId.Value) : null,
                customPrompts,
                dlsSubApplication,
                switchCentreReturnUrl
            );

            return View(model);
        }

        // TODO HEEDLS-965 Sort out edit details for centreless user, only the minimum has been done to allow it to load
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

            var model = new MyAccountEditDetailsViewModel(
                userEntity.UserAccount,
                delegateAccount,
                jobGroups,
                centreId != null ? userService.GetCentreEmail(userId, centreId.Value) : null,
                customPrompts,
                dlsSubApplication,
                returnUrl,
                isCheckDetailsRedirect
            );

            return View(model);
        }

        // TODO HEEDLS-965 Edit details post fails for centreless user, contains call to User.GetCentreIdKnownNotNull()
        [NoCaching]
        [HttpPost("EditDetails")]
        public IActionResult EditDetails(
            MyAccountEditDetailsFormData formData,
            string action,
            DlsSubApplication dlsSubApplication
        )
        {
            return action switch
            {
                "save" => EditDetailsPostSave(formData, dlsSubApplication),
                "previewImage" => EditDetailsPostPreviewImage(formData, dlsSubApplication),
                "removeImage" => EditDetailsPostRemoveImage(formData, dlsSubApplication),
                _ => new StatusCodeResult(500),
            };
        }

        private IActionResult EditDetailsPostSave(
            MyAccountEditDetailsFormData formData,
            DlsSubApplication dlsSubApplication
        )
        {
            var userDelegateId = User.GetCandidateId();
            var userId = User.GetUserIdKnownNotNull();

            if (userDelegateId.HasValue)
            {
                promptsService.ValidateCentreRegistrationPrompts(formData, User.GetCentreIdKnownNotNull(), ModelState);
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
                return ReturnToEditDetailsViewWithErrors(formData, dlsSubApplication);
            }

            var emailsValid = true;
            if (!userService.NewEmailAddressIsValid(
                    formData.Email!,
                    userId
                ))
            {
                ModelState.AddModelError(
                    nameof(MyAccountEditDetailsFormData.Email),
                    CommonValidationErrorMessages.EmailAlreadyInUse
                );
                emailsValid = false;
            }

            if (!string.IsNullOrWhiteSpace(formData.CentreSpecificEmail) && !userService.NewEmailAddressIsValid(
                    formData.CentreSpecificEmail,
                    userId
                ))
            {
                ModelState.AddModelError(
                    nameof(MyAccountEditDetailsFormData.CentreSpecificEmail),
                    CommonValidationErrorMessages.EmailAlreadyInUse
                );
                emailsValid = false;
            }

            if (!emailsValid)
            {
                return ReturnToEditDetailsViewWithErrors(formData, dlsSubApplication);
            }

            var (accountDetailsData, delegateDetailsData) = AccountDetailsDataHelper.MapToEditAccountDetailsData(
                formData,
                userId,
                userDelegateId
            );
            userService.UpdateUserDetailsAndCentreSpecificDetails(
                accountDetailsData,
                delegateDetailsData,
                formData.CentreSpecificEmail,
                User.GetCentreIdKnownNotNull(),
                true
            );

            return this.RedirectToReturnUrl(formData.ReturnUrl, logger) ?? RedirectToAction(
                "Index",
                new { dlsSubApplication = dlsSubApplication.UrlSegment }
            );
        }

        private IActionResult ReturnToEditDetailsViewWithErrors(
            MyAccountEditDetailsFormData formData,
            DlsSubApplication dlsSubApplication
        )
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            var customPrompts =
                promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(
                    formData,
                    User.GetCentreIdKnownNotNull()
                );
            var model = new MyAccountEditDetailsViewModel(formData, jobGroups, customPrompts, dlsSubApplication);
            return View(model);
        }

        private IActionResult EditDetailsPostPreviewImage(
            MyAccountEditDetailsFormData formData,
            DlsSubApplication dlsSubApplication
        )
        {
            // We don't want to display validation errors on other fields in this case
            ModelState.ClearErrorsForAllFieldsExcept(nameof(MyAccountEditDetailsViewModel.ProfileImageFile));

            var userDelegateId = User.GetCandidateId();
            var (_, delegateUser) = userService.GetUsersById(null, userDelegateId);
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            var customPrompts =
                promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(
                    delegateUser,
                    User.GetCentreIdKnownNotNull()
                );

            if (!ModelState.IsValid)
            {
                return View(new MyAccountEditDetailsViewModel(formData, jobGroups, customPrompts, dlsSubApplication));
            }

            if (formData.ProfileImageFile != null)
            {
                ModelState.Remove(nameof(MyAccountEditDetailsFormData.ProfileImage));
                formData.ProfileImage = imageResizeService.ResizeProfilePicture(formData.ProfileImageFile);
            }

            var model = new MyAccountEditDetailsViewModel(formData, jobGroups, customPrompts, dlsSubApplication);
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

            var userDelegateId = User.GetCandidateId();
            var (_, delegateUser) = userService.GetUsersById(null, userDelegateId);
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            var customPrompts =
                promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(
                    delegateUser,
                    User.GetCentreIdKnownNotNull()
                );

            var model = new MyAccountEditDetailsViewModel(formData, jobGroups, customPrompts, dlsSubApplication);
            return View(model);
        }

        private static DelegateAccount? GetDelegateAccountIfActive(UserEntity? user, int? centreId)
        {
            var delegateAccount = user?.GetCentreAccountSet(centreId)?.DelegateAccount;

            return delegateAccount is { Active: true } ? delegateAccount : null;
        }
    }
}
