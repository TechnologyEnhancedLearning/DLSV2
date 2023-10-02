﻿namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{
    using System.Collections.Generic;
    using System.Web;
    using DigitalLearningSolutions.Data.ModelBinders;
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(BinderType = typeof(ReturnPageQueryModelBinder))]
    public readonly struct ReturnPageQuery
    {
        public ReturnPageQuery(string returnPageQuery)
        {
            var nameValueCollection = HttpUtility.ParseQueryString(returnPageQuery);
            PageNumber = int.TryParse(nameValueCollection["pageNumber"], out var pageNumberResult) ? pageNumberResult : 0;
            SearchString = nameValueCollection["searchString"];
            SortBy = nameValueCollection["sortBy"];
            SortDirection = nameValueCollection["sortDirection"];
            ItemsPerPage = !string.IsNullOrWhiteSpace(nameValueCollection["itemsPerPage"])
                ?  int.TryParse(nameValueCollection["itemsPerPage"], out var itemsPerPageResult) ? itemsPerPageResult : 0
                : (int?)null;
            ItemIdToReturnTo = nameValueCollection["itemIdToScrollToOnReturn"];
        }

        public ReturnPageQuery(
            int page,
            string? itemIdToReturnTo,
            int? itemsPerPage = null,
            string? searchString = null,
            string? sortBy = null,
            string? sortDirection = null
        )
        {
            PageNumber = page;
            SearchString = searchString;
            SortBy = sortBy;
            SortDirection = sortDirection;
            ItemsPerPage = itemsPerPage;
            ItemIdToReturnTo = itemIdToReturnTo;
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

        public static bool TryGetFromFormData(string formData, out ReturnPageQuery? returnPageQuery)
        {
            try
            {
                returnPageQuery = new ReturnPageQuery(formData);
                return true;
            }
            catch
            {
                returnPageQuery = null;
                return false;
            }
        }
    }
}
