﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
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
        public const string SaveAction = "save";
        private readonly ICourseService courseService;
        private readonly ISectionService sectionService;
        private readonly ITutorialService tutorialService;

        public CourseContentController(
            ICourseService courseDataService,
            ISectionService sectionService,
            ITutorialService tutorialService
        )
        {
            this.courseService = courseDataService;
            this.sectionService = sectionService;
            this.tutorialService = tutorialService;
        }

        [HttpGet]
        public IActionResult Index(int customisationId)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var categoryId = User.GetAdminCategoryId();
            var courseDetails = courseService.GetCourseDetailsFilteredByCategory(
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
            var centreId = User.GetCentreIdKnownNotNull();
            var categoryId = User.GetAdminCategoryId();
            var courseDetails = courseService.GetCourseDetailsFilteredByCategory(
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
            if (action == SaveAction)
            {
                return EditSave(formData, customisationId);
            }

            var bulkSelectResult = EditCourseSectionHelper.ProcessBulkSelect(formData, action);
            if (bulkSelectResult != null)
            {
                return bulkSelectResult;
            }

            var viewModel = new EditCourseSectionViewModel(formData, customisationId);
            return View(viewModel);
        }

        private IActionResult EditSave(EditCourseSectionFormData formData, int customisationId)
        {
            var tutorials = formData.Tutorials.Select(
                t => new Tutorial(t.TutorialId, t.TutorialName, t.LearningEnabled, t.DiagnosticEnabled)
            );

            tutorialService.UpdateTutorialsStatuses(tutorials, customisationId);

            return RedirectToAction("Index", new { customisationId });
        }
    }
}
