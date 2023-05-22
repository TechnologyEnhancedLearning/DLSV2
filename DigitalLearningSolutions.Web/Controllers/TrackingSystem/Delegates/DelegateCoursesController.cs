﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
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

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/Courses")]
    public class DelegateCoursesController : Controller
    {
        private const string CourseFilterCookieName = "DelegateCoursesFilter";
        private readonly ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService;
        private readonly ICourseService courseService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;

        public DelegateCoursesController(
            ICourseService courseService,
            ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.courseService = courseService;
            this.courseDelegatesDownloadFileService = courseDelegatesDownloadFileService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
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

            var details = courseService.GetCentreCourseDetailsWithAllCentreCourses(centreId, categoryId);
            var courses = UpdateCoursesNotActiveStatus(details.Courses);

            var availableFilters = DelegateCourseStatisticsViewModelFilterOptions
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
                courses,
                searchSortPaginationOptions
            );

            var model = new DelegateCoursesViewModel(
                result,
                availableFilters
            );

            Response.UpdateFilterCookie(CourseFilterCookieName, result.FilterString);

            return View(model);
        }

        [Route("AllCourseStatistics")]
        public IActionResult AllCourseStatistics()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var categoryId = User.GetAdminCategoryId();
            var details = courseService.GetCentreCourseDetailsWithAllCentreCourses(centreId, categoryId);

            var model = new AllDelegateCourseStatisticsViewModel(details);

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
            var content = courseDelegatesDownloadFileService.GetCourseDelegateDownloadFile(
                centreId,
                categoryId,
                searchString,
                sortBy,
                existingFilterString,
                sortDirection
            );

            const string fileName = "Digital Learning Solutions Delegate Courses.xlsx";
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
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
