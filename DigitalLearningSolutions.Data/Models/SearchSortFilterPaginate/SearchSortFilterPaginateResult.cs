namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{
    using System.Collections.Generic;

    public class SearchSortFilterPaginateResult<T> : PaginateResult<T> where T : BaseSearchableItem
    {
        public SearchSortFilterPaginateResult(
            PaginateResult<T> paginateResult,
            string? searchString,
            string? sortBy,
            string? sortDirection,
            string? filterString
        ) : base(paginateResult)
        {
            SearchString = searchString;
            SortBy = sortBy;
            SortDirection = sortDirection;
            FilterString = filterString;
        }

        public SearchSortFilterPaginateResult(
            IEnumerable<T> itemsToDisplay,
            int page,
            int totalPages,
            int itemsPerPage,
            int matchingSearchResults,
            string? searchString,
            string? sortBy,
            string? sortDirection,
            string? filterString
        ) : base(itemsToDisplay, page, totalPages, itemsPerPage, matchingSearchResults)
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
    }
}
