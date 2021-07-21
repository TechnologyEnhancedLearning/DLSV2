namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/View/{delegateId:int}")]
    public class ViewDelegateController : Controller
    {
        private readonly ICourseDataService courseDataService;
        private readonly CustomPromptHelper customPromptHelper;
        private readonly ICustomPromptsService customPromptsService;
        private readonly IUserDataService userDataService;

        public ViewDelegateController(
            IUserDataService userDataService,
            CustomPromptHelper customPromptHelper,
            ICourseDataService courseDataService,
            ICustomPromptsService customPromptsService
        )
        {
            this.userDataService = userDataService;
            this.customPromptHelper = customPromptHelper;
            this.courseDataService = courseDataService;
            this.customPromptsService = customPromptsService;
        }

        public IActionResult Index(int delegateId)
        {
            var centreId = User.GetCentreId();

            var delegateUser = userDataService.GetDelegateUserCardById(delegateId);
            if (delegateUser == null || delegateUser.CentreId != centreId)
            {
                return new NotFoundResult();
            }

            var customFields = customPromptHelper.GetCustomFieldViewModelsForCentre(centreId, delegateUser);
            var delegateInfoViewModel = new DelegateInfoViewModel(delegateUser, customFields);

            var courseInfoViewModelList = courseDataService.GetDelegateCoursesInfo(delegateId)
                .Select(
                    info =>
                    {
                        var courseCustomPrompts = customPromptsService.GetCustomPromptsForCourse(
                            info.CustomisationId,
                            centreId,
                            includeArchived: true
                        );
                        var attemptStats =
                            courseDataService.GetDelegateCourseAttemptStats(delegateId, info.CustomisationId);
                        return new DelegateCourseInfoViewModel(info, courseCustomPrompts?.CourseAdminFields, attemptStats);
                    }
                );

            var model = new ViewDelegateViewModel(delegateInfoViewModel, courseInfoViewModelList);
            return View(model);
        }
    }
}
