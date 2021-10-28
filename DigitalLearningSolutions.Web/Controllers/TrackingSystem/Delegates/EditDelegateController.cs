namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EditDelegate;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/{delegateId:int}/Edit")]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    public class EditDelegateController : Controller
    {
        private readonly CentreCustomPromptHelper centreCustomPromptHelper;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserService userService;

        public EditDelegateController(
            IUserService userService,
            IJobGroupsDataService jobGroupsDataService,
            CentreCustomPromptHelper customPromptHelper
        )
        {
            this.userService = userService;
            this.jobGroupsDataService = jobGroupsDataService;
            centreCustomPromptHelper = customPromptHelper;
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
                centreCustomPromptHelper.GetEditCustomFieldViewModelsForCentre(delegateUser, centreId);
            var model = new EditDelegateViewModel(delegateUser, jobGroups, customPrompts);

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(EditDelegateFormData formData, int delegateId)
        {
            var centreId = User.GetCentreId();
            if (!formData.JobGroupId.HasValue)
            {
                ModelState.AddModelError(nameof(EditDetailsFormData.JobGroupId), "Select a job group");
            }

            centreCustomPromptHelper.ValidateCustomPrompts(formData, centreId, ModelState);

            if (!userService.NewEmailAddressIsValid(formData.Email!, null, delegateId, centreId))
            {
                ModelState.AddModelError(
                    nameof(EditDetailsFormData.Email),
                    "A user with this email address is already registered at this centre"
                );
            }

            if (!userService.NewAliasIsValid(formData.AliasId, delegateId, centreId))
            {
                ModelState.AddModelError(
                    nameof(EditDelegateFormData.AliasId),
                    "A user with this alias ID is already registered at this centre"
                );
            }

            if (!ModelState.IsValid)
            {
                var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
                var customPrompts =
                    centreCustomPromptHelper.GetEditCustomFieldViewModelsForCentre(formData, centreId);
                var model = new EditDelegateViewModel(formData, jobGroups, customPrompts, delegateId);
                return View(model);
            }

            var (accountDetailsData, centreAnswersData) = AccountDetailsDataHelper.MapToUpdateAccountData(
                formData,
                delegateId,
                User.GetCentreId()
            );
            userService.UpdateUserAccountDetailsByAdmin(accountDetailsData, centreAnswersData);

            return RedirectToAction("Index", "ViewDelegate", new { delegateId });
        }
    }
}
