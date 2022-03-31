namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Web;

    public static class ReturnPageQueryHelper
    {
        // pageNumber=${this.page.toString()}&searchString=${searchString}&sortBy=${sortBy}&sortDirection=${sortDirection}&itemsPerPage=${itemsPerPage}&javascriptItemIdToScrollToOnReturn=${searchableElement.element.id}
        public static Dictionary<string, string> GetRouteDataForBackLinkFromReturnPageQuery(string? returnPageQuery)
        {
            if (returnPageQuery == null)
            {
                return new Dictionary<string, string>();
            }

            var nameValueCollection = HttpUtility.ParseQueryString(returnPageQuery);
            if (!string.IsNullOrWhiteSpace(nameValueCollection["searchString"]))
            {
                return new Dictionary<string, string> { { "page", "1" } };
            }

            return new Dictionary<string, string>
            {
                { "page", nameValueCollection["pageNumber"] },
                { "itemsPerPage", nameValueCollection["itemsPerPage"] },
                { "sortBy", nameValueCollection["sortBy"] },
                { "sortDirection", nameValueCollection["sortDirection"] },
                { "javascriptItemIdToScrollTo", nameValueCollection["javascriptItemIdToScrollToOnReturn"] }
            };
        }
    }
}
