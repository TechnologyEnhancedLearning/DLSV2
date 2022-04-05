namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{
    using System.Collections.Generic;

    public class PaginationResult<T> where T : BaseSearchableItem
    {
        public PaginationResult(
            IEnumerable<T> itemsToDisplay,
            int page,
            int totalPages,
            int itemsPerPage,
            int matchingSearchResults,
            bool javascriptSearchSortFilterPaginateEnabled
        )
        {
            ItemsToDisplay = itemsToDisplay;
            Page = page;
            TotalPages = totalPages;
            ItemsPerPage = itemsPerPage;
            MatchingSearchResults = matchingSearchResults;
            JavascriptSearchSortFilterPaginateEnabled = javascriptSearchSortFilterPaginateEnabled;
        }

        public PaginationResult(
            PaginationResult<T> paginationResult
        )
        {
            ItemsToDisplay = paginationResult.ItemsToDisplay;
            Page = paginationResult.Page;
            TotalPages = paginationResult.TotalPages;
            ItemsPerPage = paginationResult.ItemsPerPage;
            MatchingSearchResults = paginationResult.MatchingSearchResults;
            JavascriptSearchSortFilterPaginateEnabled = paginationResult.JavascriptSearchSortFilterPaginateEnabled;
        }

        public IEnumerable<T> ItemsToDisplay { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }

        public int ItemsPerPage { get; set; }

        public int MatchingSearchResults { get; set; }

        public bool JavascriptSearchSortFilterPaginateEnabled { get; set; }

        public ReturnPageQuery GetReturnPageQuery(string? itemIdToReturnTo = null)
        {
            return new ReturnPageQuery(Page, ItemsPerPage, itemIdToReturnTo);
        }
    }
}
