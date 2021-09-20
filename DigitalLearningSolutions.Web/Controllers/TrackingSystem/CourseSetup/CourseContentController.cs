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
        public const string SelectAllDiagnosticPrefix = "diagnostic-select-all-";
        public const string DeselectAllDiagnosticPrefix = "diagnostic-deselect-all-";
        public const string SelectAllLearningPrefix = "learning-select-all-";
        public const string DeselectAllLearningPrefix = "learning-deselect-all-";
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
        public IActionResult Edit(EditCourseContentViewModel model, int customisationId, string action)
        {
            return action == SaveAction
                ? EditSave(model, customisationId)
                : ProcessBulkSelect(model, customisationId, action);
        }

        private IActionResult ProcessBulkSelect(EditCourseContentViewModel model, int customisationId, string action)
        {
            var lastDashIndex = action.LastIndexOf('-') + 1;
            var actionPrefix = action.Substring(0, lastDashIndex);
            var sectionId = int.Parse(action.Substring(lastDashIndex));

            switch (actionPrefix)
            {
                case SelectAllDiagnosticPrefix:
                    SelectAllDiagnosticsInSection(sectionId, model);
                    break;
                case DeselectAllDiagnosticPrefix:
                    DeselectAllDiagnosticsInSection(sectionId, model);
                    break;
                case SelectAllLearningPrefix:
                    SelectAllLearningInSection(sectionId, model);
                    break;
                case DeselectAllLearningPrefix:
                    DeselectAllLearningInSection(sectionId, model);
                    break;
                default:
                    return new StatusCodeResult(500);
            }

            return View(model);
        }

        public void SelectAllDiagnosticsInSection(int sectionId, EditCourseContentViewModel model)
        {
           foreach (var tutorial in model.Sections.Single(s => s.SectionId == sectionId).Tutorials)
           {
               tutorial.DiagnosticEnabled = true;
           }
        }

        public void DeselectAllDiagnosticsInSection(int sectionId, EditCourseContentViewModel model)
        {
            foreach (var tutorial in model.Sections.Single(s => s.SectionId == sectionId).Tutorials)
            {
                tutorial.DiagnosticEnabled = false;
            }
        }

        public void SelectAllLearningInSection(int sectionId, EditCourseContentViewModel model)
        {
            foreach (var tutorial in model.Sections.Single(s => s.SectionId == sectionId).Tutorials)
            {
                tutorial.LearningEnabled = true;
            }
        }

        public void DeselectAllLearningInSection(int sectionId, EditCourseContentViewModel model)
        {
            foreach (var tutorial in model.Sections.Single(s => s.SectionId == sectionId).Tutorials)
            {
                tutorial.LearningEnabled = false;
            }
        }

        private IActionResult EditSave(EditCourseContentViewModel model, int customisationId)
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
