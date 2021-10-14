namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/CourseSetup")]
    public class ManageCourseController : Controller
    {
        private readonly ICourseDataService courseDataService;

        public ManageCourseController(ICourseDataService courseDataService)
        {
            this.courseDataService = courseDataService;
        }

        [HttpGet]
        [Route("{customisationId:int}/Manage")]
        public IActionResult Index(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;

            var courseDetails = courseDataService.GetCourseDetailsForAdminCategoryId(
                customisationId,
                centreId,
                categoryId.Value
            );

            if (courseDetails == null)
            {
                return NotFound();
            }

            var model = new ManageCourseViewModel(courseDetails);

            return View(model);
        }

        [HttpGet]
        [Route("{customisationId:int}/EditCourseOptions")]
        public IActionResult EditCourseOptions(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;

            var courseOptions = courseDataService.GetCourseOptionsForAdminCategoryId(
                customisationId,
                centreId,
                categoryId.Value
            );

            if (courseOptions is null)
            {
                return NotFound();
            }

            var model = new EditCourseOptionsViewModel(courseOptions, customisationId);
            return View(model);
        }

        [HttpPost]
        [Route("{customisationId:int}/EditCourseOptions")]
        public IActionResult EditCourseOptions(EditCourseOptionsViewModel editCourseOptionsViewModel)
        {
            var centreId = User.GetCentreId();
            var customisationId = editCourseOptionsViewModel.CustomisationId;
            var categoryId = User.GetAdminCategoryId()!;

            var courseOptions = new CourseOptions
            {
                Active = editCourseOptionsViewModel.Active,
                SelfRegister = editCourseOptionsViewModel.AllowSelfEnrolment,
                HideInLearnerPortal = editCourseOptionsViewModel.HideInLearningPortal,
                DiagObjSelect = editCourseOptionsViewModel.DiagnosticObjectiveSelection
            };

            courseDataService.TryUpdateCourseOptions(courseOptions, customisationId, centreId, categoryId);

            return RedirectToAction("Index", "ManageCourse", new { customisationId });
        }
    }
}
