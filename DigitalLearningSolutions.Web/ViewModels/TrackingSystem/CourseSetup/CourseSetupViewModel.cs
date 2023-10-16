﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using Microsoft.Extensions.Configuration;

    public class CourseSetupViewModel : BaseSearchablePageViewModel<CourseStatisticsWithAdminFieldResponseCounts>
    {
        public CourseSetupViewModel(
            SearchSortFilterPaginationResult<CourseStatisticsWithAdminFieldResponseCounts> result,
            IEnumerable<FilterModel> availableFilters,
            IConfiguration config,
            string courseCategoryName
        ) : base(
            result,
            true,
            availableFilters,
            "Search courses"
        )
        {
            Courses = result.ItemsToDisplay.Select(c => new SearchableCourseStatisticsViewModel(c, config));
            CourseCategoryName = courseCategoryName;
        }

        public IEnumerable<SearchableCourseStatisticsViewModel> Courses { get; set; }
        public string CourseCategoryName { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.CourseName,
            CourseSortByOptions.TotalDelegates,
            CourseSortByOptions.InProgress,
        };

        public override bool NoDataFound => !Courses.Any() && NoSearchOrFilter;
    }
}
