﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateCourses;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/Activities")]
    public class DelegateCoursesController : Controller
    {
        private const string CourseFilterCookieName = "DelegateCoursesFilter";
        private readonly ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService;
        private readonly ICourseService courseService;
        private readonly IPaginateService paginateService;
        private readonly IActivityService activityService;
        private readonly ICourseCategoriesService courseCategoriesService;
        private readonly ICourseTopicsService courseTopicsService;

        public DelegateCoursesController(
            ICourseService courseService,
            ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService,
            IPaginateService paginateService,
            IActivityService activityService,
            ICourseCategoriesService courseCategoriesService,
            ICourseTopicsService courseTopicsService
        )
        {
            this.courseService = courseService;
            this.courseDelegatesDownloadFileService = courseDelegatesDownloadFileService;
            this.paginateService = paginateService;
            this.activityService = activityService;
            this.courseCategoriesService = courseCategoriesService;
            this.courseTopicsService = courseTopicsService;
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

            var centreId = User.GetCentreIdKnownNotNull();
            var categoryId = User.GetAdminCategoryId();
            var courseCategoryName = this.activityService.GetCourseCategoryNameForActivityFilter(categoryId);
            var Categories = courseCategoriesService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId).Select(c => c.CategoryName);
            var Topics = courseTopicsService.GetCourseTopicsAvailableAtCentre(centreId).Select(c => c.CourseTopic);

            int offSet = ((page - 1) * itemsPerPage) ?? 0;
            string isActive, categoryName, courseTopic, hasAdminFields, isCourse, isSelfAssessment;
            isActive = categoryName = courseTopic = hasAdminFields = isCourse = isSelfAssessment = "Any";

            var availableFilters = DelegateCourseStatisticsViewModelFilterOptions
               .GetFilterOptions(categoryId.HasValue ? new string[] { } : Categories, Topics).ToList();

            var filterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                CourseFilterCookieName,
                CourseStatusFilterOptions.IsActive.FilterValue,
                availableFilters
            );

            if (!string.IsNullOrEmpty(filterString))
            {
                var selectedFilters = filterString.Split(FilteringHelper.FilterSeparator).ToList();

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

                        if (filter.Contains("Course|"))
                            isCourse = filterValue;

                        if (filter.Contains("SelfAssessment"))
                            isSelfAssessment = filterValue;
                    }
                }
            }

            IEnumerable<DelegateAssessmentStatistics> delegateAssessments = new DelegateAssessmentStatistics[] { };
            IEnumerable<CourseStatisticsWithAdminFieldResponseCounts> delegateActivities = new CourseStatisticsWithAdminFieldResponseCounts[] { };

            if (isCourse == "Any" && isSelfAssessment == "Any")
            {
                delegateActivities = courseService.GetDelegateCourses(searchString, centreId, categoryId, true, null, isActive, categoryName, courseTopic, hasAdminFields).ToList();
                if (courseTopic == "Any" && hasAdminFields == "Any")
                    delegateAssessments = courseService.GetDelegateAssessments(searchString, centreId, categoryName, isActive, categoryId);
            }

            if (isCourse == "true")
                delegateActivities = courseService.GetDelegateCourses(searchString ?? string.Empty, centreId, categoryId, true, null, isActive, categoryName, courseTopic, hasAdminFields).ToList();
            if (isSelfAssessment == "true" && courseTopic == "Any" && hasAdminFields == "Any")
                delegateAssessments = courseService.GetDelegateAssessments(searchString, centreId, categoryName, isActive, categoryId);

            delegateAssessments = UpdateCompletedCount(delegateAssessments);

            var allItems = delegateActivities.Cast<CourseStatistics>().ToList();
            allItems.AddRange(delegateAssessments);

            allItems = OrderActivities(allItems, sortBy, sortDirection);

            var resultCount = allItems.Count();

            var result = paginateService.Paginate(
                allItems,
                resultCount,
                new PaginationOptions(page, itemsPerPage),
                new FilterOptions(filterString, availableFilters, DelegateActiveStatusFilterOptions.IsActive.FilterValue),
                searchString,
                sortBy,
                sortDirection
            );

            result.Page = page;
            TempData["Page"] = result.Page;

            var model = new DelegateCoursesViewModel(
                result,
                availableFilters,
                courseCategoryName
            );


            for (int optionIndex = 0; model.SortOptions.Count() > optionIndex; optionIndex++)
            {
                if ((((string, string)[])model.SortOptions)[optionIndex].Item1 == "Completed")
                {
                    (((string, string)[])model.SortOptions)[optionIndex].Item1 = "Completed/Signed off/Submitted";
                }
            }

            model.TotalPages = (int)(resultCount / itemsPerPage) + ((resultCount % itemsPerPage) > 0 ? 1 : 0);
            model.MatchingSearchResults = resultCount;
            Response.UpdateFilterCookie(CourseFilterCookieName, result.FilterString);

            return View(model);
        }

        [Route("DownloadAll")]
        public IActionResult DownloadAll(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null
        )
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var categoryId = User.GetAdminCategoryId();

            searchString = searchString == null ? string.Empty : searchString.Trim();

            string isActive, categoryName, courseTopic, hasAdminFields, isCourse, isSelfAssessment;
            isActive = categoryName = courseTopic = hasAdminFields = isCourse = isSelfAssessment = "Any";

            if (!string.IsNullOrEmpty(existingFilterString))
            {
                var selectedFilters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();

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

                        if (filter.Contains("Course|"))
                            isCourse = filterValue;

                        if (filter.Contains("SelfAssessment"))
                            isSelfAssessment = filterValue;
                    }
                }
            }

            if (!string.IsNullOrEmpty(existingFilterString))
            {
                if (existingFilterString.Contains("NotActive"))
                    existingFilterString = existingFilterString.Replace("NotActive|true", "Active|false");

                var filters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();

                foreach (var filter in filters)
                {
                    if (filter.Contains("Type|"))
                    {
                        filters.Remove(filter);
                        existingFilterString = string.Join(FilteringHelper.FilterSeparator, filters);
                        break;
                    }
                }
                if (existingFilterString == "") existingFilterString = null;
            }

            var content = courseDelegatesDownloadFileService.GetActivityDelegateDownloadFile(
                centreId,
                categoryId,
                searchString,
                existingFilterString,
                courseTopic,
                hasAdminFields,
                categoryName,
                isActive,
                isCourse,
                isSelfAssessment,
                sortBy,
                sortDirection
            );

            const string fileName = "Digital Learning Solutions Delegate Activities.xlsx";
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
            fileName
            );
        }

        private IEnumerable<DelegateAssessmentStatistics> UpdateCompletedCount(IEnumerable<DelegateAssessmentStatistics> statistics)
        {
            foreach (var statistic in statistics)
            {
                statistic.CompletedCount = statistic.SubmittedSignedOffCount;
            }
            return statistics;
        }

        private List<CourseStatistics> OrderActivities(List<CourseStatistics> allItems, string sortBy, string sortDirection)
        {
            if (sortBy == "InProgressCount")
            {
                allItems = sortDirection == "Ascending"
                            ? allItems.OrderBy(x => x.InProgressCount).ThenBy(n => n.SearchableName).ToList()
                            : allItems.OrderByDescending(x => x.InProgressCount).ThenBy(n => n.SearchableName).ToList();
            }
            else if (sortBy == "CompletedCount")
            {
                allItems = sortDirection == "Ascending"
                            ? allItems.OrderBy(x => x.CompletedCount).ThenBy(n => n.SearchableName).ToList()
                            : allItems.OrderByDescending(x => x.CompletedCount).ThenBy(n => n.SearchableName).ToList();
            }
            else
            {
                allItems = sortDirection == "Ascending"
                            ? allItems.OrderBy(x => x.SearchableName).ToList()
                            : allItems.OrderByDescending(x => x.SearchableName).ToList();
            }
            return allItems;
        }
    }
}
