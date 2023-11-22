namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using Microsoft.Extensions.Configuration;

    public interface IPaginateService
    {
        SearchSortFilterPaginationResult<T> Paginate<T>(
            IEnumerable<T> items,
            int searchResultCount,
            PaginationOptions paginationOptions,
            FilterOptions filterOptions = null,
            string searchString = null,
            string sortBy = null,
            string sortDirection = null
        ) where T : BaseSearchableItem;
    }

    public class PaginateService : IPaginateService
    {
        private readonly IConfiguration configuration;

        public PaginateService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public SearchSortFilterPaginationResult<T> Paginate<T>(
            IEnumerable<T> items,
            int searchResultCount,
            PaginationOptions paginationOptions,
            FilterOptions filterOptions = null,
            string searchString = null,
            string sortBy = null,
            string sortDirection = null
        ) where T : BaseSearchableItem
        {
            var itemsToReturn = items.ToList();

            var paginateResult =  PaginateItems(
                itemsToReturn,
                searchResultCount,
                paginationOptions
            );

            return new SearchSortFilterPaginationResult<T>(
                paginateResult,
                searchString,
                sortBy,
                sortDirection,
                filterOptions?.FilterString
            );
        }

        private static PaginationResult<T> PaginateItems<T>(
            IEnumerable<T> items,
            int searchResultCount,
            PaginationOptions? paginationOptions
        )
            where T : BaseSearchableItem
        {
            var paginationOptionsToUse = paginationOptions ?? new PaginationOptions(1, int.MaxValue);

            var listedItems = items.ToList();
            var matchingSearchResults = searchResultCount;
            var totalPages = GetTotalPages(matchingSearchResults, paginationOptionsToUse.ItemsPerPage);
            var page = paginationOptionsToUse.PageNumber < 1 || paginationOptionsToUse.PageNumber > totalPages
                ? 1
                : paginationOptionsToUse.PageNumber;
            var itemsToDisplay = GetItemsOnCurrentPage(
                listedItems,
                page,
                paginationOptionsToUse.ItemsPerPage
            );
            return new PaginationResult<T>(
                itemsToDisplay,
                page,
                totalPages,
                paginationOptionsToUse.ItemsPerPage,
                matchingSearchResults,
                false
            );
        }

        private static IEnumerable<T> GetItemsOnCurrentPage<T>(IList<T> items, int page, int itemsPerPage)
        {
            return items.Count > itemsPerPage
                ? items.Skip(OffsetFromPageNumber(page, itemsPerPage)).Take(itemsPerPage).ToList()
                : items;
        }

        private static int GetTotalPages(int matchingSearchResults, int itemsPerPage)
        {
            return (int)Math.Ceiling(matchingSearchResults / (double)itemsPerPage);
        }

        private static int OffsetFromPageNumber(int page, int itemsPerPage)
        {
            return (page - 1) * itemsPerPage;
        }
    }
}
