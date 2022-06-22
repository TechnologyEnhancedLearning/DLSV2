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
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly PromptsService promptsService;
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
            var delegateEntity = userService.GetDelegateById(delegateId);

            if (delegateEntity == null || delegateEntity.DelegateAccount.CentreId != centreId)
            {
                return NotFound();
            }

            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();

            var customPrompts =
                promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(delegateEntity, centreId);
            var model = new EditDelegateViewModel(delegateEntity, jobGroups, customPrompts);

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(EditDelegateFormData formData, int delegateId)
        {
            var centreId = User.GetCentreId();

            promptsService.ValidateCentreRegistrationPrompts(formData, centreId, ModelState);

            ProfessionalRegistrationNumberHelper.ValidateProfessionalRegistrationNumber(
                ModelState,
                formData.HasProfessionalRegistrationNumber,
                formData.ProfessionalRegistrationNumber
            );

            if (!ModelState.IsValid)
            {
                return ReturnToEditDetailsViewWithErrors(formData, delegateId, centreId);
            }

            var delegateEntity = userService.GetDelegateById(delegateId);

            var centreEmailDefaultsToPrimary =
                formData.CentreSpecificEmail == delegateEntity!.UserAccount.PrimaryEmail &&
                delegateEntity.UserCentreDetails?.Email == null;

            if (centreEmailDefaultsToPrimary || string.IsNullOrWhiteSpace(formData.CentreSpecificEmail))
            {
                formData.CentreSpecificEmail = null;
            }

            if (formData.CentreSpecificEmail != null && !userService.NewEmailAddressIsValid(
                formData.CentreSpecificEmail,
                delegateEntity!.UserAccount.Id
            ))
            {
                ModelState.AddModelError(
                    nameof(EditDetailsFormData.CentreSpecificEmail),
                    CommonValidationErrorMessages.EmailAlreadyInUse
                );
                return ReturnToEditDetailsViewWithErrors(formData, delegateId, centreId);
            }

            var (editDelegateDetailsData, delegateDetailsData) = AccountDetailsDataHelper.MapToEditAccountDetailsData(
                formData,
                delegateEntity.UserAccount.Id,
                delegateId
            );
            userService.UpdateUserDetailsAndCentreSpecificDetails(
                editDelegateDetailsData,
                delegateDetailsData,
                formData.CentreSpecificEmail,
                centreId,
                false
            );

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
