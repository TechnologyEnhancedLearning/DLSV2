namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{
    public class SearchSortFilterAndPaginateOptions
    {
        public SearchSortFilterAndPaginateOptions(
            SearchOptions? searchOptions,
            SortOptions? sortOptions,
            FilterOptions? filterOptions,
            PaginationOptions? paginationOptions
        )
        {
            SearchOptions = searchOptions;
            SortOptions = sortOptions;
            FilterOptions = filterOptions;
            PaginationOptions = paginationOptions;
        }

        public SearchOptions? SearchOptions { get; set; }
        public SortOptions? SortOptions { get; set; }
        public FilterOptions? FilterOptions { get; set; }
        public PaginationOptions? PaginationOptions { get; set; }
    }
}
