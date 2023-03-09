namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    //using DigitalLearningSolutions.Data.Enums;
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
    using GDS.MultiPageFormData.Enums;
    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(DigitalLearningSolutions.Data.Enums.NavMenuTab.CourseSetup))]
    [Route("/TrackingSystem/CourseSetup")]
    public class CourseSetupController : Controller
    {
        public const string SaveAction = "save";
        private const string CourseFilterCookieName = "CourseFilter";
        private readonly IConfiguration config;
        private readonly ICourseService courseService;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly ISectionService sectionService;
        private readonly ITutorialService tutorialService;

        public CourseSetupController(
            ICourseService courseService,
            ITutorialService tutorialService,
            ISectionService sectionService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IConfiguration config,
            IMultiPageFormService multiPageFormService
        )
        {
            this.courseService = courseService;
            this.tutorialService = tutorialService;
            this.sectionService = sectionService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.config = config;
            this.multiPageFormService = multiPageFormService;
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

            var centreId = User.GetCentreIdKnownNotNull();
            var categoryId = User.GetAdminCategoryId();

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
            var centreId = User.GetCentreIdKnownNotNull();
            var categoryId = User.GetAdminCategoryId();
            var details = courseService.GetCentreCourseDetails(centreId, categoryId);

            var model = new AllCourseStatisticsViewModel(details, config);

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
            ).GetAwaiter().GetResult();
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
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            ).GetAwaiter().GetResult();

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
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            ).GetAwaiter().GetResult();

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
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            ).GetAwaiter().GetResult();
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
            multiPageFormService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

            return RedirectToAction("SetCourseOptions");
        }

        [HttpGet("AddCourse/SetCourseOptions")]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult SetCourseOptions()
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            ).GetAwaiter().GetResult();

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
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            ).GetAwaiter().GetResult();

            data!.CourseOptionsData = model.ToCourseOptionsTempData();
            multiPageFormService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

            return RedirectToAction("SetCourseContent");
        }

        [HttpGet("AddCourse/SetCourseContent")]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult SetCourseContent()
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            ).GetAwaiter().GetResult();

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
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            ).GetAwaiter().GetResult();

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
            multiPageFormService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

            return RedirectToAction(model.IncludeAllSections ? "Summary" : "SetSectionContent");
        }

        [HttpGet("AddCourse/SetSectionContent")]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewCourse) }
        )]
        public IActionResult SetSectionContent(int sectionIndex)
        {
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            ).GetAwaiter().GetResult();

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
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            ).GetAwaiter().GetResult();

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
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            ).GetAwaiter().GetResult();

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

            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

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
            var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
                MultiPageFormDataFeature.AddNewCourse,
                TempData
            ).GetAwaiter().GetResult();

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
            multiPageFormService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData).GetAwaiter().GetResult();

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
    }
}
