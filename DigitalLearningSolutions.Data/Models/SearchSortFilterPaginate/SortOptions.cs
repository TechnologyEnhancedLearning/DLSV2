namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{
    public class SortOptions
    {
        public SortOptions(string sortBy, string sortDirection)
        {
            SortBy = sortBy;
            SortDirection = sortDirection;
        }

        public string SortBy { get; set; }

        public string SortDirection { get; set; }
    }
}
