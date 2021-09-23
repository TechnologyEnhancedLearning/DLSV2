namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
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
        private readonly ICourseService courseService;

        public ManageCourseController(ICourseService courseService, ICourseDataService courseDataService)
        {
            this.courseService = courseService;
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
        [Route("{customisationId:int}/Manage/LearningPathwayDefaults")]
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
        [Route("{customisationId}/Manage/LearningPathwayDefaults")]
        public IActionResult SaveLearningPathwayDefaults(
            int customisationId,
            EditLearningPathwayDefaultsViewModel model
        )
        {
            if (customisationId != model.CustomisationId)
            {
                return new StatusCodeResult(500);
            }

            ValidateNumberInputs(model);

            if (!ModelState.IsValid)
            {
                return View("EditLearningPathwayDefaults", model);
            }

            courseService.UpdateLearningPathwayDefaultsForCourse(
                model.CustomisationId,
                Int32.Parse(model.CompleteWithinMonths!),
                Int32.Parse(model.ValidityMonths!),
                model.Mandatory,
                model.AutoRefresh
            );

            return RedirectToAction("Index", new { customisationId = model.CustomisationId });
        }

        private void ValidateNumberInputs(EditLearningPathwayDefaultsViewModel model)
        {
            if (model.CompleteWithinMonths == null)
            {
                ModelState.AddModelError(nameof(model.CompleteWithinMonths), "Enter the number of months that the course must be completed within");
            } else if (!Int32.TryParse(model.CompleteWithinMonths, out _))
            {
                ModelState.AddModelError(nameof(model.CompleteWithinMonths), "Value must only contain numbers");
            } else if (Int32.Parse(model.CompleteWithinMonths) < 0 || Int32.Parse(model.CompleteWithinMonths) > 48)
            {
                ModelState.AddModelError(nameof(model.CompleteWithinMonths), "Value must be a number between 0 and 48");
            }

            if (model.ValidityMonths == null)
            {
                ModelState.AddModelError(nameof(model.ValidityMonths), "Enter the number of months that the completion will remain valid for");
            } else if (!Int32.TryParse(model.ValidityMonths, out _))
            {
                ModelState.AddModelError(nameof(model.ValidityMonths), "Value must only contain numbers.");
            } else if (Int32.Parse(model.ValidityMonths) < 0 || Int32.Parse(model.ValidityMonths) > 48)
            {
                ModelState.AddModelError(nameof(model.ValidityMonths), "Value must be a number between 0 and 48");
            }
        }
    }
}
