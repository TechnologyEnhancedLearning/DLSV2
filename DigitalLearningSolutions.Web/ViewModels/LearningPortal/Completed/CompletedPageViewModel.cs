namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.Extensions.Configuration;

    public class CompletedPageViewModel : BaseSearchablePageViewModel
    {
        public CompletedPageViewModel(
            IEnumerable<CompletedCourse> completedCourses,
            IConfiguration config,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? bannerText,
            int page
        ) : base(searchString, sortBy, sortDirection, page, bannerText)
        {
            var sortedItems = GenericSortingHelper.SortAllItems(
                completedCourses.AsQueryable(),
                sortBy,
                sortDirection
            );
            var filteredItems = GenericSearchHelper.SearchItems(sortedItems, SearchString).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = PaginateItems(filteredItems);
            CompletedCourses = paginatedItems.Select(
                completedCourse =>
                    new CompletedCourseViewModel(completedCourse, config)
            );
        }

        public IEnumerable<CompletedCourseViewModel> CompletedCourses { get; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptionTexts.Name,
            CourseSortByOptionTexts.StartedDate,
            CourseSortByOptionTexts.LastAccessed,
            CourseSortByOptionTexts.CompletedDate
        };
    }
}
