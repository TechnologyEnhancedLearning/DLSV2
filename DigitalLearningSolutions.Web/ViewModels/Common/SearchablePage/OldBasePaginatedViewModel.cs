namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Obsolete]
    public abstract class OldBasePaginatedViewModel : IBasePaginatedViewModel
    {
        public const int DefaultItemsPerPage = 10;

        protected OldBasePaginatedViewModel(int page, int itemsPerPage = DefaultItemsPerPage)
        {
            Page = page;
            ItemsPerPage = itemsPerPage;
        }

        public int MatchingSearchResults { get; set; }

        public virtual int Page { get; set; }

        public virtual int TotalPages { get; set; }

        public int ItemsPerPage { get; set; }

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

        protected IEnumerable<T> PaginateItems<T>(IEnumerable<T> items) where T : BaseSearchableItem
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
