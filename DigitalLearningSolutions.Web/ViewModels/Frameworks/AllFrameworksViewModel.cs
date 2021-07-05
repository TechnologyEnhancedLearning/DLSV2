﻿namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class AllFrameworksViewModel : BaseSearchablePageViewModel
    {
        public readonly IEnumerable<BrandedFramework> BrandedFrameworks;

        public AllFrameworksViewModel(
            IEnumerable<BrandedFramework> brandedFrameworks,
            string? searchString,
            string sortBy,
            string sortDirection,
            int page
        ) : base(searchString, sortBy, sortDirection, page, itemsPerPage: 12)
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
        }

        public override IEnumerable<(string, string)> SortOptions { get; } = new []
        {
            FrameworkSortByOptions.FrameworkName,
            FrameworkSortByOptions.FrameworkOwner,
            FrameworkSortByOptions.FrameworkCreatedDate,
            FrameworkSortByOptions.FrameworkPublishStatus,
            FrameworkSortByOptions.FrameworkBrand,
            FrameworkSortByOptions.FrameworkCategory,
            FrameworkSortByOptions.FrameworkTopic
        };
    }
}
