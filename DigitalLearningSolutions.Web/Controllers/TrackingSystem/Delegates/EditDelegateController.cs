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
            var delegateUser = userService.GetUsersById(null, delegateId).delegateUser;

            if (delegateUser == null || delegateUser.CentreId != User.GetCentreId())
            {
                return NotFound();
            }

            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();

            var model = new EditDelegateViewModel(delegateUser, jobGroups, GetCustomFieldsWithDelegateAnswers(delegateUser));

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(EditDelegateFormData formData, int delegateId)
        {
            ValidateJobGroup(formData);
            ValidateCustomPrompts(formData);

            if (!userService.NewEmailAddressIsValid(formData.Email!, null, delegateId, User.GetCentreId()))
            {
                ModelState.AddModelError(
                    nameof(EditDetailsFormData.Email),
                    "A user with this email address is already registered at this centre"
                );
            }

            if (!userService.NewAliasIsValid(formData.AliasId, delegateId, User.GetCentreId()))
            {
                ModelState.AddModelError(
                    nameof(EditDelegateFormData.AliasId),
                    "A user with this alias ID is already registered at this centre"
                );
            }

            if (!ModelState.IsValid)
            {
                var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
                var model = new EditDelegateViewModel(formData, jobGroups, GetCustomFieldsWithEnteredAnswers(formData), delegateId);
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

        private void ValidateJobGroup(EditDetailsFormData formData)
        {
            if (!formData.JobGroupId.HasValue)
            {
                ModelState.AddModelError(nameof(EditDetailsFormData.JobGroupId), "Select a job group");
            }
        }

        private void ValidateCustomPrompts(EditDetailsFormData formData)
        {
            centreCustomPromptHelper.ValidateCustomPrompts(
                User.GetCentreId(),
                formData.Answer1,
                formData.Answer2,
                formData.Answer3,
                formData.Answer4,
                formData.Answer5,
                formData.Answer6,
                ModelState
            );
        }

        private List<EditCustomFieldViewModel> GetCustomFieldsWithDelegateAnswers(DelegateUser? delegateUser)
        {
            return centreCustomPromptHelper.GetEditCustomFieldViewModelsForCentre(
                User.GetCentreId(),
                delegateUser?.Answer1,
                delegateUser?.Answer2,
                delegateUser?.Answer3,
                delegateUser?.Answer4,
                delegateUser?.Answer5,
                delegateUser?.Answer6
            );
        }

        private List<EditCustomFieldViewModel> GetCustomFieldsWithEnteredAnswers(EditDetailsFormData formData)
        {
            return centreCustomPromptHelper.GetEditCustomFieldViewModelsForCentre(
                User.GetCentreId(),
                formData.Answer1,
                formData.Answer2,
                formData.Answer3,
                formData.Answer4,
                formData.Answer5,
                formData.Answer6
            );
        }
    }
}
