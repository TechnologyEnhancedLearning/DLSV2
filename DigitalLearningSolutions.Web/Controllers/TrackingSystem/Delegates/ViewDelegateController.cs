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
        private readonly ICourseService courseService;
        private readonly CustomPromptHelper customPromptHelper;
        private readonly IUserDataService userDataService;

        public ViewDelegateController(
            IUserDataService userDataService,
            CustomPromptHelper customPromptHelper,
            ICourseService courseService
        )
        {
            this.userDataService = userDataService;
            this.customPromptHelper = customPromptHelper;
            this.courseService = courseService;
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

            var courseInfoViewModels = courseService.GetAllCoursesForDelegate(delegateId, centreId)
                .Select(x => new DelegateCourseInfoViewModel(x.info, x.prompts, x.stats));

            var model = new ViewDelegateViewModel(delegateInfoViewModel, courseInfoViewModels);
            return View(model);
        }
    }
}
