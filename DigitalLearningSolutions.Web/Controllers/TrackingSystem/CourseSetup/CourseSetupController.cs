namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.CourseSetup))]
    [Route("/TrackingSystem/CourseSetup")]
    public class CourseSetupController : Controller
    {
        public const string SaveAction = "save";
        private const string CourseFilterCookieName = "CourseFilter";
        private readonly IConfiguration config;
        private readonly ICourseService courseService;
        private readonly IMultiPageFormDataService multiPageFormDataService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly ISectionService sectionService;
        private readonly ITutorialService tutorialService;

        public CourseSetupController(
            ICourseService courseService,
            ITutorialService tutorialService,
            ISectionService sectionService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IConfiguration config,
            IMultiPageFormDataService multiPageFormDataService
        )
        {
            this.courseService = courseService;
            this.tutorialService = tutorialService;
            this.sectionService = sectionService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.config = config;
            this.multiPageFormDataService = multiPageFormDataService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int page = 1,
            int? itemsPerPage = null
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                CourseFilterCookieName,
                CourseStatusFilterOptions.IsActive.FilterValue
            );

            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();

            var details = courseService.GetCentreCourseDetails(centreId, categoryId);

            var availableFilters = CourseStatisticsViewModelFilterOptions
                .GetFilterOptions(categoryId.HasValue ? new string[] { } : details.Categories, details.Topics).ToList();

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                new SortOptions(sortBy, sortDirection),
                new FilterOptions(
                    existingFilterString,
                    availableFilters,
                    CourseStatusFilterOptions.IsActive.FilterValue
                ),
                new PaginationOptions(page, itemsPerPage)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                details.Courses,
                searchSortPaginationOptions
            );

            var model = new CourseSetupViewModel(
                result,
                availableFilters,
                config
            );

            Response.UpdateFilterCookie(CourseFilterCookieName, result.FilterString);

            return View(model);
        }

        [Route("AllCourseStatistics")]
        public IActionResult AllCourseStatistics()
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();
            var details = courseService.GetCentreCourseDetails(centreId, categoryId);

            var model = new AllCourseStatisticsViewModel(details, config);

            return View(model);
        }

        [HttpGet("AddCourseNew")]
        public IActionResult AddCourseNew()
        {
            TempData.Clear();

            multiPageFormDataService.SetMultiPageFormData(
                new AddNewCentreCourseData(),
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            );
            return RedirectToAction("SelectCourse");
        }

        [HttpGet("AddCourse/SelectCourse")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddNewCentreCourseData>))]
        public IActionResult SelectCourse(
            string? categoryFilterString = null,
            string? topicFilterString = null
        )
        {
            var model = GetSelectCourseViewModel(categoryFilterString, topicFilterString);

            return View("AddNewCentreCourse/SelectCourse", model);
        }

        [Route("AddCourse/SelectCourseAllCourses")]
        public IActionResult SelectCourseAllCourses()
        {
            var centreId = User.GetCentreId();
            var adminCategoryFilter = User.GetAdminCourseCategoryFilter();

            var applications = courseService
                .GetApplicationOptionsAlphabeticalListForCentre(centreId, adminCategoryFilter);
            var categories = courseService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
            var topics = courseService.GetTopicsForCentreAndCentrallyManagedCourses(centreId);

            var model = new SelectCourseAllCoursesViewModel(applications, categories, topics);
            return View("AddNewCentreCourse/SelectCourseAllCourses", model);
        }

        [HttpPost("AddCourse/SelectCourse")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddNewCentreCourseData>))]
        public IActionResult SelectCourse(
            int? applicationId,
            string? categoryFilterString = null,
            string? topicFilterString = null
        )
        {
            var data = multiPageFormDataService.GetMultiPageFormData<AddNewCentreCourseData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            );

            if (applicationId == null)
            {
                ModelState.AddModelError("ApplicationId", "Select a course");
                return View(
                    "AddNewCentreCourse/SelectCourse",
                    GetSelectCourseViewModel(
                        categoryFilterString,
                        topicFilterString
                    )
                );
            }

            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();

            var selectedApplication =
                courseService.GetApplicationOptionsAlphabeticalListForCentre(centreId, categoryId)
                    .Single(ap => ap.ApplicationId == applicationId);

            data!.SetApplicationAndResetModels(selectedApplication);

            multiPageFormDataService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData);

            return RedirectToAction("SetCourseDetails");
        }

        [HttpGet("AddCourse/SetCourseDetails")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddNewCentreCourseData>))]
        public IActionResult SetCourseDetails()
        {
            var data = multiPageFormDataService.GetMultiPageFormData<AddNewCentreCourseData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            );

            var model = data.CourseDetailsData != null
                ? new SetCourseDetailsViewModel(data.CourseDetailsData)
                : new SetCourseDetailsViewModel(data!.Application!);

            return View("AddNewCentreCourse/SetCourseDetails", model);
        }

        [HttpPost("AddCourse/SetCourseDetails")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddNewCentreCourseData>))]
        public IActionResult SetCourseDetails(SetCourseDetailsViewModel model)
        {
            var data = multiPageFormDataService.GetMultiPageFormData<AddNewCentreCourseData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            );
            var centreId = User.GetCentreId();

            CourseDetailsValidator.ValidateCustomisationName(
                model,
                ModelState,
                courseService,
                centreId
            );
            CourseDetailsValidator.ResetValueAndClearErrorsOnPasswordIfUnselected(model, ModelState);
            CourseDetailsValidator.ResetValueAndClearErrorsOnEmailIfUnselected(model, ModelState);
            CourseDetailsValidator.ResetValueAndClearErrorsOnOtherCompletionCriteriaIfUnselected(model, ModelState);

            if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/SetCourseDetails", model);
            }

            data!.CourseDetailsData = new CourseDetailsData(
                model.ApplicationId,
                model.ApplicationName,
                model.CustomisationName,
                model.PasswordProtected,
                model.Password,
                model.ReceiveNotificationEmails,
                model.NotificationEmails,
                model.PostLearningAssessment,
                model.IsAssessed,
                model.DiagAssess,
                model.TutCompletionThreshold,
                model.DiagCompletionThreshold
            );
            multiPageFormDataService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData);

            return RedirectToAction("SetCourseOptions");
        }

        [HttpGet("AddCourse/SetCourseOptions")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddNewCentreCourseData>))]
        public IActionResult SetCourseOptions()
        {
            var data = multiPageFormDataService.GetMultiPageFormData<AddNewCentreCourseData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            );

            var model = data!.CourseOptionsData != null
                ? new EditCourseOptionsFormData(data!.CourseOptionsData)
                : new EditCourseOptionsFormData();
            model.SetUpCheckboxes(data.Application!.DiagAssess);

            return View("AddNewCentreCourse/SetCourseOptions", model);
        }

        [HttpPost("AddCourse/SetCourseOptions")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddNewCentreCourseData>))]
        public IActionResult SetCourseOptions(EditCourseOptionsFormData model)
        {
            var data = multiPageFormDataService.GetMultiPageFormData<AddNewCentreCourseData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            );

            data!.CourseOptionsData = new CourseOptionsData(
                model.Active,
                model.AllowSelfEnrolment,
                model.DiagnosticObjectiveSelection,
                model.HideInLearningPortal
            );
            multiPageFormDataService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData);

            return RedirectToAction("SetCourseContent");
        }

        [HttpGet("AddCourse/SetCourseContent")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddNewCentreCourseData>))]
        public IActionResult SetCourseContent()
        {
            var data = multiPageFormDataService.GetMultiPageFormData<AddNewCentreCourseData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            );

            if (!sectionService.GetSectionsThatHaveTutorialsForApplication(data!.Application!.ApplicationId).Any())
            {
                return RedirectToAction("Summary");
            }

            var model = data!.CourseContentData != null
                ? new SetCourseContentViewModel(data.CourseContentData)
                : GetSetCourseContentViewModel(data!);

            return View("AddNewCentreCourse/SetCourseContent", model);
        }

        [HttpPost("AddCourse/SetCourseContent")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddNewCentreCourseData>))]
        public IActionResult SetCourseContent(SetCourseContentViewModel model)
        {
            var data = multiPageFormDataService.GetMultiPageFormData<AddNewCentreCourseData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            );

            if (model.IncludeAllSections)
            {
                ModelState.ClearErrorsOnField(nameof(model.SelectedSectionIds));
                model.SelectAllSections();
                data!.SectionContentData =
                    GetSectionContentDataWithAllContentEnabled(model, data!.Application!.DiagAssess).ToList();
            }
            else
            {
                data!.SectionContentData = null;
            }

            if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/SetCourseContent", model);
            }

            data.CourseContentData = new CourseContentData(
                model.AvailableSections,
                model.IncludeAllSections,
                model.SelectedSectionIds
            );
            multiPageFormDataService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData);

            return RedirectToAction(model.IncludeAllSections ? "Summary" : "SetSectionContent");
        }

        [HttpGet("AddCourse/SetSectionContent")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddNewCentreCourseData>))]
        public IActionResult SetSectionContent(int sectionIndex)
        {
            var data = multiPageFormDataService.GetMultiPageFormData<AddNewCentreCourseData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            );

            var section = data!.CourseContentData!.GetSelectedSections().ElementAt(sectionIndex);
            var tutorials = tutorialService.GetTutorialsForSection(section.SectionId).ToList();

            if (!tutorials.Any())
            {
                return RedirectToNextSectionOrSummary(
                    sectionIndex,
                    new SetCourseContentViewModel(data.CourseContentData)
                );
            }

            var showDiagnostic = data.Application!.DiagAssess;
            var model = new SetSectionContentViewModel(section, sectionIndex, showDiagnostic, tutorials);

            return View("AddNewCentreCourse/SetSectionContent", model);
        }

        [HttpPost("AddCourse/SetSectionContent")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddNewCentreCourseData>))]
        public IActionResult SetSectionContent(
            SetSectionContentViewModel model,
            string action
        )
        {
            if (action == SaveAction)
            {
                return SaveSectionAndRedirect(model);
            }

            var bulkSelectResult = EditCourseSectionHelper.ProcessBulkSelect(model, action);
            return bulkSelectResult ?? View("AddNewCentreCourse/SetSectionContent", model);
        }

        [HttpGet("AddCourse/Summary")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddNewCentreCourseData>))]
        public IActionResult Summary()
        {
            var data = multiPageFormDataService.GetMultiPageFormData<AddNewCentreCourseData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            );

            var model = new SummaryViewModel(data!);

            return View("AddNewCentreCourse/Summary", model);
        }

        [HttpPost("AddCourse/Summary")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddNewCentreCourseData>))]
        public IActionResult? CreateNewCentreCourse()
        {
            var data = multiPageFormDataService.GetMultiPageFormData<AddNewCentreCourseData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            );

            using var transaction = new TransactionScope();

            var customisation = GetCustomisationFromTempData(data!);

            var customisationId = courseService.CreateNewCentreCourse(customisation);

            if (data.SectionContentData != null)
            {
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
            }

            transaction.Complete();

            multiPageFormDataService.ClearMultiPageFormData(MultiPageFormDataFeature.AddNewCourse, TempData);

            TempData.Clear();
            TempData.Add("customisationId", customisationId);
            TempData.Add("applicationName", data.Application!.ApplicationName);
            TempData.Add("customisationName", data.CourseDetailsData!.CustomisationName);

            return RedirectToAction("Confirmation");
        }

        [HttpGet("AddCourse/Confirmation")]
        public IActionResult Confirmation()
        {
            var customisationId = (int)TempData.Peek("customisationId");
            var applicationName = (string)TempData.Peek("applicationName");
            var customisationName = (string)TempData.Peek("customisationName");

            var model = new ConfirmationViewModel(customisationId, applicationName, customisationName);

            return View("AddNewCentreCourse/Confirmation", model);
        }

        private SelectCourseViewModel GetSelectCourseViewModel(
            string? categoryFilterString,
            string? topicFilterString
        )
        {
            var centreId = User.GetCentreId();
            var categoryIdFilter = User.GetAdminCourseCategoryFilter()!;

            var applications = courseService
                .GetApplicationOptionsAlphabeticalListForCentre(centreId, categoryIdFilter).ToList();
            var categories = courseService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
            var topics = courseService.GetTopicsForCentreAndCentrallyManagedCourses(centreId);

            var availableFilters = (categoryIdFilter == null
                ? SelectCourseViewModelFilterOptions.GetAllCategoriesFilters(
                    categories,
                    topics,
                    categoryFilterString,
                    topicFilterString
                )
                : SelectCourseViewModelFilterOptions.GetSingleCategoryFilters(
                    applications,
                    categoryFilterString,
                    topicFilterString
                )).ToList();

            var currentFilterString =
                FilteringHelper.GetCategoryAndTopicFilterString(categoryFilterString, topicFilterString);

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions(nameof(ApplicationDetails.ApplicationName), GenericSortingHelper.Ascending),
                new FilterOptions(currentFilterString, availableFilters),
                null
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                applications,
                searchSortPaginationOptions
            );

            return new SelectCourseViewModel(
                result,
                availableFilters,
                categoryFilterString,
                topicFilterString
            );
        }

        private SetCourseContentViewModel GetSetCourseContentViewModel(AddNewCentreCourseData data)
        {
            var sections = sectionService.GetSectionsThatHaveTutorialsForApplication(data!.Application!.ApplicationId);
            return new SetCourseContentViewModel(sections);
        }

        private IEnumerable<SectionContentData> GetSectionContentDataWithAllContentEnabled(
            SetCourseContentViewModel model,
            bool diagAssess
        )
        {
            return model.GetSelectedSections()
                .Select(
                    (s, index) =>
                    {
                        var tutorials = tutorialService.GetTutorialsForSection(s.SectionId).ToList();
                        foreach (var tutorial in tutorials)
                        {
                            tutorial.Status = true;
                            tutorial.DiagStatus = diagAssess;
                        }

                        return new SectionContentData(s, index, diagAssess, tutorials);
                    }
                );
        }

        private IActionResult SaveSectionAndRedirect(SetSectionContentViewModel model)
        {
            var data = multiPageFormDataService.GetMultiPageFormData<AddNewCentreCourseData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            );

            if (data!.SectionContentData == null)
            {
                data.SectionContentData = new List<SectionContentData>();
            }

            data!.SectionContentData!.Add(
                new SectionContentData(
                    model.SectionName,
                    model.ShowDiagnostic,
                    model.Tutorials != null
                        ? model.Tutorials.Select(GetCourseTutorialData)
                        : new List<CourseTutorialData>(),
                    model.Index
                )
            );
            multiPageFormDataService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData);

            return RedirectToNextSectionOrSummary(
                model.Index,
                new SetCourseContentViewModel(data.CourseContentData!)
            );
        }

        private static CourseTutorialData GetCourseTutorialData(CourseTutorialViewModel model)
        {
            return new CourseTutorialData(
                model.TutorialId,
                model.TutorialName,
                model.LearningEnabled,
                model.DiagnosticEnabled
            );
        }

        private IActionResult RedirectToNextSectionOrSummary(
            int index,
            SetCourseContentViewModel setCourseContentViewModel
        )
        {
            var nextSectionIndex = index + 1;

            return nextSectionIndex == setCourseContentViewModel.GetSelectedSections().Count()
                ? RedirectToAction("Summary")
                : RedirectToAction("SetSectionContent", new { sectionIndex = nextSectionIndex });
        }

        private Customisation GetCustomisationFromTempData(AddNewCentreCourseData data)
        {
            return new Customisation(
                User.GetCentreId(),
                data!.Application!.ApplicationId,
                data.CourseDetailsData!.CustomisationName ?? string.Empty,
                data.CourseDetailsData.Password,
                data.CourseOptionsData!.AllowSelfEnrolment,
                int.Parse(data.CourseDetailsData.TutCompletionThreshold!),
                data.CourseDetailsData.IsAssessed,
                int.Parse(data.CourseDetailsData.DiagCompletionThreshold!),
                data.CourseOptionsData.DiagnosticObjectiveSelection,
                data.CourseOptionsData.HideInLearningPortal,
                data.CourseDetailsData.NotificationEmails
            );
        }
    }
}
