namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public interface ISearchSortFilterPaginateService
    {
        SearchSortFilterPaginateResult<T> SearchFilterSortAndPaginate<T>(
            IEnumerable<T> items,
            SearchSortFilterAndPaginateOptions searchSortFilterAndPaginateOptions
        ) where T : BaseSearchableItem;
    }

    public class SearchSortFilterPaginateService : ISearchSortFilterPaginateService
    {
        public SearchSortFilterPaginateResult<T> SearchFilterSortAndPaginate<T>(
            IEnumerable<T> items,
            SearchSortFilterAndPaginateOptions searchSortFilterAndPaginateOptions
        ) where T : BaseSearchableItem
        {
            var itemsToReturn = items;
            string? appliedFilterString = null;

            if (searchSortFilterAndPaginateOptions.SearchOptions != null)
            {
                itemsToReturn = GenericSearchHelper.SearchItems(
                    itemsToReturn,
                    searchSortFilterAndPaginateOptions.SearchOptions.SearchString,
                    searchSortFilterAndPaginateOptions.SearchOptions.SearchMatchCutoff
                );
            }

            if (searchSortFilterAndPaginateOptions.FilterOptions != null)
            {
                (itemsToReturn, appliedFilterString) = FilterOrResetFilterToDefault(
                    itemsToReturn,
                    searchSortFilterAndPaginateOptions.FilterOptions
                );
            }

            if (searchSortFilterAndPaginateOptions.SortOptions != null)
            {
                itemsToReturn = GenericSortingHelper.SortAllItems(
                    itemsToReturn.AsQueryable(),
                    searchSortFilterAndPaginateOptions.SortOptions.SortBy,
                    searchSortFilterAndPaginateOptions.SortOptions.SortDirection
                );
            }

            var paginateResult = PaginateItems(itemsToReturn, searchSortFilterAndPaginateOptions.PaginationOptions);

            return new SearchSortFilterPaginateResult<T>(
                paginateResult,
                searchSortFilterAndPaginateOptions.SearchOptions?.SearchString,
                searchSortFilterAndPaginateOptions.SortOptions?.SortBy,
                searchSortFilterAndPaginateOptions.SortOptions?.SortDirection,
                appliedFilterString
            );
        }

        private static (IEnumerable<T>, string?) FilterOrResetFilterToDefault<T>(
            IEnumerable<T> items,
            FilterOptions filterOptions
        )
            where T : BaseSearchableItem
        {
            if (AvailableFiltersContainsAllFilters(filterOptions))
            {
                return (FilteringHelper.FilterItems(items.AsQueryable(), filterOptions.FilterString),
                    filterOptions.FilterString);
            }

            return filterOptions.DefaultFilterString != null
                ? (FilteringHelper.FilterItems(items.AsQueryable(), filterOptions.DefaultFilterString),
                    filterOptions.DefaultFilterString)
                : (items, null);
        }

        private static bool AvailableFiltersContainsAllFilters(FilterOptions filterOptions)
        {
            var currentFilters = filterOptions.FilterString?.Split(FilteringHelper.FilterSeparator).ToList() ??
                                 new List<string>();

            return currentFilters.All(filter => AvailableFiltersContainsFilter(filterOptions.AvailableFilters, filter));
        }

        private static bool AvailableFiltersContainsFilter(IEnumerable<FilterModel> availableFilters, string filter)
        {
            return availableFilters.Any(filterModel => FilterOptionsContainsFilter(filter, filterModel.FilterOptions));
        }

        private static bool FilterOptionsContainsFilter(
            string filter,
            IEnumerable<FilterOptionModel> filterOptions
        )
        {
            return filterOptions.Any(filterOption => filterOption.FilterValue == filter);
        }

        private static PaginateResult<T> PaginateItems<T>(
            IEnumerable<T> items,
            PaginationOptions? paginationOptions
        )
            where T : BaseSearchableItem
        {
            var paginationOptionsToUse = paginationOptions ?? new PaginationOptions(1, int.MaxValue);

            var listedItems = items.ToList();
            var matchingSearchResults = listedItems.Count;
            var totalPages = GetTotalPages(matchingSearchResults, paginationOptionsToUse.ItemsPerPage);
            var itemsToDisplay = GetItemsOnCurrentPage(
                listedItems,
                paginationOptionsToUse
            );
            return new PaginateResult<T>(
                itemsToDisplay,
                paginationOptionsToUse,
                totalPages,
                matchingSearchResults
            );
        }

        private static IEnumerable<T> GetItemsOnCurrentPage<T>(IList<T> items, PaginationOptions paginationOptions)
        {
            return items.Count > paginationOptions.ItemsPerPage
                ? items.Skip(OffsetFromPageNumber(paginationOptions)).Take(paginationOptions.ItemsPerPage).ToList()
                : items;
        }

        private static int GetTotalPages(int matchingSearchResults, int itemsPerPage)
        {
            return (int)Math.Ceiling(matchingSearchResults / (double)itemsPerPage);
        }

        private static int OffsetFromPageNumber(PaginationOptions paginationOptions)
        {
            return (paginationOptions.PageNumber - 1) * paginationOptions.ItemsPerPage;
        }
    }
}
