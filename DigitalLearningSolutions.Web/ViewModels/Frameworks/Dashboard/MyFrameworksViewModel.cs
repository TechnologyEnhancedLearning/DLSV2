﻿namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;
    public class MyFrameworksViewModel : BaseFrameworksPageViewModel
    {
        public readonly IEnumerable<BrandedFramework> BrandedFrameworks;
        public readonly bool IsFrameworkDeveloper;
        public override SelectList FrameworkSortByOptions { get; } = new SelectList(new[]
    {
            FrameworkSortByOptionTexts.FrameworkName,
            FrameworkSortByOptionTexts.FrameworkOwner,
            FrameworkSortByOptionTexts.FrameworkCreatedDate,
            FrameworkSortByOptionTexts.FrameworkPublishStatus
        });
        public MyFrameworksViewModel(
            IEnumerable<BrandedFramework> brandedFrameworks,
            string? searchString,
            string sortBy,
            string sortDirection,
            int page,
            bool isFrameworkDeveloper
        ) : base(searchString, sortBy, sortDirection, page)
        {
            var sortedItems = SortingHelper.SortFrameworkItems(
                brandedFrameworks,
                sortBy,
                sortDirection
            );
            var filteredItems = SearchHelper.FilterFrameworks(sortedItems, SearchString, 60, false).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = PaginateItems(filteredItems);
            BrandedFrameworks = paginatedItems.Cast<BrandedFramework>();
            IsFrameworkDeveloper = isFrameworkDeveloper;
        }
    }
}
