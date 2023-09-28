namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using GDS.MultiPageFormData;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using GDS.MultiPageFormData.Enums;
    using DigitalLearningSolutions.Data.DataServices;

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
        private readonly IMultiPageFormService multiPageFormService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IPaginateService paginateService;
        private readonly ISectionService sectionService;
        private readonly ITutorialService tutorialService;
        private readonly IActivityService activityService;
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly ICourseTopicsDataService courseTopicsDataService;

        public CourseSetupController(
            ICourseService courseService,
            ITutorialService tutorialService,
            ISectionService sectionService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IPaginateService paginateService,
            IConfiguration config,
            IMultiPageFormService multiPageFormService,
            IActivityService activityService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICourseTopicsDataService courseTopicsDataService

        )
        {
            this.courseService = courseService;
            this.tutorialService = tutorialService;
            this.sectionService = sectionService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.paginateService = paginateService;
            this.config = config;
            this.multiPageFormService = multiPageFormService;
            this.activityService = activityService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.courseTopicsDataService = courseTopicsDataService;

        }

        [NoCaching]
        [Route("{page=1:int}")]
        public IActionResult Index(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int page = 1,
            int? itemsPerPage = 10
        )
        {
            searchString = searchString == null ? string.Empty : searchString.Trim();
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            sortDirection ??= GenericSortingHelper.Ascending;

            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                CourseFilterCookieName,
                CourseStatusFilterOptions.IsActive.FilterValue
            );

            var centreId = User.GetCentreIdKnownNotNull();
            var categoryId = User.GetAdminCategoryId();
            var courseCategoryName = this.activityService.GetCourseCategoryNameForActivityFilter(categoryId);
            var Categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId).Select(c => c.CategoryName);
            var Topics = courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId).Select(c => c.CourseTopic);

            int offSet = ((page - 1) * itemsPerPage) ?? 0;
            string isActive, categoryName, courseTopic, hasAdminFields;
            isActive = categoryName = courseTopic = hasAdminFields = "Any";
            bool? hideInLearnerPortal = null;

            if (!string.IsNullOrEmpty(existingFilterString))
            {
                var selectedFilters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();

                if (!string.IsNullOrEmpty(newFilterToAdd))
                {
                    var filterHeader = newFilterToAdd.Split(FilteringHelper.Separator)[0];
                    var dupfilters = selectedFilters.Where(x => x.Contains(filterHeader));
                    if (dupfilters.Count() > 1)
                    {
                        foreach (var filter in selectedFilters)
                        {
                            if (filter.Contains(filterHeader))
                            {
                                selectedFilters.Remove(filter);
                                existingFilterString = string.Join(FilteringHelper.FilterSeparator, selectedFilters);
                                break;
                            }
                        }
                    }
                }

                if (selectedFilters.Count > 0)
                {
                    foreach (var filter in selectedFilters)
                    {
                        var filterArr = filter.Split(FilteringHelper.Separator);
                        var filterValue = filterArr[2];
                        if (filterValue == FilteringHelper.EmptyValue) filterValue = "No option selected";

                        if (filter.Contains("CategoryName"))
                            categoryName = filterValue;

                        if (filter.Contains("CourseTopic"))
                            courseTopic = filterValue;

                        if (filter.Contains("Active"))
                            isActive = filterValue;

                        if (filter.Contains("NotActive"))
                            isActive = "false";

                        if (filter.Contains("HasAdminFields"))
                            hasAdminFields = filterValue;

                        if (filter.Contains("HideInLearnerPortal"))
                            hideInLearnerPortal = filterValue=="true" ? true:false;
                    }
                }
            }

            var (courses, resultCount) = courseService.GetCentreCourses(searchString ?? string.Empty, offSet, (int)itemsPerPage, sortBy, sortDirection, centreId, categoryId, false, hideInLearnerPortal,
                                                            isActive, categoryName, courseTopic, hasAdminFields);
            if (courses.Count() == 0 && resultCount > 0)
            {
                page = 1;
                offSet = 0;
                (courses, resultCount) = courseService.GetCentreCourses(searchString ?? string.Empty, offSet, (int)itemsPerPage, sortBy, sortDirection, centreId, categoryId, false, hideInLearnerPortal,
                                                            isActive, categoryName, courseTopic, hasAdminFields);
            }

            var availableFilters = CourseStatisticsViewModelFilterOptions
                .GetFilterOptions(categoryId.HasValue ? new string[] { } : Categories, Topics).ToList();

            var result = paginateService.Paginate(
                 courses,
                 resultCount,
                 new PaginationOptions(page, itemsPerPage),
                 new FilterOptions(existingFilterString, availableFilters),
                 searchString,
                 sortBy,
                 sortDirection
             );

            result.Page = page;
            TempData["Page"] = result.Page;

            var model = new CourseSetupViewModel(
                result,
                availableFilters,
                config,
                courseCategoryName
            );

            model.TotalPages = (int)(resultCount / itemsPerPage) + ((resultCount % itemsPerPage) > 0 ? 1 : 0);
            model.MatchingSearchResults = resultCount;
            Response.UpdateFilterCookie(CourseFilterCookieName, result.FilterString);

            return View(model);
        }


        [HttpGet("AddCourseNew")]
        public IActionResult AddCourseNew()
        {
            TempData.Clear();

            multiPageFormService.SetMultiPageFormData(
                new AddNewCentreCourseTempData(),
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            );
            return RedirectToAction("SelectCourse");
        }

        [HttpGet("AddCourse/SelectCourse")]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult SelectCourse(
            string? categoryFilterString = null,
            string? topicFilterString = null
        )
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            ).GetAwaiter().GetResult();

            var model = GetSelectCourseViewModel(
                categoryFilterString ?? data.CategoryFilter,
                topicFilterString ?? data.TopicFilter,
                data.Application?.ApplicationId
            );

            return View("AddNewCentreCourse/SelectCourse", model);
        }

        [Route("AddCourse/SelectCourseAllCourses")]
        public IActionResult SelectCourseAllCourses()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var adminCategoryFilter = User.GetAdminCategoryId();

            var applications = courseService
                .GetApplicationOptionsAlphabeticalListForCentre(centreId, adminCategoryFilter);
            var categories = courseService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
            var topics = courseService.GetTopicsForCentreAndCentrallyManagedCourses(centreId);

            var model = new SelectCourseAllCoursesViewModel(applications, categories, topics);
            return View("AddNewCentreCourse/SelectCourseAllCourses", model);
        }

        [HttpPost("AddCourse/SelectCourse")]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult SelectCourse(
            int? applicationId,
            string? categoryFilterString = null,
            string? topicFilterString = null
        )
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

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

            var centreId = User.GetCentreIdKnownNotNull();
            var categoryId = User.GetAdminCategoryId();

            var selectedApplication =
                courseService.GetApplicationOptionsAlphabeticalListForCentre(centreId, categoryId)
                    .Single(ap => ap.ApplicationId == applicationId);
            data.CategoryFilter = categoryFilterString;
            data.TopicFilter = topicFilterString;
            data!.SetApplicationAndResetModels(selectedApplication);

            multiPageFormService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData);

            return RedirectToAction("SetCourseDetails");
        }

        [HttpGet("AddCourse/SetCourseDetails")]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult SetCourseDetails()
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

            if (data.Application == null)
            {
                throw new Exception("Application should not be null at this point in the journey");
            }

            var model = data.CourseDetailsData != null
                ? new SetCourseDetailsViewModel(data.CourseDetailsData)
                : new SetCourseDetailsViewModel(data.Application);

            return View("AddNewCentreCourse/SetCourseDetails", model);
        }

        [HttpPost("AddCourse/SetCourseDetails")]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult SetCourseDetails(SetCourseDetailsViewModel model)
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();
            var centreId = User.GetCentreIdKnownNotNull();

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

            data!.CourseDetailsData = model.ToCourseDetailsTempData();
            multiPageFormService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData);

            return RedirectToAction("SetCourseOptions");
        }

        [HttpGet("AddCourse/SetCourseOptions")]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult SetCourseOptions()
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

            if (data.Application == null)
            {
                throw new Exception("Application should not be null at this point in the journey");
            }

            var model = data!.CourseOptionsData != null
                ? new EditCourseOptionsFormData(data!.CourseOptionsData)
                : new EditCourseOptionsFormData();
            model.SetUpCheckboxes(data.Application.DiagAssess);

            return View("AddNewCentreCourse/SetCourseOptions", model);
        }

        [HttpPost("AddCourse/SetCourseOptions")]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult SetCourseOptions(EditCourseOptionsFormData model)
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

            data!.CourseOptionsData = model.ToCourseOptionsTempData();
            multiPageFormService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData);

            return RedirectToAction("SetCourseContent");
        }

        [HttpGet("AddCourse/SetCourseContent")]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult SetCourseContent()
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

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
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult SetCourseContent(SetCourseContentViewModel model)
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

            if (data.Application == null)
            {
                throw new Exception("Application should not be null at this point in the journey");
            }

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

            data.CourseContentData = model.ToDataCourseContentTempData();
            multiPageFormService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData);

            return RedirectToAction(model.IncludeAllSections ? "Summary" : "SetSectionContent");
        }

        [HttpGet("AddCourse/SetSectionContent")]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult SetSectionContent(int sectionIndex)
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

            if (data.CourseContentData == null || data.Application == null)
            {
                throw new Exception(
                    "Application amd CourseContentData should not be null at this point in the journey"
                );
            }

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
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
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
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult Summary()
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

            var model = new SummaryViewModel(data!);

            return View("AddNewCentreCourse/Summary", model);
        }

        [HttpPost("AddCourse/Summary")]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult? CreateNewCentreCourse()
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

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

            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddNewCourse, TempData);

            transaction.Complete();

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
            string? topicFilterString,
            int? selectedApplicationId = null
        )
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var categoryIdFilter = User.GetAdminCategoryId()!;

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
                topicFilterString,
                selectedApplicationId
            );
        }

        private SetCourseContentViewModel GetSetCourseContentViewModel(AddNewCentreCourseTempData tempData)
        {
            if (tempData.Application == null)
            {
                throw new Exception("Application should not be null at this point in the journey");
            }

            var sections =
                sectionService.GetSectionsThatHaveTutorialsForApplication(tempData.Application.ApplicationId);
            return new SetCourseContentViewModel(sections);
        }

        private IEnumerable<SectionContentTempData> GetSectionContentDataWithAllContentEnabled(
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

                        return new SectionContentTempData(tutorials);
                    }
                );
        }

        private IActionResult SaveSectionAndRedirect(SetSectionContentViewModel model)
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

            if (data!.SectionContentData == null)
            {
                data.SectionContentData = new List<SectionContentTempData>();
            }

            data!.SectionContentData!.Add(
                new SectionContentTempData(
                    model.Tutorials != null
                        ? model.Tutorials.Select(GetCourseTutorialData)
                        : new List<CourseTutorialTempData>()
                )
            );
            multiPageFormService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData);

            return RedirectToNextSectionOrSummary(
                model.Index,
                new SetCourseContentViewModel(data.CourseContentData!)
            );
        }

        private static CourseTutorialTempData GetCourseTutorialData(CourseTutorialViewModel model)
        {
            return new CourseTutorialTempData(
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

        private Customisation GetCustomisationFromTempData(AddNewCentreCourseTempData tempData)
        {
            return new Customisation(
                User.GetCentreIdKnownNotNull(),
                tempData!.Application!.ApplicationId,
                tempData.CourseDetailsData!.CustomisationName ?? string.Empty,
                tempData.CourseDetailsData.Password,
                tempData.CourseOptionsData!.AllowSelfEnrolment,
                int.Parse(tempData.CourseDetailsData.TutCompletionThreshold!),
                tempData.CourseDetailsData.IsAssessed,
                int.Parse(tempData.CourseDetailsData.DiagCompletionThreshold!),
                tempData.CourseOptionsData.DiagnosticObjectiveSelection,
                tempData.CourseOptionsData.HideInLearningPortal,
                tempData.CourseDetailsData.NotificationEmails
            );
        }

        private static IEnumerable<CourseStatisticsWithAdminFieldResponseCounts> UpdateCoursesNotActiveStatus(IEnumerable<CourseStatisticsWithAdminFieldResponseCounts> courses)
        {
            var updatedCourses = courses.ToList();

            foreach (var course in updatedCourses)
            {
                if (course.Archived || course.Active == false)
                {
                  course.NotActive = true;
                }
                else
                {
                  course.NotActive = false;
                }
            }

            return updatedCourses;
        }

  }
}
