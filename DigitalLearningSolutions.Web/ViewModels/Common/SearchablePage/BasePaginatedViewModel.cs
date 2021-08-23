﻿namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class BasePaginatedViewModel
    {
        public const int DefaultItemsPerPage = 10;
        private readonly int itemsPerPage;

        public int MatchingSearchResults;

        protected BasePaginatedViewModel(int page, int itemsPerPage = DefaultItemsPerPage)
        {
            Page = page;
            this.itemsPerPage = itemsPerPage;
        }

        public int Page { get; protected set; }

        public int TotalPages { get; protected set; }

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
