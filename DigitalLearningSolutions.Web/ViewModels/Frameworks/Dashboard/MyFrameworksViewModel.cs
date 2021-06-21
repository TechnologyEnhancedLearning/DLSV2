﻿namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class MyFrameworksViewModel : BaseSearchablePageViewModel
    {
        public readonly IEnumerable<BrandedFramework> BrandedFrameworks;
        public readonly bool IsFrameworkDeveloper;

        public MyFrameworksViewModel(
            IEnumerable<BrandedFramework> brandedFrameworks,
            string? searchString,
            string sortBy,
            string sortDirection,
            int page,
            bool isFrameworkDeveloper
        ) : base(searchString, sortBy, sortDirection, page, 12)
        {
            var sortedItems = GenericSortingHelper.SortAllItems(
                brandedFrameworks.AsQueryable(),
                sortBy,
                sortDirection
            );
            var filteredItems = GenericSearchHelper.SearchItems(sortedItems, SearchString, 60).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);
            BrandedFrameworks = paginatedItems;
            IsFrameworkDeveloper = isFrameworkDeveloper;
        }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            FrameworkSortByOptions.FrameworkName,
            FrameworkSortByOptions.FrameworkOwner,
            FrameworkSortByOptions.FrameworkCreatedDate,
            FrameworkSortByOptions.FrameworkPublishStatus
        };
    }
}
