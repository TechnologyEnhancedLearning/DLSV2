namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/CourseDelegates")]
    public class CourseDelegatesController : Controller
    {
        private readonly ICourseDataService courseDataService;
        private readonly ICourseDelegatesDataService courseDelegatesDataService;
        private readonly IUserDataService userDataService;

        public CourseDelegatesController(
            ICourseDataService courseDataService,
            IUserDataService userDataService,
            ICourseDelegatesDataService courseDelegatesDataService
        )
        {
            this.courseDataService = courseDataService;
            this.userDataService = userDataService;
            this.courseDelegatesDataService = courseDelegatesDataService;
        }

        public IActionResult Index(int? customisationId = null)
        {
            var adminId = User.GetAdminId()!.Value;
            var centreId = User.GetCentreId();
            var adminUser = userDataService.GetAdminUserById(adminId)!;

            var courses = courseDataService.GetCoursesAtCentreForCategoryId(centreId, adminUser.CategoryId).ToList();

            var currentCustomisationId = customisationId ?? courses.FirstOrDefault()?.CustomisationId;

            // TODO: HEEDLS-564 - paginate properly instead of taking 10.
            var courseDelegates = currentCustomisationId.HasValue
                ? courseDelegatesDataService.GetDelegatesOnCourse(currentCustomisationId.Value, centreId)
                    .Take(10).ToList()
                : new List<CourseDelegate>();
            var model = new CourseDelegatesViewModel(courses, courseDelegates, currentCustomisationId);

            return View(model);
        }
    }
}
