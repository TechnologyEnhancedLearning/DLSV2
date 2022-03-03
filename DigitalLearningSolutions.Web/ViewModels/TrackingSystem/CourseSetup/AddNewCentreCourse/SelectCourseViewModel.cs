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
            string? categoryFilterBy,
            string? topicFilterBy
        ) : base(
            null,
            1,
            true,
            nameof(ApplicationDetails.ApplicationName),
            GenericSortingHelper.Ascending,
            FilteringHelper.GetCategoryAndTopicFilterBy(categoryFilterBy, topicFilterBy)
        )
        {
            var applicationsList = applications.ToList();
            var applicationsToShow = FilterItems(applicationsList);

            ApplicationOptions = applicationsToShow.Select(a => new FilterableApplicationSelectListItemViewModel(a));

            CategoryFilterBy = categoryFilterBy;
            TopicFilterBy = topicFilterBy;

            Filters = adminCategoryFilter == null
                ? SelectCourseViewModelFilterOptions.GetAllCategoriesFilters(
                    categories,
                    topics,
                    categoryFilterBy,
                    topicFilterBy
                )
                : SelectCourseViewModelFilterOptions.GetSingleCategoryFilters(
                    applicationsList,
                    categoryFilterBy,
                    topicFilterBy
                );
        }

        public int? ApplicationId { get; set; }
        public IEnumerable<FilterableApplicationSelectListItemViewModel> ApplicationOptions { get; set; }
        public string? CategoryFilterBy { get; set; }
        public string? TopicFilterBy { get; set; }
        public override IEnumerable<(string, string)> SortOptions { get; } = Array.Empty<(string, string)>();
        public override bool NoDataFound => !ApplicationOptions.Any() && NoSearchOrFilter;
    }
}
