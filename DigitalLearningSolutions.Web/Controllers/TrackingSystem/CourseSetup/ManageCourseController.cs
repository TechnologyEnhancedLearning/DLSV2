namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/CourseSetup/{customisationId:int}/Manage")]
    public class ManageCourseController : Controller
    {
        private readonly ICourseDataService courseDataService;
        private readonly ICourseService courseService;

        public ManageCourseController(ICourseService courseService, ICourseDataService courseDataService)
        {
            this.courseService = courseService;
            this.courseDataService = courseDataService;
        }

        [HttpGet]
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
        [Route("LearningPathwayDefaults")]
        public IActionResult EditLearningPathwayDefaults(int customisationId)
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

            var model = new EditLearningPathwayDefaultsViewModel(courseDetails);

            return View(model);
        }

        [HttpPost]
        [Route("LearningPathwayDefaults")]
        public IActionResult SaveLearningPathwayDefaults(
            int customisationId,
            EditLearningPathwayDefaultsViewModel model
        )
        {
            if (customisationId != model.CustomisationId)
            {
                return new StatusCodeResult(500);
            }

            ValidateNumberInput(model.CompleteWithinMonths);
            ValidateNumberInput(model.ValidityMonths);

            if (!ModelState.IsValid)
            {
                return View("EditLearningPathwayDefaults", model);
            }

            if (model.AutoRefresh)
            {
                // Redirect to "Edit auto-refresh options" page
                // To be configured in HEEDLS-442
            }

            var completeWithinMonthsInt =
                model.CompleteWithinMonths == null ? 0 : int.Parse(model.CompleteWithinMonths);
            var validityMonthsInt =
                model.ValidityMonths == null ? 0 : int.Parse(model.ValidityMonths);

            courseService.UpdateLearningPathwayDefaultsForCourse(
                model.CustomisationId,
                completeWithinMonthsInt,
                validityMonthsInt,
                model.Mandatory,
                model.AutoRefresh
            );

            return RedirectToAction("Index", new { customisationId = model.CustomisationId });
        }

        private void ValidateNumberInput(string? numberInput)
        {
            if (numberInput == null)
            {
                return;
            }

            if (!int.TryParse(numberInput, out _))
            {
                ModelState.AddModelError(nameof(numberInput), "Value must only contain numbers");
            }
            else if (int.Parse(numberInput) < 0 || int.Parse(numberInput) > 48)
            {
                ModelState.AddModelError(nameof(numberInput), "Value must be a number between 0 and 48");
            }
        }
    }
}
