namespace DigitalLearningSolutions.Data.Tests.TestHelpers
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
    }
}
