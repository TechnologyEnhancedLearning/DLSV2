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
            int totalItems
        )
        {
            ItemsToDisplay = itemsToDisplay;
            Page = page;
            TotalPages = totalPages;
            ItemsPerPage = itemsPerPage;
            MatchingSearchResults = matchingSearchResults;
            TotalItems = totalItems;
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
            TotalItems = paginationResult.TotalItems;
        }

        public IEnumerable<T> ItemsToDisplay { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }

        public int ItemsPerPage { get; set; }

        public int MatchingSearchResults { get; set; }

        public int TotalItems { get; set; }
    }
}
