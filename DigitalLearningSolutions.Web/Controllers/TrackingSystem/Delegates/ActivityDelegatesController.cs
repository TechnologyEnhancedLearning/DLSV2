namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/ActivityDelegates")]
    public class ActivityDelegatesController : Controller
    {
        private string courseDelegatesFilterCookieName = "CourseDelegatesFilter";
        private string selfAssessmentDelegatesFilterCookieName = "SelfAssessmentDelegatesFilter";
        private readonly ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService;
        private readonly ICourseDelegatesService courseDelegatesService;
        private readonly IPaginateService paginateService;
        private readonly IConfiguration configuration;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly ICourseService courseService;

        public ActivityDelegatesController(
            ICourseDelegatesService courseDelegatesService,
            ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService,
            IPaginateService paginateService,
            IConfiguration configuration,
            ISelfAssessmentService selfAssessmentService,
            ICourseService courseService
        )
        {
            this.courseDelegatesService = courseDelegatesService;
            this.courseDelegatesDownloadFileService = courseDelegatesDownloadFileService;
            this.paginateService = paginateService;
            this.configuration = configuration;
            this.selfAssessmentService = selfAssessmentService;
            this.courseService = courseService;
        }

        [NoCaching]
        [Route("{page:int=1}")]
        public IActionResult Index(
            int? customisationId = null,
            int? selfAssessmentId = null,
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
            if ((!customisationId.HasValue || customisationId == 0)
                && (!selfAssessmentId.HasValue || selfAssessmentId == 0))
            {
                return new NotFoundResult();
            }
            searchString = searchString == null ? string.Empty : searchString.Trim();
            var isCourseDelegate = customisationId != null;

            var filterCookieName = isCourseDelegate ? courseDelegatesFilterCookieName : selfAssessmentDelegatesFilterCookieName;

            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            sortDirection ??= GenericSortingHelper.Ascending;

            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                filterCookieName,
                CourseDelegateAccountStatusFilterOptions.Active.FilterValue
            );

            if (existingFilterString != null && existingFilterString.Contains("Answer"))
            {
                var filtersWithoutPromo = existingFilterString!.Split(FilteringHelper.FilterSeparator).Where(filter => !filter.Contains("Answer")).ToList();
                existingFilterString = filtersWithoutPromo.Any() ? string.Join(FilteringHelper.FilterSeparator, filtersWithoutPromo) : null;
            }
            int offSet = ((page - 1) * itemsPerPage) ?? 0;

            var centreId = User.GetCentreIdKnownNotNull();
            var adminCategoryId = User.GetAdminCategoryId();

            bool? isDelegateActive, isProgressLocked, removed, hasCompleted;
            isDelegateActive = isProgressLocked = removed = hasCompleted = null;

            string? answer1, answer2, answer3;
            answer1 = answer2 = answer3 = null;

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
                        dynamic filterValue = filterArr[2];
                        switch (filterValue)
                        {
                            case FilteringHelper.EmptyValue:
                                filterValue = "No option selected"; break;
                            case "true":
                                filterValue = true; break;
                            case "false":
                                filterValue = false; break;
                        }

                        if (filter.Contains("AccountStatus"))
                            isDelegateActive = filterValue;

                        if (filter.Contains("ProgressLocked"))
                            isProgressLocked = filterValue;

                        if (filter.Contains("ProgressRemoved") || filter.Contains("Removed"))
                            removed = filterValue;

                        if (filter.Contains("CompletionStatus"))
                            hasCompleted = filterValue;

                        if (filter.Contains("Answer1"))
                            answer1 = filterValue;

                        if (filter.Contains("Answer2"))
                            answer2 = filterValue;

                        if (filter.Contains("Answer3"))
                            answer3 = filterValue;
                    }
                }
            }

            try
            {
                var courseDelegatesData = new CourseDelegatesData();
                var selfAssessmentDelegatesData = new SelfAssessmentDelegatesData();
                var resultCount = 0;
                if (isCourseDelegate)
                {
                    (courseDelegatesData, resultCount) = courseDelegatesService.GetCoursesAndCourseDelegatesPerPageForCentre(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection,
                    customisationId, centreId, adminCategoryId, isDelegateActive, isProgressLocked, removed, hasCompleted, answer1, answer2, answer3);

                    if (courseDelegatesData?.Delegates.Count() == 0 && resultCount > 0)
                    {
                        page = 1;
                        offSet = 0;
                        (courseDelegatesData, resultCount) = courseDelegatesService.GetCoursesAndCourseDelegatesPerPageForCentre(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection,
                            customisationId, centreId, adminCategoryId, isDelegateActive, isProgressLocked, removed, hasCompleted, answer1, answer2, answer3);
                    }
                }
                else
                {
                    (selfAssessmentDelegatesData, resultCount) = selfAssessmentService.GetSelfAssessmentDelegatesPerPage(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection,
                        selfAssessmentId, centreId, isDelegateActive, removed);

                    if (selfAssessmentDelegatesData?.Delegates?.Count() == 0 && resultCount > 0)
                    {
                        page = 1;
                        offSet = 0;
                        (selfAssessmentDelegatesData, resultCount) = selfAssessmentService.GetSelfAssessmentDelegatesPerPage(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection,
                            selfAssessmentId, centreId, isDelegateActive, removed);
                    }
                }

                var availableFilters = isCourseDelegate
                   ? CourseDelegateViewModelFilterOptions.GetAllCourseDelegatesFilterViewModels(courseDelegatesData.CourseAdminFields)
                   : SelfAssessmentDelegateViewModelFilterOptions.GetAllSelfAssessmentDelegatesFilterViewModels();

                var activityName = isCourseDelegate
                    ? courseService.GetCourseNameAndApplication((int)customisationId).CourseName
                    : selfAssessmentService.GetSelfAssessmentNameById((int)selfAssessmentId);

                if (isCourseDelegate)
                {
                    var result = paginateService.Paginate(
                    courseDelegatesData.Delegates,
                    resultCount,
                    new PaginationOptions(page, itemsPerPage),
                    new FilterOptions(existingFilterString, availableFilters, CourseDelegateAccountStatusFilterOptions.Active.FilterValue),
                    searchString,
                    sortBy,
                    sortDirection);

                    result.Page = page;
                    TempData["Page"] = result.Page;
                    Response.UpdateFilterCookie(filterCookieName, result.FilterString);
                    var model = new ActivityDelegatesViewModel(courseDelegatesData, result, availableFilters, "customisationId", activityName, true);
                    return View(model);
                }
                else
                {
                    var result = paginateService.Paginate(
                    selfAssessmentDelegatesData.Delegates,
                    resultCount,
                    new PaginationOptions(page, itemsPerPage),
                    new FilterOptions(existingFilterString, availableFilters, CourseDelegateAccountStatusFilterOptions.Active.FilterValue),
                    searchString,
                    sortBy,
                    sortDirection);

                    result.Page = page;
                    TempData["Page"] = result.Page;
                    Response.UpdateFilterCookie(filterCookieName, result.FilterString);
                    var model = new ActivityDelegatesViewModel(selfAssessmentDelegatesData, result, availableFilters, "selfAssessmentId", selfAssessmentId, activityName, false);
                    return View(model);
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [ServiceFilter(typeof(VerifyAdminUserCanViewCourse))]
        [Route("DownloadCurrent/{customisationId:int}")]
        public IActionResult DownloadCurrent(
            int customisationId,
             string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null
        )
        {
            var centreId = User.GetCentreIdKnownNotNull();

            bool? isDelegateActive, isProgressLocked, removed, hasCompleted;
            isDelegateActive = isProgressLocked = removed = hasCompleted = null;

            string? answer1, answer2, answer3;
            answer1 = answer2 = answer3 = null;

            if (!string.IsNullOrEmpty(existingFilterString))
            {
                var selectedFilters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();

                if (selectedFilters.Count > 0)
                {
                    foreach (var filter in selectedFilters)
                    {
                        var filterArr = filter.Split(FilteringHelper.Separator);
                        dynamic filterValue = filterArr[2];
                        switch (filterValue)
                        {
                            case FilteringHelper.EmptyValue:
                                filterValue = "No option selected"; break;
                            case "true":
                                filterValue = true; break;
                            case "false":
                                filterValue = false; break;
                        }

                        if (filter.Contains("AccountStatus"))
                            isDelegateActive = filterValue;

                        if (filter.Contains("ProgressLocked"))
                            isProgressLocked = filterValue;

                        if (filter.Contains("ProgressRemoved"))
                            removed = filterValue;

                        if (filter.Contains("CompletionStatus"))
                            hasCompleted = filterValue;

                        if (filter.Contains("Answer1"))
                            answer1 = filterValue;

                        if (filter.Contains("Answer2"))
                            answer2 = filterValue;

                        if (filter.Contains("Answer3"))
                            answer3 = filterValue;
                    }
                }
            }
            var itemsPerPage = Data.Extensions.ConfigurationExtensions.GetExportQueryRowLimit(configuration);
            var content = courseDelegatesDownloadFileService.GetCourseDelegateDownloadFileForCourse(searchString ?? string.Empty, 0, itemsPerPage, sortBy, sortDirection,
                    customisationId, centreId, isDelegateActive, isProgressLocked, removed, hasCompleted, answer1, answer2, answer3
            );

            const string fileName = "Digital Learning Solutions Course Delegates.xlsx";
            return File(content,
                        FileHelper.GetContentTypeFromFileName(fileName),
                        fileName
            );
        }
    }
}
