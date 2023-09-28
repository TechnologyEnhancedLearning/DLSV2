namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;

    public static class SearchSortFilterAndPaginateTestHelper
    {
        public static void GivenACallToSearchSortFilterPaginateServiceReturnsResult<T>(
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            int maxItemsBeforeJavascriptSearchDisabled = 250
        ) where T : BaseSearchableItem
        {
            A.CallTo(
                () => searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                    A<IEnumerable<T>>._,
                    A<SearchSortFilterAndPaginateOptions>._
                )
            ).ReturnsLazily(
                x =>
                {
                    var items = x.Arguments.Get<IEnumerable<T>>("items")?.ToList() ??
                                new List<T>();
                    var options =
                        x.Arguments.Get<SearchSortFilterAndPaginateOptions>("searchSortFilterAndPaginateOptions");
                    return new SearchSortFilterPaginationResult<T>(
                        new PaginationResult<T>(
                            items,
                            options!.PaginationOptions?.PageNumber ?? 1,
                            1,
                            options.PaginationOptions?.ItemsPerPage ?? int.MaxValue,
                            items.Count,
                            items.Count <= maxItemsBeforeJavascriptSearchDisabled
                        ),
                        options.SearchOptions?.SearchString,
                        options.SortOptions?.SortBy,
                        options.SortOptions?.SortDirection,
                        options.FilterOptions?.FilterString
                    );
                }
            );
        }

        public static void GivenACallToPaginateServiceReturnsResult<T>(
            IPaginateService paginateService,
            int searchresultCount,
            string searchString,
            string sortBy,
            string sortDirection
        ) where T : BaseSearchableItem
        {
            A.CallTo(
                () => paginateService.Paginate(
                        A<IEnumerable<T>>._,
                        A<int>._,
                        A<PaginationOptions>._,
                        A<FilterOptions>._,
                        A<string>._,
                        A<string>._,
                        A<string>._
                )
            ).ReturnsLazily(
                x =>
                {
                    var items = x.Arguments.Get<IEnumerable<T>>("items")?.ToList() ??
                                new List<T>();
                    var options =
                        x.Arguments.Get<PaginationOptions>("paginationOptions");
                    var filterOptions =
                       x.Arguments.Get<FilterOptions>("filterOptions");
                    return new SearchSortFilterPaginationResult<T>(
                        new PaginationResult<T>(
                            items,
                            options!.PageNumber,
                            1,
                            options.ItemsPerPage,
                            searchresultCount,
                            false
                        ),
                        searchString,
                        sortBy,
                        sortDirection,
                        filterOptions!.FilterString
                    );
                }
            );
        }
    }
}
