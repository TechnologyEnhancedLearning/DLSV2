namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    public abstract class BaseFrameworksPageViewModel
    {
        [BindProperty] public string SortDirection { get; set; }
        [BindProperty] public string SortBy { get; set; }
        public int Page { get; protected set; }
        public int TotalPages { get; protected set; }
        public int MatchingSearchResults;
        public abstract SelectList FrameworkSortByOptions { get; }

        public const string DescendingText = "Descending";
        public const string AscendingText = "Ascending";

        private const int ItemsPerPage = 20;

        public readonly string? SearchString;

        protected BaseFrameworksPageViewModel(
            string? searchString,
            string sortBy,
            string sortDirection,
            int page
        )
        {
            SortBy = sortBy;
            SortDirection = sortDirection;
            SearchString = searchString;
            Page = page;
        }
        protected IEnumerable<BrandedFramework> PaginateItems(IList<BrandedFramework> items)
        {
            if (items.Count > ItemsPerPage)
            {
                items = items.Skip(OffsetFromPageNumber(Page)).Take(ItemsPerPage).ToList();
            }

            return items;
        }
        protected void SetTotalPages()
        {
            TotalPages = (int)Math.Ceiling(MatchingSearchResults / (double)ItemsPerPage);
            if (Page < 1 || Page > TotalPages)
            {
                Page = 1;
            }
        }

        private int OffsetFromPageNumber(int pageNumber) =>
            (pageNumber - 1) * ItemsPerPage;
    }
}
public static class FrameworkSortByOptionTexts
{
    public const string
        FrameworkName = "Framework Name",
        FrameworkOwner = "Owner",
        FrameworkCreatedDate = "Created Date",
        FrameworkPublishStatus = "Publish Status",
        FrameworkBrand = "Brand",
        FrameworkCategory = "Category",
        FrameworkTopic = "Topic";
}
