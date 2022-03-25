namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{
    public class PaginationOptions
    {
        public const int DefaultItemsPerPage = 10;

        public PaginationOptions(int pageNumber, int? itemsPerPage = null)
        {
            PageNumber = pageNumber;
            ItemsPerPage = itemsPerPage ?? DefaultItemsPerPage;
        }

        public int PageNumber { get; set; }

        public int ItemsPerPage { get; set; }
    }
}
