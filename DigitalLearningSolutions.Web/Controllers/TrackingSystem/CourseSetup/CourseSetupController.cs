namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/CourseSetup")]
    public class CourseSetupController : Controller
    {
        private readonly ICourseService courseService;

        public CourseSetupController(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;

            var centreCourses = courseService.GetCentreSpecificCourseStatistics(centreId, categoryId.Value);
            var model = new CourseSetupViewModel(centreCourses.Take(10));

            return View(model);
        }
    }
}
