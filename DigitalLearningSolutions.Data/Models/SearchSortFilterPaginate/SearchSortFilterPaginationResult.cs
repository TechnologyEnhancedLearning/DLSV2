namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{
    using System.Collections.Generic;

    public class SearchSortFilterPaginationResult<T> : PaginationResult<T> where T : BaseSearchableItem
    {
        public SearchSortFilterPaginationResult(
            PaginationResult<T> paginationResult,
            string? searchString,
            string? sortBy,
            string? sortDirection,
            string? filterString
        ) : base(paginationResult)
        {
            SearchString = searchString;
            SortBy = sortBy;
            SortDirection = sortDirection;
            FilterString = filterString;
        }

        public SearchSortFilterPaginationResult(
            IEnumerable<T> itemsToDisplay,
            int page,
            int totalPages,
            int itemsPerPage,
            int matchingSearchResults,
            bool javascriptPaginationShouldBeEnabled,
            string? searchString,
            string? sortBy,
            string? sortDirection,
            string? filterString
        ) : base(
            itemsToDisplay,
            page,
            totalPages,
            itemsPerPage,
            matchingSearchResults,
            javascriptPaginationShouldBeEnabled
        )
        {
            SearchString = searchString;
            SortBy = sortBy;
            SortDirection = sortDirection;
            FilterString = filterString;
        }

        public string? SearchString { get; set; }

        public string? SortBy { get; set; }

        public string? SortDirection { get; set; }

        public string? FilterString { get; set; }

        public new ReturnPageQuery GetReturnPageQuery(string? itemIdToReturnTo = null)
        {
            return new ReturnPageQuery(Page, itemIdToReturnTo, ItemsPerPage, SearchString, SortBy, SortDirection);
        }
    }
}
