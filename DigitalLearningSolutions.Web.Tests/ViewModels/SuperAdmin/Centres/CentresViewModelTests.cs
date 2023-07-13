namespace DigitalLearningSolutions.Web.Tests.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    public class CentresViewModelTests
    {
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        [SetUp]
        public void Setup()
        {
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
        }

        [Test]
        public void CentresViewModel_default_should_return_first_page_of_centres()
        {
            // Given
            var centres = new List<CentreEntity>
            {
                new CentreEntity { Centre = new Centre{ CentreName = "A" } },
                new CentreEntity { Centre = new Centre{ CentreName = "b" } },
                new CentreEntity { Centre = new Centre{ CentreName = "C" } },
                new CentreEntity { Centre = new Centre{ CentreName = "F" } },
                new CentreEntity { Centre = new Centre{ CentreName = "J" } },
                new CentreEntity { Centre = new Centre{ CentreName = "e" } },
                new CentreEntity { Centre = new Centre{ CentreName = "w" } },
                new CentreEntity { Centre = new Centre{ CentreName = "S" } },
                new CentreEntity { Centre = new Centre{ CentreName = "r" } },
                new CentreEntity { Centre = new Centre{ CentreName = "H" } },
            };

            // When
            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions(GenericSortingHelper.DefaultSortOption, GenericSortingHelper.Ascending),
                null,
                new PaginationOptions(1, 10)
            );

            A.CallTo(() => searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                centres.AsEnumerable(),
                searchSortPaginationOptions
            )).ReturnsLazily(
                x =>
                {
                    var items = centres.AsEnumerable();
                    var options =
                        x.Arguments.Get<SearchSortFilterAndPaginateOptions>("searchSortFilterAndPaginateOptions");
                    return new SearchSortFilterPaginationResult<CentreEntity>(
                        new PaginationResult<CentreEntity>(
                            items,
                            options!.PaginationOptions?.PageNumber ?? 1,
                            1,
                            10,
                            10,
                            false
                        ),
                        options.SearchOptions?.SearchString,
                        options.SortOptions?.SortBy,
                        options.SortOptions?.SortDirection,
                        options.FilterOptions?.FilterString
                    );
                });

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                centres.AsEnumerable(),
                searchSortPaginationOptions
            );
            var model = new CentresViewModel(result);

            // Then
            model.Centres
                .Should().HaveCount(10);
        }
    }
}
