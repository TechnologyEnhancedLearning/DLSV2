namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{
    using System.Collections.Generic;

    public class PaginateResult<T> where T : BaseSearchableItem
    {
        public PaginateResult(
            IEnumerable<T> itemsToDisplay,
            PaginationOptions paginationOptions,
            int totalPages,
            int matchingSearchResults
        )
        {
            ItemsToDisplay = itemsToDisplay;
            Page = paginationOptions.PageNumber;
            TotalPages = totalPages;
            ItemsPerPage = paginationOptions.ItemsPerPage;
            MatchingSearchResults = matchingSearchResults;
        }

        public PaginateResult(
            IEnumerable<T> itemsToDisplay,
            int page,
            int totalPages,
            int itemsPerPage,
            int matchingSearchResults
        )
        {
            ItemsToDisplay = itemsToDisplay;
            Page = page;
            TotalPages = totalPages;
            ItemsPerPage = itemsPerPage;
            MatchingSearchResults = matchingSearchResults;
        }

        public PaginateResult(
            PaginateResult<T> paginateResult
        )
        {
            ItemsToDisplay = paginateResult.ItemsToDisplay;
            Page = paginateResult.Page;
            TotalPages = paginateResult.TotalPages;
            ItemsPerPage = paginateResult.ItemsPerPage;
            MatchingSearchResults = paginateResult.MatchingSearchResults;
        }

        public IEnumerable<T> ItemsToDisplay { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }

        public int ItemsPerPage { get; set; }

        public int MatchingSearchResults { get; set; }
    }
}
