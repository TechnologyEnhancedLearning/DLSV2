namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SelectCourseViewModel : BaseSearchablePageViewModel
    {
        public SelectCourseViewModel(
            IEnumerable<ApplicationDetails> applications,
            IEnumerable<string> categories,
            IEnumerable<string> topics,
            int? adminCategoryFilter,
            string? categoryFilterString,
            string? topicFilterString
        ) : base(
            null,
            1,
            true,
            nameof(ApplicationDetails.ApplicationName),
            GenericSortingHelper.Ascending,
            FilteringHelper.GetCategoryAndTopicFilterString(categoryFilterString, topicFilterString)
        )
        {
            var applicationsList = applications.ToList();
            var applicationsToShow = FilterItems(applicationsList);

            ApplicationOptions = applicationsToShow.Select(a => new FilterableApplicationSelectListItemViewModel(a));

            CategoryFilterString = categoryFilterString;
            TopicFilterString = topicFilterString;

            Filters = adminCategoryFilter == null
                ? SelectCourseViewModelFilterOptions.GetAllCategoriesFilters(
                    categories,
                    topics,
                    categoryFilterString,
                    topicFilterString
                )
                : SelectCourseViewModelFilterOptions.GetSingleCategoryFilters(
                    applicationsList,
                    categoryFilterString,
                    topicFilterString
                );
        }

        public int? ApplicationId { get; set; }
        public IEnumerable<FilterableApplicationSelectListItemViewModel> ApplicationOptions { get; set; }
        public string? CategoryFilterString { get; set; }
        public string? TopicFilterString { get; set; }
        public override IEnumerable<(string, string)> SortOptions { get; } = Array.Empty<(string, string)>();
        public override bool NoDataFound => !ApplicationOptions.Any() && NoSearchOrFilter;
    }
}
