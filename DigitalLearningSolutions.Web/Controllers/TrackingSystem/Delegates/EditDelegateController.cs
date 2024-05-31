﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
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
        private readonly IJobGroupsService jobGroupsService;
        private readonly PromptsService promptsService;
        private readonly IUserService userService;

        public EditDelegateController(
            IUserService userService,
            IJobGroupsService jobGroupsService,
            PromptsService registrationPromptsService
        )
        {
            this.userService = userService;
            this.jobGroupsService = jobGroupsService;
            promptsService = registrationPromptsService;
        }

        [HttpGet]
        public IActionResult Index(int delegateId)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var delegateEntity = userService.GetDelegateById(delegateId);

            if (delegateEntity == null || delegateEntity.DelegateAccount.CentreId != centreId)
            {
                return NotFound();
            }

            var jobGroups = jobGroupsService.GetJobGroupsAlphabetical().ToList();

            var customPrompts =
                promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(delegateEntity, centreId);
            var model = new EditDelegateViewModel(delegateEntity, jobGroups, customPrompts);

            if (DisplayStringHelper.IsGuid(model.CentreSpecificEmail))
                model.CentreSpecificEmail = null;

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(EditDelegateFormData formData, int delegateId)
        {
            var centreId = User.GetCentreIdKnownNotNull();

            promptsService.ValidateCentreRegistrationPrompts(formData, centreId, ModelState);

            ProfessionalRegistrationNumberHelper.ValidateProfessionalRegistrationNumber(
                ModelState,
                formData.HasProfessionalRegistrationNumber,
                formData.ProfessionalRegistrationNumber
            );

            if (string.IsNullOrEmpty(formData.CentreSpecificEmail))
            {
                ModelState.AddModelError(nameof(formData.CentreSpecificEmail), "Enter an email address");
            }

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

            if (
                formData.CentreSpecificEmail != null &&
                formData.CentreSpecificEmail != delegateEntity.UserCentreDetails?.Email &&
                userService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    formData.CentreSpecificEmail,
                    delegateEntity.DelegateAccount.CentreId,
                    delegateEntity.UserAccount.Id
                )
            )
            {
                ModelState.AddModelError(
                    nameof(EditDetailsFormData.CentreSpecificEmail),
                    CommonValidationErrorMessages.EmailInUseAtCentre
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
                false,
                !string.Equals(delegateEntity.UserCentreDetails?.Email, formData.CentreSpecificEmail),
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
            var jobGroups = jobGroupsService.GetJobGroupsAlphabetical().ToList();
            var customPrompts =
                promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(formData, centreId);
            var model = new EditDelegateViewModel(formData, jobGroups, customPrompts, delegateId);
            return View(model);
        }
    }
}
