namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using Microsoft.Extensions.Configuration;

    public class CompletedPageViewModel : BaseSearchablePageViewModel
    {
        public readonly string? BannerText;

        public CompletedPageViewModel(
            IEnumerable<CompletedCourse> completedCourses,
            IConfiguration config,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? bannerText,
            int page
        ) : base(searchString, page, true, false, sortBy, sortDirection)
        {
            BannerText = bannerText;
            var sortedItems = GenericSortingHelper.SortAllItems(
                completedCourses.AsQueryable(),
                sortBy,
                sortDirection
            );
            var filteredItems = GenericSearchHelper.SearchItems(sortedItems, SearchString).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);
            CompletedCourses = paginatedItems.Select(
                completedCourse =>
                    new CompletedCourseViewModel(completedCourse, config)
            );
        }

        public IEnumerable<CompletedCourseViewModel> CompletedCourses { get; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.Name,
            CourseSortByOptions.StartedDate,
            CourseSortByOptions.LastAccessed,
            CourseSortByOptions.CompletedDate
        };
    }
}
