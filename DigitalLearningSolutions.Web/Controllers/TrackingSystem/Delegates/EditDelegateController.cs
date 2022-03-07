namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EditDelegate;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [ServiceFilter(typeof(VerifyAdminUserCanAccessDelegateUser))]
    [Route("TrackingSystem/Delegates/{delegateId:int}/Edit")]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    public class EditDelegateController : Controller
    {
        private readonly PromptsService promptsService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserService userService;

        public EditDelegateController(
            IUserService userService,
            IJobGroupsDataService jobGroupsDataService,
            PromptsService registrationPromptsService
        )
        {
            this.userService = userService;
            this.jobGroupsDataService = jobGroupsDataService;
            promptsService = registrationPromptsService;
        }

        [HttpGet]
        public IActionResult Index(int delegateId)
        {
            var centreId = User.GetCentreId();
            var delegateUser = userService.GetUsersById(null, delegateId).delegateUser;

            if (delegateUser == null || delegateUser.CentreId != centreId)
            {
                return NotFound();
            }

            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();

            var customPrompts =
                promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(delegateUser, centreId);
            var model = new EditDelegateViewModel(delegateUser, jobGroups, customPrompts);

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(EditDelegateFormData formData, int delegateId)
        {
            var centreId = User.GetCentreId();

            promptsService.ValidateCentreRegistrationPrompts(formData, centreId, ModelState);

            if (!userService.NewAliasIsValid(formData.AliasId, delegateId, centreId))
            {
                ModelState.AddModelError(
                    nameof(EditDelegateFormData.AliasId),
                    "A user with this alias ID is already registered at this centre"
                );
            }

            ProfessionalRegistrationNumberHelper.ValidateProfessionalRegistrationNumber(
                ModelState,
                formData.HasProfessionalRegistrationNumber,
                formData.ProfessionalRegistrationNumber
            );

            if (!ModelState.IsValid)
            {
                return ReturnToEditDetailsViewWithErrors(formData, delegateId, centreId);
            }

            if (!userService.NewEmailAddressIsValid(formData.Email!, null, delegateId, centreId))
            {
                ModelState.AddModelError(
                    nameof(EditDetailsFormData.Email),
                    "A user with this email address is already registered at this centre"
                );
                return ReturnToEditDetailsViewWithErrors(formData, delegateId, centreId);
            }

            var (accountDetailsData, centreAnswersData) = AccountDetailsDataHelper.MapToUpdateAccountData(
                formData,
                delegateId,
                User.GetCentreId()
            );
            userService.UpdateUserAccountDetailsViaDelegateAccount(accountDetailsData, centreAnswersData);

            return RedirectToAction("Index", "ViewDelegate", new { delegateId });
        }

        private IActionResult ReturnToEditDetailsViewWithErrors(
            EditDelegateFormData formData,
            int delegateId,
            int centreId
        )
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            var customPrompts =
                promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(formData, centreId);
            var model = new EditDelegateViewModel(formData, jobGroups, customPrompts, delegateId);
            return View(model);
        }
    }
}
