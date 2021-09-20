namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/CourseSetup/{customisationId:int}/Content")]
    public class CourseContentController : Controller
    {
        private readonly ICourseDataService courseDataService;
        private readonly ISectionService sectionService;

        public CourseContentController(ICourseDataService courseDataService, ISectionService sectionService)
        {
            this.courseDataService = courseDataService;
            this.sectionService = sectionService;
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

            var courseSections = sectionService.GetSectionsAndTutorialsForCustomisation(
                customisationId,
                courseDetails.ApplicationId
            );
            var model = new CourseContentViewModel(
                customisationId,
                courseDetails.CourseName,
                courseDetails.PostLearningAssessment,
                courseSections
            );

            return View(model);
        }

        [HttpGet]
        [Route("Edit")]
        public IActionResult Edit(int customisationId)
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

            var courseSections = sectionService.GetSectionsAndTutorialsForCustomisation(
                customisationId,
                courseDetails.ApplicationId
            );
            var model = new EditCourseContentViewModel(
                customisationId,
                courseDetails.CourseName,
                courseSections
            );

            return View(model);
        }

        [HttpPost]
        [Route("Edit")]
        public IActionResult Edit(EditCourseContentViewModel model, int customisationId)
        {
            var sections = model.Sections.Select(
                s => new Section(
                    s.SectionId,
                    s.SectionName,
                    s.Tutorials.Select(
                        t => new Tutorial(t.TutorialId, t.TutorialName, t.LearningEnabled, t.DiagnosticEnabled)
                    )
                )
            );

            sectionService.UpdateSectionTutorialDiagnosticsAndLearningEnabled(sections, customisationId);

            return RedirectToAction("Index", new { customisationId });
        }
    }
}
