namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{
    using System.Collections.Generic;
    using System.Web;

    public readonly struct ReturnPageQuery
    {
        public ReturnPageQuery(string returnPageQuery)
        {
            var nameValueCollection = HttpUtility.ParseQueryString(returnPageQuery);

            PageNumber = int.Parse(nameValueCollection["pageNumber"]);
            SearchString = nameValueCollection["searchString"];
            SortBy = nameValueCollection["sortBy"];
            SortDirection = nameValueCollection["sortDirection"];
            ItemsPerPage = nameValueCollection["itemsPerPage"] != null
                ? int.Parse(nameValueCollection["itemsPerPage"])
                : (int?)null;
            ItemIdToReturnTo = nameValueCollection["itemIdToScrollToOnReturn"];
        }

        public ReturnPageQuery(
            int page,
            int itemsPerPage,
            string? searchString,
            string? sortBy,
            string? sortDirection,
            string? itemIdToReturnTo
        )
        {
            PageNumber = page;
            SearchString = searchString;
            SortBy = sortBy;
            SortDirection = sortDirection;
            ItemsPerPage = itemsPerPage;
            ItemIdToReturnTo = itemIdToReturnTo;
        }

        public ReturnPageQuery(int pageNumber, string? itemIdToReturnTo)
        {
            PageNumber = pageNumber;
            ItemIdToReturnTo = itemIdToReturnTo;
            SearchString = null;
            SortBy = null;
            SortDirection = null;
            ItemsPerPage = null;
        }

        public int PageNumber { get; }

        public string? SearchString { get; }

        public string? SortBy { get; }

        public string? SortDirection { get; }

        public int? ItemsPerPage { get; }

        public string? ItemIdToReturnTo { get; }

        public Dictionary<string, string> ToRouteDataDictionary()
        {
            var dict = new Dictionary<string, string> { { "page", PageNumber.ToString() } };

            if (SearchString != null)
            {
                dict.Add("searchString", SearchString);
            }

            if (SortBy != null)
            {
                dict.Add("sortBy", SortBy);
            }

            if (SortDirection != null)
            {
                dict.Add("sortDirection", SortDirection);
            }

            if (ItemsPerPage != null)
            {
                dict.Add("itemsPerPage", ItemsPerPage.Value.ToString());
            }

            return dict;
        }

        public override string ToString()
        {
            return
                $"pageNumber={PageNumber}&searchString={SearchString}&sortBy={SortBy}&sortDirection={SortDirection}&itemsPerPage={ItemsPerPage}&itemIdToScrollToOnReturn={ItemIdToReturnTo}";
        }
    }
}
