namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public abstract class BasePaginatedViewModel
    {
        public const int DefaultItemsPerPage = 10;

        public int MatchingSearchResults;

        protected BasePaginatedViewModel(int page, int itemsPerPage = DefaultItemsPerPage)
        {
            Page = page;
            ItemsPerPage = itemsPerPage;
        }

        public int Page { get; protected set; }

        public int TotalPages { get; protected set; }

        public int ItemsPerPage { get; protected set; }

        public IEnumerable<SelectListItem> ItemsPerPageSelectListItems =>
            SelectListHelper.MapOptionsToSelectListItems(ItemsPerPageOptions);

        public virtual IEnumerable<(int, string)> ItemsPerPageOptions { get; } = new[]
        {
            (10, "10"),
            (25, "25"),
            (50, "50"),
            (100, "100"),
        };

        protected IEnumerable<T> GetItemsOnCurrentPage<T>(IList<T> items)
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

        protected IEnumerable<T> PaginateItems<T>(IEnumerable<T> items) where T:BaseSearchableItem
        {
            var listedItems = items.ToList();
            MatchingSearchResults = listedItems.Count;
            SetTotalPages();
            return GetItemsOnCurrentPage(listedItems);
        }

        private int OffsetFromPageNumber(int pageNumber)
        {
            return (pageNumber - 1) * ItemsPerPage;
        }
    }
}
