namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;

    public class CompletedPageViewModel : BaseSearchablePageViewModel
    {
        public IEnumerable<CompletedCourseViewModel> CompletedCourses { get; }

        public override List<SelectListItem> SortByOptions { get; } = new List<SelectListItem>
        {
            item1,  item2, item3, item4
        };

        private static SelectListItem item1 = new SelectListItem(SortByOptionTexts.Name, nameof(CompletedCourse.Name));
        private static SelectListItem item2 = new SelectListItem(SortByOptionTexts.StartedDate, nameof(CompletedCourse.StartedDate));
        private static SelectListItem item3 = new SelectListItem(SortByOptionTexts.LastAccessed, nameof(CompletedCourse.LastAccessed));
        private static SelectListItem item4 = new SelectListItem(SortByOptionTexts.CompletedDate, nameof(CompletedCourse.Completed));

        public CompletedPageViewModel(
            IEnumerable<CompletedCourse> completedCourses,
            IConfiguration config,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? bannerText,
            int page
        ) : base (searchString, sortBy, sortDirection, page, bannerText)
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
            CompletedCourses = paginatedItems.Cast<CompletedCourse>().Select(completedCourse =>
                new CompletedCourseViewModel(completedCourse, config)
            );
        }
    }
}
