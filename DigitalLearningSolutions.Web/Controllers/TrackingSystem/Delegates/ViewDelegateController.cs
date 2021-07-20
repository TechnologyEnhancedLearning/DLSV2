namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
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
        private readonly IUserDataService userDataService;

        public ViewDelegateController(
            IUserDataService userDataService,
            CustomPromptHelper customPromptHelper,
            ICourseDataService courseDataService
        )
        {
            this.userDataService = userDataService;
            this.customPromptHelper = customPromptHelper;
            this.courseDataService = courseDataService;
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
                .Select(info => new DelegateCourseInfoViewModel(info));

            var model = new ViewDelegateViewModel(delegateInfoViewModel, courseInfoViewModelList);
            return View(model);
        }
    }
}
