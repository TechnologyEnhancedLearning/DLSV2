namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SelectCourseViewModel : BaseSearchablePageViewModel<ApplicationDetails>
    {
        public SelectCourseViewModel(
            SearchSortFilterPaginateResult<ApplicationDetails> result,
            IEnumerable<FilterModel> availableFilters,
            string? categoryFilterString,
            string? topicFilterString
        ) : base(
            result,
            true,
            availableFilters
        )
        {
            ApplicationOptions = result.ItemsToDisplay.Select(a => new FilterableApplicationSelectListItemViewModel(a));

            CategoryFilterString = categoryFilterString;
            TopicFilterString = topicFilterString;
        }

        public int? ApplicationId { get; set; }
        public IEnumerable<FilterableApplicationSelectListItemViewModel> ApplicationOptions { get; set; }
        public string? CategoryFilterString { get; set; }
        public string? TopicFilterString { get; set; }
        public override IEnumerable<(string, string)> SortOptions { get; } = Array.Empty<(string, string)>();
        public override bool NoDataFound => !ApplicationOptions.Any() && NoSearchOrFilter;
    }
}
