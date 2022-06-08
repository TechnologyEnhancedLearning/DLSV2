namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse;
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
        private readonly ICourseService courseService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly ISectionService sectionService;
        private readonly ITutorialService tutorialService;
        private readonly IConfiguration config;

        public CourseSetupController(
            ICourseService courseService,
            ITutorialService tutorialService,
            ISectionService sectionService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IConfiguration config
        )
        {
            this.courseService = courseService;
            this.tutorialService = tutorialService;
            this.sectionService = sectionService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.config = config;
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

            var addNewCentreCourseData = new AddNewCentreCourseData();
            TempData.Set(addNewCentreCourseData);

            return RedirectToAction("SelectCourse");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet("AddCourse/SelectCourse")]
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

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost("AddCourse/SelectCourse")]
        public IActionResult SelectCourse(
            int? applicationId,
            string? categoryFilterString = null,
            string? topicFilterString = null
        )
        {
            var data = TempData.Peek<AddNewCentreCourseData>()!;

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
            TempData.Set(data);

            return RedirectToAction("SetCourseDetails");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet("AddCourse/SetCourseDetails")]
        public IActionResult SetCourseDetails()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var model = data?.SetCourseDetailsModel ?? new SetCourseDetailsViewModel(data!.Application!);

            return View("AddNewCentreCourse/SetCourseDetails", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost("AddCourse/SetCourseDetails")]
        public IActionResult SetCourseDetails(SetCourseDetailsViewModel model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();
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

            data!.SetCourseDetailsModel = model;
            TempData.Set(data);

            return RedirectToAction("SetCourseOptions");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet("AddCourse/SetCourseOptions")]
        public IActionResult SetCourseOptions()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var model = data!.SetCourseOptionsModel ?? new EditCourseOptionsFormData();
            model.SetUpCheckboxes(data.Application!.DiagAssess);

            return View("AddNewCentreCourse/SetCourseOptions", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost("AddCourse/SetCourseOptions")]
        public IActionResult SetCourseOptions(EditCourseOptionsFormData model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            data!.SetCourseOptionsModel = model;
            TempData.Set(data);

            return RedirectToAction("SetCourseContent");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet("AddCourse/SetCourseContent")]
        public IActionResult SetCourseContent()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            if (!sectionService.GetSectionsThatHaveTutorialsForApplication(data!.Application!.ApplicationId).Any())
            {
                return RedirectToAction("Summary");
            }

            var model = data!.SetCourseContentModel ?? GetSetCourseContentModel(data!);

            return View("AddNewCentreCourse/SetCourseContent", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost("AddCourse/SetCourseContent")]
        public IActionResult SetCourseContent(SetCourseContentViewModel model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            if (model.IncludeAllSections)
            {
                ModelState.ClearErrorsOnField(nameof(model.SelectedSectionIds));
                model.SelectAllSections();
                data!.SetSectionContentModels =
                    GetSectionModelsWithAllContentEnabled(model, data!.Application!.DiagAssess).ToList();
            }
            else
            {
                data!.SetSectionContentModels = null;
            }

            if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/SetCourseContent", model);
            }

            data.SetCourseContentModel = model;
            TempData.Set(data);

            return RedirectToAction(model.IncludeAllSections ? "Summary" : "SetSectionContent");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet("AddCourse/SetSectionContent")]
        public IActionResult SetSectionContent(int sectionIndex)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var section = data!.SetCourseContentModel!.GetSelectedSections().ElementAt(sectionIndex);
            var tutorials = tutorialService.GetTutorialsForSection(section.SectionId).ToList();

            if (!tutorials.Any())
            {
                return RedirectToNextSectionOrSummary(sectionIndex, data.SetCourseContentModel);
            }

            var showDiagnostic = data.Application!.DiagAssess;
            var model = new SetSectionContentViewModel(section, sectionIndex, showDiagnostic, tutorials);

            return View("AddNewCentreCourse/SetSectionContent", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost("AddCourse/SetSectionContent")]
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

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet("AddCourse/Summary")]
        public IActionResult Summary()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var model = new SummaryViewModel(data!);

            return View("AddNewCentreCourse/Summary", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost("AddCourse/Summary")]
        public IActionResult? CreateNewCentreCourse()
        {
            var data = TempData.Peek<AddNewCentreCourseData>()!;

            using var transaction = new TransactionScope();

            var customisation = GetCustomisationFromTempData(data!);

            var customisationId = courseService.CreateNewCentreCourse(customisation);

            if (data.SetSectionContentModels != null)
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

            TempData.Clear();
            TempData.Add("customisationId", customisationId);
            TempData.Add("applicationName", data.Application!.ApplicationName);
            TempData.Add("customisationName", data.SetCourseDetailsModel!.CustomisationName);

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

        private SetCourseContentViewModel GetSetCourseContentModel(AddNewCentreCourseData data)
        {
            var sections = sectionService.GetSectionsThatHaveTutorialsForApplication(data!.Application!.ApplicationId);
            return new SetCourseContentViewModel(sections);
        }

        private IEnumerable<SetSectionContentViewModel> GetSectionModelsWithAllContentEnabled(
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

                        return new SetSectionContentViewModel(s, index, diagAssess, tutorials);
                    }
                );
        }

        private IActionResult SaveSectionAndRedirect(SetSectionContentViewModel model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            if (data!.SetSectionContentModels == null)
            {
                data.SetSectionContentModels = new List<SetSectionContentViewModel>();
            }

            data!.SetSectionContentModels!.Add(model);
            TempData.Set(data);

            return RedirectToNextSectionOrSummary(model.Index, data.SetCourseContentModel!);
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
                data.SetCourseDetailsModel!.CustomisationName ?? string.Empty,
                data.SetCourseDetailsModel.Password,
                data.SetCourseOptionsModel!.AllowSelfEnrolment,
                int.Parse(data.SetCourseDetailsModel.TutCompletionThreshold!),
                data.SetCourseDetailsModel.IsAssessed,
                int.Parse(data.SetCourseDetailsModel.DiagCompletionThreshold!),
                data.SetCourseOptionsModel.DiagnosticObjectiveSelection,
                data.SetCourseOptionsModel.HideInLearningPortal,
                data.SetCourseDetailsModel.NotificationEmails
            );
        }
    }
}
