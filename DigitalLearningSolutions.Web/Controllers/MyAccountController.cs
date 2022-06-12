namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Route("/{dlsSubApplication}/MyAccount", Order = 1)]
    [Route("/MyAccount", Order = 2)]
    [TypeFilter(typeof(ValidateAllowedDlsSubApplication))]
    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.MyAccount))]
    [Authorize]
    public class MyAccountController : Controller
    {
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
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
            ILogger<MyAccountController> logger
        )
        {
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.userService = userService;
            this.imageResizeService = imageResizeService;
            this.jobGroupsDataService = jobGroupsDataService;
            promptsService = registrationPromptsService;
            this.logger = logger;
        }

        [NoCaching]
        [SetSelectedTab(nameof(NavMenuTab.MyAccount))]
        public IActionResult Index(DlsSubApplication dlsSubApplication)
        {
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetCandidateId();
            var (adminUser, delegateUser) = userService.GetUsersById(userAdminId, userDelegateId);

            var customPrompts =
                centreRegistrationPromptsService.GetCentreRegistrationPromptsWithAnswersByCentreIdAndDelegateUser(
                    User.GetCentreId(),
                    delegateUser
                );

            var model = new MyAccountViewModel(
                adminUser,
                delegateUser,
                userService.GetCentreEmail(User.GetUserId()!.Value, User.GetCentreId()),
                customPrompts,
                dlsSubApplication
            );

            return View(model);
        }

        [NoCaching]
        [HttpGet("EditDetails")]
        [SetSelectedTab(nameof(NavMenuTab.MyAccount))]
        public IActionResult EditDetails(
            DlsSubApplication dlsSubApplication,
            string? returnUrl = null,
            bool isCheckDetailsRedirect = false
        )
        {
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetCandidateId();
            var (adminUser, delegateUser) = userService.GetUsersById(userAdminId, userDelegateId);

            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            var customPrompts =
                promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(delegateUser, User.GetCentreId());

            var model = new MyAccountEditDetailsViewModel(
                adminUser,
                delegateUser,
                jobGroups,
                userService.GetCentreEmail(User.GetUserId()!.Value, User.GetCentreId()),
                customPrompts,
                dlsSubApplication,
                returnUrl,
                isCheckDetailsRedirect
            );

            return View(model);
        }

        [NoCaching]
        [HttpPost("EditDetails")]
        [SetSelectedTab(nameof(NavMenuTab.MyAccount))]
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
            var userId = User.GetUserId()!.Value;

            if (userDelegateId.HasValue)
            {
                promptsService.ValidateCentreRegistrationPrompts(formData, User.GetCentreId(), ModelState);
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
                formData.ProfessionalRegistrationNumber,
                userDelegateId.HasValue
            );

            if (!ModelState.IsValid)
            {
                return ReturnToEditDetailsViewWithErrors(formData, dlsSubApplication);
            }

            if (!userService.NewEmailAddressIsValid(
                    formData.Email!,
                    userId
                ))
            {
                ModelState.AddModelError(
                    nameof(MyAccountEditDetailsFormData.Email),
                    CommonValidationErrorMessages.EmailAlreadyInUse
                );
                return ReturnToEditDetailsViewWithErrors(formData, dlsSubApplication);
            }

            if (!string.IsNullOrWhiteSpace(formData.CentreEmail) && !userService.NewEmailAddressIsValid(
                    formData.CentreEmail,
                    userId
                ))
            {
                ModelState.AddModelError(
                    nameof(MyAccountEditDetailsFormData.CentreEmail),
                    CommonValidationErrorMessages.EmailAlreadyInUse
                );
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
                formData.CentreEmail,
                User.GetCentreId()
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
                promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(formData, User.GetCentreId());
            var model = new MyAccountEditDetailsViewModel(formData, jobGroups, customPrompts, dlsSubApplication);
            return View(model);
        }

        [SetSelectedTab(nameof(NavMenuTab.MyAccount))]
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
                promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(delegateUser, User.GetCentreId());

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

        [SetSelectedTab(nameof(NavMenuTab.MyAccount))]
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
                promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(delegateUser, User.GetCentreId());

            var model = new MyAccountEditDetailsViewModel(formData, jobGroups, customPrompts, dlsSubApplication);
            return View(model);
        }
    }
}
