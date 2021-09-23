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
        public const string SelectAllDiagnosticAction = "diagnostic-select-all";
        public const string DeselectAllDiagnosticAction = "diagnostic-deselect-all";
        public const string SelectAllLearningAction = "learning-select-all";
        public const string DeselectAllLearningAction = "learning-deselect-all";
        public const string SaveAction = "Save";

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
        [Route("Edit/{sectionId:int}")]
        public IActionResult EditSection(int customisationId, int sectionId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;
            var courseDetails = courseDataService.GetCourseDetailsForAdminCategoryId(
                customisationId,
                centreId,
                categoryId.Value
            );
            var section = sectionService.GetSectionAndTutorialsBySectionId(customisationId, sectionId);

            if (courseDetails == null || section == null)
            {
                return NotFound();
            }

            var model = new EditCourseSectionViewModel(
                customisationId,
                courseDetails.CourseName,
                section
            );

            return View(model);
        }

        [HttpPost]
        [Route("Edit/{sectionId:int}")]
        public IActionResult EditSection(
            EditCourseSectionViewModel model,
            int customisationId,
            int sectionId,
            string action
        )
        {
            return action == SaveAction
                ? EditSave(model, customisationId, sectionId)
                : ProcessBulkSelect(model, action);
        }

        private IActionResult ProcessBulkSelect(EditCourseSectionViewModel model, string action)
        {
            switch (action)
            {
                case SelectAllDiagnosticAction:
                    SelectAllDiagnostics(model);
                    break;
                case DeselectAllDiagnosticAction:
                    DeselectAllDiagnostics(model);
                    break;
                case SelectAllLearningAction:
                    SelectAllLearning(model);
                    break;
                case DeselectAllLearningAction:
                    DeselectAllLearning(model);
                    break;
                default:
                    return new StatusCodeResult(500);
            }

            return View(model);
        }

        private void SelectAllDiagnostics(EditCourseSectionViewModel model)
        {
            foreach (var tutorial in model.Tutorials)
            {
                tutorial.DiagnosticEnabled = true;
            }
        }

        private void DeselectAllDiagnostics(EditCourseSectionViewModel model)
        {
            foreach (var tutorial in model.Tutorials)
            {
                tutorial.DiagnosticEnabled = false;
            }
        }

        private void SelectAllLearning(EditCourseSectionViewModel model)
        {
            foreach (var tutorial in model.Tutorials)
            {
                tutorial.LearningEnabled = true;
            }
        }

        private void DeselectAllLearning(EditCourseSectionViewModel model)
        {
            foreach (var tutorial in model.Tutorials)
            {
                tutorial.LearningEnabled = false;
            }
        }

        private IActionResult EditSave(EditCourseSectionViewModel model, int customisationId, int sectionId)
        {
            var section = new Section(
                model.SectionId,
                model.SectionName,
                model.Tutorials.Select(
                    t => new Tutorial(t.TutorialId, t.TutorialName, t.LearningEnabled, t.DiagnosticEnabled)
                )
            );

            sectionService.UpdateSectionTutorialsStatuses(section, customisationId);

            return RedirectToAction("Index", new { customisationId });
        }
    }
}
