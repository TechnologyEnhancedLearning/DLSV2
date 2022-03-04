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
            string? searchString = null,
            int searchMatchCutoff = GenericSearchHelper.MatchCutoffScore,
            string? sortBy = GenericSortingHelper.DefaultSortOption,
            string? sortDirection = GenericSortingHelper.Ascending,
            string? filterString = null,
            string? defaultFilterString = null,
            IEnumerable<FilterModel>? availableFilters = null,
            int pageNumber = 1,
            int itemsPerPage = SearchSortFilterPaginateService.DefaultItemsPerPage
        ) where T : BaseSearchableItem;

        PaginateResult<T> PaginateItems<T>(
            IEnumerable<T> items,
            int pageNumber,
            int itemsPerPage = SearchSortFilterPaginateService.DefaultItemsPerPage
        )
            where T : BaseSearchableItem;
    }

    public class SearchSortFilterPaginateService : ISearchSortFilterPaginateService
    {
        public const int DefaultItemsPerPage = 10;

        public SearchSortFilterPaginateResult<T> SearchFilterSortAndPaginate<T>(
            IEnumerable<T> items,
            string? searchString = null,
            int searchMatchCutoff = GenericSearchHelper.MatchCutoffScore,
            string? sortBy = GenericSortingHelper.DefaultSortOption,
            string? sortDirection = GenericSortingHelper.Ascending,
            string? filterString = null,
            string? defaultFilterString = null,
            IEnumerable<FilterModel>? availableFilters = null,
            int pageNumber = 1,
            int itemsPerPage = DefaultItemsPerPage
        ) where T : BaseSearchableItem
        {
            var searchedItems = Search(items, searchString);
            var (filteredItems, appliedFilterString) = FilterOrResetFilterToDefault(
                searchedItems,
                availableFilters,
                filterString,
                defaultFilterString
            );
            var sortedItems = sortBy != null && sortDirection != null ? Sort(filteredItems, sortBy, sortDirection) : filteredItems;
            var paginateResult = PaginateItems(sortedItems, pageNumber, itemsPerPage);
            return new SearchSortFilterPaginateResult<T>(
                paginateResult,
                searchString,
                sortBy,
                sortDirection,
                appliedFilterString
            );
        }

        public PaginateResult<T> PaginateItems<T>(
            IEnumerable<T> items,
            int pageNumber,
            int itemsPerPage = DefaultItemsPerPage
        )
            where T : BaseSearchableItem
        {
            var listedItems = items.ToList();
            var matchingSearchResults = listedItems.Count;
            var totalPages = GetTotalPages(matchingSearchResults, itemsPerPage);
            var itemsToDisplay = GetItemsOnCurrentPage(listedItems, pageNumber, itemsPerPage);
            return new PaginateResult<T>(itemsToDisplay, pageNumber, totalPages, itemsPerPage, matchingSearchResults);
        }

        private IEnumerable<T> Sort<T>(
            IEnumerable<T> items,
            string sortBy,
            string sortDirection
        ) where T : BaseSearchableItem
        {
            return GenericSortingHelper.SortAllItems(items.AsQueryable(), sortBy, sortDirection);
        }

        private static (IEnumerable<T>, string?) FilterOrResetFilterToDefault<T>(
            IEnumerable<T> items,
            IEnumerable<FilterModel>? availableFilters,
            string? filterString,
            string? defaultFilterString
        )
            where T : BaseSearchableItem
        {
            if (AvailableFiltersContainsAllFilters(availableFilters, filterString))
            {
                return (FilteringHelper.FilterItems(items.AsQueryable(), filterString), filterString);
            }

            return defaultFilterString != null
                ? (FilteringHelper.FilterItems(items.AsQueryable(), defaultFilterString), defaultFilterString)
                : (items, null);
        }

        private static bool AvailableFiltersContainsAllFilters(
            IEnumerable<FilterModel>? availableFilters,
            string? filterString
        )
        {
            if (availableFilters == null)
            {
                return filterString == null;
            }

            var currentFilters = filterString?.Split(FilteringHelper.FilterSeparator).ToList() ??
                                 new List<string>();

            return currentFilters.All(filter => AvailableFiltersContainsFilter(availableFilters, filter));
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

        private IEnumerable<T> Search<T>(IEnumerable<T> items, string? searchString = null)
            where T : BaseSearchableItem
        {
            return GenericSearchHelper.SearchItems(items, searchString);
        }

        protected IEnumerable<T> GetItemsOnCurrentPage<T>(IList<T> items, int pageNumber, int itemsPerPage)
        {
            if (items.Count > itemsPerPage)
            {
                items = items.Skip(OffsetFromPageNumber(pageNumber, itemsPerPage)).Take(itemsPerPage).ToList();
            }

            return items;
        }

        private int GetTotalPages(int matchingSearchResults, int itemsPerPage)
        {
            return (int)Math.Ceiling(matchingSearchResults / (double)itemsPerPage);
        }

        private int OffsetFromPageNumber(int pageNumber, int itemsPerPage)
        {
            return (pageNumber - 1) * itemsPerPage;
        }
    }
}
