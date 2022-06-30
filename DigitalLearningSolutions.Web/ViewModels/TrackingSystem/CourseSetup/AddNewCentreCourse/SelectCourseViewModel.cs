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
            SearchSortFilterPaginationResult<ApplicationDetails> result,
            IEnumerable<FilterModel> availableFilters,
            string? categoryFilterString,
            string? topicFilterString,
            int? selectedApplicationId = null
        ) : base(
            result,
            true,
            availableFilters
        )
        {
            ApplicationOptions = result.ItemsToDisplay.Select(a => new FilterableApplicationSelectListItemViewModel(a, selectedApplicationId));

            CategoryFilterString = categoryFilterString;
            TopicFilterString = topicFilterString;
            ApplicationId = selectedApplicationId;
        }

        public int? ApplicationId { get; set; }
        public IEnumerable<FilterableApplicationSelectListItemViewModel> ApplicationOptions { get; set; }
        public string? CategoryFilterString { get; set; }
        public string? TopicFilterString { get; set; }
        public override IEnumerable<(string, string)> SortOptions { get; } = Array.Empty<(string, string)>();
        public override bool NoDataFound => !ApplicationOptions.Any() && NoSearchOrFilter;
    }
}
