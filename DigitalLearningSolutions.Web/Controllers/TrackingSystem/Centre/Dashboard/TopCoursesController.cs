﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Dashboard
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.TopCourses;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
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
            var centreId = User.GetCentreId();
            var adminCategoryId = User.GetAdminCategoryId()!;

            var topCourses =
                courseService.GetTopCourseStatistics(centreId, adminCategoryId.Value).Take(NumberOfTopCourses);

            var model = new TopCoursesViewModel(topCourses);

            return View(model);
        }
    }
}
