﻿namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class AvailablePageViewModel : BaseSearchablePageViewModel
    {
        public readonly IEnumerable<AvailableCourseViewModel> AvailableCourses;
        public readonly string? BannerText;

        public AvailablePageViewModel(
            IEnumerable<AvailableCourse> availableCourses,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? bannerText,
            int page
        ) : base(searchString, sortBy, sortDirection, page)
        {
            BannerText = bannerText;
            var sortedItems = GenericSortingHelper.SortAllItems(
                availableCourses.AsQueryable(),
                sortBy,
                sortDirection
            );
            var filteredItems = GenericSearchHelper.SearchItems(sortedItems, SearchString).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);
            AvailableCourses = paginatedItems.Select(c => new AvailableCourseViewModel(c));
        }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.Name,
            CourseSortByOptions.Brand,
            CourseSortByOptions.Category,
            CourseSortByOptions.Topic
        };
    }
}
