namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.CourseSetup))]
    [Route("/TrackingSystem/CourseSetup/{customisationId:int}/Content")]
    [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
    public class CourseContentController : Controller
    {
        public const string SelectAllDiagnosticAction = "diagnostic-select-all";
        public const string DeselectAllDiagnosticAction = "diagnostic-deselect-all";
        public const string SelectAllLearningAction = "learning-select-all";
        public const string DeselectAllLearningAction = "learning-deselect-all";
        public const string SaveAction = "save";

        private readonly ICourseDataService courseDataService;
        private readonly ISectionService sectionService;
        private readonly ITutorialService tutorialService;

        public CourseContentController(
            ICourseDataService courseDataService,
            ISectionService sectionService,
            ITutorialService tutorialService
        )
        {
            this.courseDataService = courseDataService;
            this.sectionService = sectionService;
            this.tutorialService = tutorialService;
        }

        [HttpGet]
        public IActionResult Index(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();
            var courseDetails = courseDataService.GetCourseDetailsFilteredByCategory(
                customisationId,
                centreId,
                categoryId
            )!;

            var courseSections = sectionService.GetSectionsAndTutorialsForCustomisation(
                customisationId,
                courseDetails!.ApplicationId
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
        [Route("EditSection/{sectionId:int}")]
        public IActionResult EditSection(int customisationId, int sectionId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();
            var courseDetails = courseDataService.GetCourseDetailsFilteredByCategory(
                customisationId,
                centreId,
                categoryId
            )!;
            var section = sectionService.GetSectionAndTutorialsBySectionIdForCustomisation(customisationId, sectionId);

            if (section == null)
            {
                return NotFound();
            }

            var model = new EditCourseSectionViewModel(
                customisationId,
                courseDetails!.CourseName,
                section,
                courseDetails.DiagAssess
            );

            return View(model);
        }

        [HttpPost]
        [Route("EditSection/{sectionId:int}")]
        public IActionResult EditSection(
            EditCourseSectionFormData formData,
            int customisationId,
            string action
        )
        {
            return action == SaveAction
                ? EditSave(formData, customisationId)
                : ProcessBulkSelect(formData, customisationId, action);
        }

        private IActionResult EditSave(EditCourseSectionFormData formData, int customisationId)
        {
            var tutorials = formData.Tutorials.Select(
                t => new Tutorial(t.TutorialId, t.TutorialName, t.LearningEnabled, t.DiagnosticEnabled)
            );

            tutorialService.UpdateTutorialsStatuses(tutorials, customisationId);

            return RedirectToAction("Index", new { customisationId });
        }

        private IActionResult ProcessBulkSelect(
            EditCourseSectionFormData formData,
            int customisationId,
            string action
        )
        {
            switch (action)
            {
                case SelectAllDiagnosticAction:
                    SelectAllDiagnostics(formData);
                    break;
                case DeselectAllDiagnosticAction:
                    DeselectAllDiagnostics(formData);
                    break;
                case SelectAllLearningAction:
                    SelectAllLearning(formData);
                    break;
                case DeselectAllLearningAction:
                    DeselectAllLearning(formData);
                    break;
                default:
                    return new StatusCodeResult(400);
            }

            var viewModel = new EditCourseSectionViewModel(formData, customisationId);
            return View(viewModel);
        }

        private static void SelectAllDiagnostics(EditCourseSectionFormData model)
        {
            foreach (var tutorial in model.Tutorials)
            {
                tutorial.DiagnosticEnabled = true;
            }
        }

        private static void DeselectAllDiagnostics(EditCourseSectionFormData model)
        {
            foreach (var tutorial in model.Tutorials)
            {
                tutorial.DiagnosticEnabled = false;
            }
        }

        private static void SelectAllLearning(EditCourseSectionFormData model)
        {
            foreach (var tutorial in model.Tutorials)
            {
                tutorial.LearningEnabled = true;
            }
        }

        private static void DeselectAllLearning(EditCourseSectionFormData model)
        {
            foreach (var tutorial in model.Tutorials)
            {
                tutorial.LearningEnabled = false;
            }
        }
    }
}
