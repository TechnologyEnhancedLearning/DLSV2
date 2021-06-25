namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public abstract class BaseSearchablePageViewModel
    {
        public const string Descending = "Descending";
        public const string Ascending = "Ascending";

        public readonly string? FilterString;

        private readonly int itemsPerPage;

        public readonly string? SearchString;

        public int MatchingSearchResults;

        protected BaseSearchablePageViewModel(
            string? searchString,
            string sortBy,
            string sortDirection,
            string? filterString,
            int page,
            int itemsPerPage = 10
        )
        {
            SortBy = sortBy;
            SortDirection = sortDirection;
            SearchString = searchString;
            FilterString = filterString;
            Page = page;
            Filters = new List<(string, IEnumerable<(string, string)>)>();
            this.itemsPerPage = itemsPerPage;
        }

        public string SortDirection { get; set; }

        public string SortBy { get; set; }

        public int Page { get; protected set; }

        public int TotalPages { get; protected set; }

        public IEnumerable<SelectListItem> SortBySelectListItems =>
            SelectListHelper.MapOptionsToSelectListItems(SortOptions);

        public abstract IEnumerable<(string, string)> SortOptions { get; }

        public IEnumerable<(string, IEnumerable<(string, string)>)> Filters { get; set; }

        protected IEnumerable<T> GetItemsOnCurrentPage<T>(IList<T> items)
        {
            if (items.Count > itemsPerPage)
            {
                items = items.Skip(OffsetFromPageNumber(Page)).Take(itemsPerPage).ToList();
            }

            return items;
        }

        protected void SetTotalPages()
        {
            TotalPages = (int)Math.Ceiling(MatchingSearchResults / (double)itemsPerPage);
            if (Page < 1 || Page > TotalPages)
            {
                Page = 1;
            }
        }

        private int OffsetFromPageNumber(int pageNumber)
        {
            return (pageNumber - 1) * itemsPerPage;
        }
    }
}
