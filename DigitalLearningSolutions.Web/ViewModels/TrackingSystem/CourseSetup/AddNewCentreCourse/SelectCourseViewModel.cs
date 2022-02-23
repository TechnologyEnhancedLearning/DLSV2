namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using Microsoft.AspNetCore.Mvc.Rendering;

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
            AdminCategoryFilter = adminCategoryFilter;

            var applicationsList = applications.ToList();
            var applicationsToShow = FilterItems(applicationsList);

            ApplicationOptions = applicationsToShow.Select(a => new FilterableApplicationSelectListItemViewModel(a));

            ApplicationOptionsSelectList = SelectListHelper.MapOptionsToSelectListItems(
                ApplicationOptions.Select(a => (a.ApplicationId, a.ApplicationName))
            );

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

        [Required(ErrorMessage = "Select a course")]
        public int? ApplicationId { get; set; }

        public IEnumerable<FilterableApplicationSelectListItemViewModel> ApplicationOptions { get; set; }
        public IEnumerable<SelectListItem> ApplicationOptionsSelectList { get; set; }
        public string? CategoryFilterBy { get; set; }
        public string? TopicFilterBy { get; set; }
        public int? AdminCategoryFilter { get; set; }
        public override IEnumerable<(string, string)> SortOptions { get; } = Array.Empty<(string, string)>();
        public override bool NoDataFound => !ApplicationOptions.Any() && NoSearchOrFilter;
    }
}
