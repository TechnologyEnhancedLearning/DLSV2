namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
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
            SearchSortFilterPaginateResult<CourseStatisticsWithAdminFieldResponseCounts> result,
            IEnumerable<FilterModel> availableFilters,
            IConfiguration config
        ) : base(
            result,
            true,
            availableFilters,
            "Search courses"
        )
        {
            Courses = result.ItemsToDisplay.Select(c => new SearchableCourseStatisticsViewModel(c, config));
        }

        public IEnumerable<SearchableCourseStatisticsViewModel> Courses { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.CourseName,
            CourseSortByOptions.TotalDelegates,
            CourseSortByOptions.InProgress,
        };

        public override bool NoDataFound => !Courses.Any() && NoSearchOrFilter;
    }
}
