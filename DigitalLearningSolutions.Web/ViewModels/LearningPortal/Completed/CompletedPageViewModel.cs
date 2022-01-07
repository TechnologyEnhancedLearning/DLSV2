namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using Microsoft.Extensions.Configuration;

    public class CompletedPageViewModel : BaseSearchablePageViewModel
    {
        public readonly string? BannerText;

        public CompletedPageViewModel(
            IEnumerable<CompletedCourse> completedCourses,
            IEnumerable<CompletedActionPlanResource> completedResources,
            IConfiguration config,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? bannerText,
            int page
        ) : base(searchString, page, false, sortBy, sortDirection, searchLabel: "Search your completed courses")
        {
            BannerText = bannerText;
            var allItems = completedCourses.Cast<CompletedLearningItem>().ToList();
            allItems.AddRange(completedResources);

            var sortedItems = GenericSortingHelper.SortAllItems(
                allItems.AsQueryable(),
                sortBy,
                sortDirection
            );
            var filteredItems = GenericSearchHelper.SearchItems(sortedItems, SearchString).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);
            CompletedActivities = paginatedItems.Select<BaseLearningItem, CompletedLearningItemViewModel>(
                activity =>
                {
                    return activity switch
                    {
                        CompletedCourse completedCourse => new CompletedCourseViewModel(completedCourse, config),
                        _ => new CompletedLearningResourceViewModel((CompletedActionPlanResource)activity),
                    };
                }
            );
        }

        public IEnumerable<CompletedLearningItemViewModel> CompletedActivities { get; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.Name,
            CourseSortByOptions.StartedDate,
            CourseSortByOptions.LastAccessed,
            CourseSortByOptions.CompletedDate,
        };

        public override bool NoDataFound => !CompletedActivities.Any() && NoSearchOrFilter;
    }
}
