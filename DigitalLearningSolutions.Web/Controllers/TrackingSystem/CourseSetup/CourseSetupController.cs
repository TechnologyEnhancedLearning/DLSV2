namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.CourseSetup))]
    [Route("/TrackingSystem/CourseSetup")]
    public class CourseSetupController : Controller
    {
        private const string CourseFilterCookieName = "CourseFilter";
        public const string SelectAllDiagnosticAction = "diagnostic-select-all";
        public const string DeselectAllDiagnosticAction = "diagnostic-deselect-all";
        public const string SelectAllLearningAction = "learning-select-all";
        public const string DeselectAllLearningAction = "learning-deselect-all";
        public const string SaveAction = "save";

        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly ICourseService courseService;
        private readonly ICourseTopicsDataService courseTopicsDataService;
        private readonly ISectionService sectionService;
        private readonly ITutorialService tutorialService;

        public CourseSetupController(
            ICourseService courseService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICourseTopicsDataService courseTopicsDataService,
            ITutorialService tutorialService,
            ISectionService sectionService
        )
        {
            this.courseService = courseService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.courseTopicsDataService = courseTopicsDataService;
            this.tutorialService = tutorialService;
            this.sectionService = sectionService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Ascending,
            string? filterBy = null,
            string? filterValue = null,
            int page = 1
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            filterBy = FilteringHelper.GetFilterBy(
                filterBy,
                filterValue,
                Request,
                CourseFilterCookieName,
                CourseStatusFilterOptions.IsActive.FilterValue
            );

            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();
            var centreCourses =
                courseService.GetCentreSpecificCourseStatistics(centreId, categoryId);
            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
                .Select(c => c.CategoryName);
            var topics = courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId).Select(c => c.CourseTopic);

            var model = new CourseSetupViewModel(
                centreCourses,
                categories,
                topics,
                searchString,
                sortBy,
                sortDirection,
                filterBy,
                page
            );

            Response.UpdateOrDeleteFilterCookie(CourseFilterCookieName, filterBy);

            return View(model);
        }

        [Route("AllCourseStatistics")]
        public IActionResult AllCourseStatistics()
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();
            var centreCourses =
                courseService.GetCentreSpecificCourseStatistics(centreId, categoryId);
            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
                .Select(c => c.CategoryName);
            var topics = courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId).Select(c => c.CourseTopic);

            var model = new AllCourseStatisticsViewModel(centreCourses, categories, topics);
            return View(model);
        }

        [HttpGet]
        [Route("AddCourseNew")]
        public IActionResult AddCourseNew()
        {
            TempData.Clear();

            var addNewCentreCourseData = new AddNewCentreCourseData();
            TempData.Set(addNewCentreCourseData);

            return RedirectToAction("SelectCourse");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SelectCourse")]
        public IActionResult SelectCourse(int? topicId = null)
        {
            var data = TempData.Peek<AddNewCentreCourseData>()!;

            var model = GetSelectCourseViewModel(data!.Application?.ApplicationId, topicId);

            return View("AddNewCentreCourse/SelectCourse", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SelectCourse")]
        public IActionResult SelectCourse(SelectCourseViewModel model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>()!;

            if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/SelectCourse", model);
            }

            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();
            var applicationOptions =
                courseService.GetApplicationOptionsAlphabeticalListForCentre(centreId, categoryId);

            var selectedApplication =
                applicationOptions.Single(ap => ap.ApplicationId == model.ApplicationId);

            data!.SetApplicationAndResetModels(selectedApplication);

            TempData.Set(data);

            return RedirectToAction("SetCourseDetails");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SetCourseDetails")]
        public IActionResult SetCourseDetails()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();
            var model = data?.SetCourseDetailsModel ?? new SetCourseDetailsViewModel(data!.Application!);

            return View("AddNewCentreCourse/SetCourseDetails", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SetCourseDetails")]
        public IActionResult SetCourseDetails(SetCourseDetailsViewModel model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();
            var centreId = User.GetCentreId();

            if (string.IsNullOrWhiteSpace(model.CustomisationName))
            {
                model.CustomisationName = string.Empty;
            }

            CourseDetailsValidator.ValidateCustomisationName(
                model,
                ModelState,
                courseService,
                centreId
            );
            CourseDetailsValidator.ValidatePassword(model, ModelState);
            CourseDetailsValidator.ValidateEmail(model, ModelState);
            CourseDetailsValidator.ValidateCompletionCriteria(model, ModelState);

            if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/SetCourseDetails", model);
            }

            data!.SetCourseDetailsModel = model;
            TempData.Set(data);

            return RedirectToAction("SetCourseOptions");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SetCourseOptions")]
        public IActionResult SetCourseOptions()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var model = data!.SetCourseOptionsModel ?? new EditCourseOptionsFormData();
            model.SetUpCheckboxes(data.Application!.DiagAssess);

            return View("AddNewCentreCourse/SetCourseOptions", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SetCourseOptions")]
        public IActionResult SetCourseOptions(EditCourseOptionsFormData model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            data!.SetCourseOptionsModel = model;
            TempData.Set(data);

            return RedirectToAction("SetCourseContent");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SetCourseContent")]
        public IActionResult SetCourseContent()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var model = GetSetCourseContentModel(data!);

            data.SetCourseContentModel = model;
            TempData.Set(data);

            return View("AddNewCentreCourse/SetCourseContent", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SetCourseContent")]
        public IActionResult SetCourseContent(SetCourseContentViewModel model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            if (model.IncludeAllSections)
            {
                ModelState.ClearErrorsOnField(nameof(model.SelectedSectionIds));
                model.SelectedSectionIds = model.AvailableSections.Select(s => s.Id);
            }

            if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/SetCourseContent", model);
            }

            data!.SetCourseContentModel = model;
            TempData.Set(data);

            return RedirectToAction(model.IncludeAllSections ? "Summary" : "SetSectionContent");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SetSectionContent")]
        public IActionResult SetSectionContent(int sectionIndex)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var section = data!.SetCourseContentModel!.GetSelectedSections().ElementAt(sectionIndex);
            var showDiagnostic = data.Application!.DiagAssess;
            var model = new SetSectionContentViewModel(section, sectionIndex, showDiagnostic);

            var tutorials = tutorialService.GetTutorialsForSection(section.Id);
            model.Tutorials = tutorials.Select(t => new CourseTutorialViewModel(t));

            return View("../AddNewCentreCourse/SetSectionContent", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SetSectionContent")]
        public IActionResult SetSectionContent(
            SetSectionContentViewModel model,
            string action
        )
        {
            return action == SaveAction
                ? SaveSectionTutorials(model)
                : ProcessBulkSelect(model, action);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/Summary")]
        public IActionResult Summary()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var model = new SummaryViewModel(data!);

            return View("AddNewCentreCourse/Summary", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/Summary")]
        public IActionResult Summary(SummaryViewModel model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>()!;

            try
            {
                var centreId = User.GetCentreId();
                var customisationId = courseService.CreateNewCentreCourse(
                    centreId,
                    data!.Application!.ApplicationId,
                    data.SetCourseDetailsModel!.CustomisationName ?? string.Empty,
                    data.SetCourseDetailsModel.Password,
                    data.SetCourseOptionsModel!.AllowSelfEnrolment,
                    int.Parse(data.SetCourseDetailsModel.TutCompletionThreshold!),
                    data.SetCourseDetailsModel.PostLearningAssessment,
                    int.Parse(data.SetCourseDetailsModel.DiagCompletionThreshold!),
                    data.SetCourseOptionsModel.DiagnosticObjectiveSelection,
                    data.SetCourseOptionsModel.HideInLearningPortal,
                    data.SetCourseDetailsModel.NotificationEmails
                );

                var tutorials = data.GetTutorialsFromSections()
                    .Select(
                        tm => new Tutorial(
                            tm.TutorialId,
                            tm.TutorialName,
                            tm.LearningEnabled,
                            tm.DiagnosticEnabled
                        )
                    );
                tutorialService.UpdateTutorialsStatuses(tutorials, customisationId);

                TempData.Clear();
                TempData.Add("customisationId", customisationId);
                TempData.Add("applicationName", data.SetCourseDetailsModel!.ApplicationName);
                TempData.Add("customisationName", data.SetCourseDetailsModel.CustomisationName);

                return RedirectToAction("Confirmation");
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("AddCourse/Confirmation")]
        public IActionResult Confirmation()
        {
            var customisationId = (int)TempData.Peek("customisationId");
            var applicationName = (string)TempData.Peek("applicationName");
            var customisationName = (string)TempData.Peek("customisationName");
            TempData.Clear();

            var model = new ConfirmationViewModel(customisationId, applicationName, customisationName);

            return View("AddNewCentreCourse/Confirmation", model);
        }

        private SelectCourseViewModel GetSelectCourseViewModel(int? selectedCourseId, int? topicId)
        {
            var availableTopics = GetTopicOptionsSelectListOrNull(topicId);
            var availableCourses = GetApplicationOptionsSelectList(selectedCourseId, topicId);
            return new SelectCourseViewModel(availableCourses, availableTopics, topicId);
        }

        private IEnumerable<SelectListItem> GetApplicationOptionsSelectList(int? selectedId, int? topicId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter()!;
            var categoryIdFilter = categoryId == 0 ? null : categoryId;

            var orderedApplications = courseService
                .GetApplicationOptionsAlphabeticalListForCentre(centreId, categoryIdFilter)
                .ToList();
            var filteredApplications = orderedApplications.Where(c => c.CourseTopicId == topicId || topicId == null);
            var applicationOptions = filteredApplications.Select(a => (a.ApplicationId, a.ApplicationName));

            return SelectListHelper.MapOptionsToSelectListItems(applicationOptions, selectedId);
        }

        private IEnumerable<SelectListItem>? GetTopicOptionsSelectListOrNull(int? selectedId)
        {
            if (User.GetAdminCourseCategoryFilter() != null)
            {
                return null;
            }

            var centreId = User.GetCentreId();
            var topics = courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId)
                .Where(c => c.Active)
                .Select(c => (c.CourseTopicID, c.CourseTopic));
            return SelectListHelper.MapOptionsToSelectListItems(topics, selectedId);
        }

        private SetCourseContentViewModel GetSetCourseContentModel(AddNewCentreCourseData data)
        {
            if (data.SetCourseContentModel != null)
            {
                return data.SetCourseContentModel;
            }

            // TODO: Ask Steve if I should only show sections with tutorials here / only courses with tutorials with sections in the dropdown
            var sections =
                sectionService.GetSectionsForApplication(data!.Application!.ApplicationId);
            var sectionsWithTutorials =
                sections.Where(s => tutorialService.GetTutorialsForSection(s.SectionId).Count() != 0);
            var sectionModels = sectionsWithTutorials.Select(section => new SelectSectionViewModel(section, false))
                .ToList();

            return new SetCourseContentViewModel(sectionModels);
        }

        private IActionResult SaveSectionTutorials(SetSectionContentViewModel model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            data!.SetSectionContentModels.Add(model);
            TempData.Set(data);

            var sectionIndex = model.Index + 1;

            return model.Index == data.SetCourseContentModel!.GetSelectedSections().Count() - 1
                ? RedirectToAction("Summary")
                : RedirectToAction("SetSectionContent", new { sectionIndex });
        }

        // TODO: Can this be commonized with CourseContentController?
        private IActionResult ProcessBulkSelect(
            SetSectionContentViewModel model,
            string action
        )
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
                    return new StatusCodeResult(400);
            }

            return View("../AddNewCentreCourse/SetSectionContent", model);
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
