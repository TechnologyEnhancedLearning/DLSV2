namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{

    /// <summary>
    /// Defines the options to be used by the <see cref="DigitalLearningSolutions.Web.Services.SearchSortFilterPaginateService"/>.
    /// When SearchOptions, SortOptions or FilterOptions is null, that portion of the functionality
    /// will be turned off. PaginationOptions works slightly differently, but should still be set
    /// to null on pages where there is no pagination. In this case, the service will return a
    /// result with a single page containing all the items.
    /// </summary>
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
