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

    [Route("/{dlsSubApplication}/MyAccount", Order = 1)]
    [Route("/MyAccount", Order = 2)]
    [TypeFilter(typeof(ValidateAllowedDlsSubApplication))]
    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.MyAccount))]
    [Authorize]
    public class MyAccountController : Controller
    {
        private readonly CentreRegistrationPromptHelper centreRegistrationPromptHelper;
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
        private readonly IImageResizeService imageResizeService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserService userService;

        public MyAccountController(
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            IUserService userService,
            IImageResizeService imageResizeService,
            IJobGroupsDataService jobGroupsDataService,
            CentreRegistrationPromptHelper registrationPromptHelper
        )
        {
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.userService = userService;
            this.imageResizeService = imageResizeService;
            this.jobGroupsDataService = jobGroupsDataService;
            centreRegistrationPromptHelper = registrationPromptHelper;
        }

        [NoCaching]
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

            var model = new MyAccountViewModel(adminUser, delegateUser, customPrompts, dlsSubApplication);

            return View(model);
        }

        [NoCaching]
        [HttpGet("EditDetails")]
        public IActionResult EditDetails(DlsSubApplication dlsSubApplication)
        {
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetCandidateId();
            var (adminUser, delegateUser) = userService.GetUsersById(userAdminId, userDelegateId);

            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            var customPrompts =
                centreRegistrationPromptHelper.GetEditDelegateRegistrationPromptViewModelsForCentre(delegateUser, User.GetCentreId());

            var model = new MyAccountEditDetailsViewModel(
                adminUser,
                delegateUser,
                jobGroups,
                customPrompts,
                dlsSubApplication
            );

            return View(model);
        }

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
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetCandidateId();

            if (userDelegateId.HasValue)
            {
                centreRegistrationPromptHelper.ValidateCentreRegistrationPrompts(formData, User.GetCentreId(), ModelState);
            }

            if (formData.ProfileImageFile != null)
            {
                ModelState.AddModelError(
                    nameof(MyAccountEditDetailsFormData.ProfileImageFile),
                    "Preview your new profile picture before saving"
                );
            }

            if (formData.Password != null &&
                !userService.IsPasswordValid(userAdminId, userDelegateId, formData.Password))
            {
                ModelState.AddModelError(
                    nameof(MyAccountEditDetailsFormData.Password),
                    CommonValidationErrorMessages.IncorrectPassword
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
                return ReturnToEditDetailsViewWithErrors(formData, dlsSubApplication, userDelegateId);
            }

            if (!userService.NewEmailAddressIsValid(formData.Email!, userAdminId, userDelegateId, User.GetCentreId()))
            {
                ModelState.AddModelError(
                    nameof(MyAccountEditDetailsFormData.Email),
                    "A user with this email address is already registered at this centre"
                );
                return ReturnToEditDetailsViewWithErrors(formData, dlsSubApplication, userDelegateId);
            }

            var (accountDetailsData, centreAnswersData) = AccountDetailsDataHelper.MapToUpdateAccountData(
                formData,
                userAdminId,
                userDelegateId,
                User.GetCentreId()
            );
            userService.UpdateUserAccountDetailsForAllVerifiedUsers(accountDetailsData, centreAnswersData);

            return RedirectToAction("Index", new { dlsSubApplication = dlsSubApplication.UrlSegment });
        }

        private IActionResult ReturnToEditDetailsViewWithErrors(
            MyAccountEditDetailsFormData formData,
            DlsSubApplication dlsSubApplication,
            int? userDelegateId
        )
        {
            var (_, delegateUser) = userService.GetUsersById(null, userDelegateId);
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            var customPrompts =
                centreRegistrationPromptHelper.GetEditDelegateRegistrationPromptViewModelsForCentre(delegateUser, User.GetCentreId());
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
                centreRegistrationPromptHelper.GetEditDelegateRegistrationPromptViewModelsForCentre(delegateUser, User.GetCentreId());

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
                centreRegistrationPromptHelper.GetEditDelegateRegistrationPromptViewModelsForCentre(delegateUser, User.GetCentreId());

            var model = new MyAccountEditDetailsViewModel(formData, jobGroups, customPrompts, dlsSubApplication);
            return View(model);
        }
    }
}
