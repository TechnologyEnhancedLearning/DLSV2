namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Dashboard
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.TopCourses;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Centre))]
    [Route("/TrackingSystem/Centre/TopCourses")]
    public class TopCoursesController : Controller
    {
        private readonly ICourseService courseService;
        private const int NumberOfTopCourses = 10;

        public TopCoursesController(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var adminCategoryId = User.GetAdminCategoryId();

            var topCourses =
                courseService.GetTopCourseStatistics(centreId, adminCategoryId).Take(NumberOfTopCourses);

            var model = new TopCoursesViewModel(topCourses);

            return View(model);
        }
    }
}
