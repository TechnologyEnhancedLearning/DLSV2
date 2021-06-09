namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AvailablePageViewModel : BaseSearchablePageViewModel
    {
        public readonly IEnumerable<AvailableCourseViewModel> AvailableCourses;

        public override List<SelectListItem> SortByOptions { get; } = new List<SelectListItem>
        {
            item1,  item2, item3, item4
        };

        private static SelectListItem item1 = new SelectListItem(SortByOptionTexts.Name, nameof(AvailableCourse.Name));
        private static SelectListItem item2 = new SelectListItem(SortByOptionTexts.Brand, nameof(AvailableCourse.Brand));
        private static SelectListItem item3 = new SelectListItem(SortByOptionTexts.Category, nameof(AvailableCourse.Category));
        private static SelectListItem item4 = new SelectListItem(SortByOptionTexts.Topic, nameof(AvailableCourse.Topic));

        //public override SelectList SortByOptions { get; } = new SelectList()

        public AvailablePageViewModel(
            IEnumerable<AvailableCourse> availableCourses,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? bannerText,
            int page
        ) : base(searchString, sortBy, sortDirection, page, bannerText)
        {
            var sortedItems = GenericSortingHelper.SortAllItems(
                availableCourses.AsQueryable(),
                sortBy,
                sortDirection
            );
            var filteredItems = GenericSearchHelper.SearchItems(sortedItems, SearchString).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = PaginateItems(filteredItems);
            AvailableCourses = paginatedItems.Select(c => new AvailableCourseViewModel(c));
        }
    }
}
