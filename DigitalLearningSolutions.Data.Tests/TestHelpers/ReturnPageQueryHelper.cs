namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public static class ReturnPageQueryHelper
    {
        public static ReturnPageQuery GetDefaultReturnPageQuery(
            int pageNumber = 1,
            int itemsPerPage = 10,
            string? searchString = null,
            string? sortBy = null,
            string? sortDirection = null,
            string? itemIdToReturnTo = null
        )
        {
            return new ReturnPageQuery(
                pageNumber,
                itemIdToReturnTo,
                itemsPerPage,
                searchString,
                sortBy,
                sortDirection
            );
        }
    }
}
