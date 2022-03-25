namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class SearchSortFilterPaginateServiceTests
    {
        private readonly IEnumerable<SortableItem> allItems = new[]
        {
            new SortableItem("A", 10),
            new SortableItem("B", 9),
            new SortableItem("C", 8),
            new SortableItem("D", 7),
            new SortableItem("E", 6),
            new SortableItem("F", 5),
            new SortableItem("G", 4),
            new SortableItem("H", 3),
            new SortableItem("I", 2),
            new SortableItem("J", 1),
        };

        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private IConfiguration configuration = null!;

        [SetUp]
        public void Setup()
        {
            configuration = A.Fake<IConfiguration>();
            searchSortFilterPaginateService = new SearchSortFilterPaginateService(configuration);
            A.CallTo(() => configuration["JavascriptSearchSortFilterPaginateItemLimit"]).Returns("250");
        }

        [Test]
        public void SearchFilterSortAndPaginate_with_search_only_returns_expected_result()
        {
            // Given
            var options = new SearchSortFilterAndPaginateOptions(new SearchOptions("A"), null, null, null);

            // When
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(allItems, options);

            // Then
            var expectedResult = new SearchSortFilterPaginationResult<SortableItem>(
                new PaginationResult<SortableItem>(new[] { new SortableItem("A", 10) }, 1, 1, int.MaxValue, 1, true),
                "A",
                null,
                null,
                null
            );
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void SearchFilterSortAndPaginate_with_sort_only_returns_expected_result()
        {
            // Given
            var options = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions("Number", GenericSortingHelper.Ascending),
                null,
                null
            );

            // When
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(allItems, options);

            // Then
            var expectedResult = new SearchSortFilterPaginationResult<SortableItem>(
                new PaginationResult<SortableItem>(allItems.OrderBy(x => x.Number), 1, 1, int.MaxValue, 10, true),
                null,
                "Number",
                GenericSortingHelper.Ascending,
                null
            );
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void SearchFilterSortAndPaginate_with_filter_only_returns_expected_result()
        {
            // Given
            var availableFilters = new List<FilterModel>
            {
                new FilterModel(
                    "Name",
                    "Name",
                    new List<FilterOptionModel> { new FilterOptionModel("A", "Name|Name|A", FilterStatus.Default) }
                ),
            };
            var options = new SearchSortFilterAndPaginateOptions(
                null,
                null,
                new FilterOptions("Name|Name|A", availableFilters),
                null
            );

            // When
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(allItems, options);

            // Then
            var expectedResult = new SearchSortFilterPaginationResult<SortableItem>(
                new PaginationResult<SortableItem>(new[] { new SortableItem("A", 10) }, 1, 1, int.MaxValue, 1, true),
                null,
                null,
                null,
                "Name|Name|A"
            );
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void SearchFilterSortAndPaginate_with_invalid_filter_returns_result_with_default_filter_applied()
        {
            // Given
            var availableFilters = new List<FilterModel>
            {
                new FilterModel(
                    "Name",
                    "Name",
                    new List<FilterOptionModel> { new FilterOptionModel("A", "Name|Name|A", FilterStatus.Default) }
                ),
            };
            var options = new SearchSortFilterAndPaginateOptions(
                null,
                null,
                new FilterOptions("Name|INVALID|A", availableFilters, "Name|Name|A"),
                null
            );

            // When
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(allItems, options);

            // Then
            var expectedResult = new SearchSortFilterPaginationResult<SortableItem>(
                new PaginationResult<SortableItem>(new[] { new SortableItem("A", 10) }, 1, 1, int.MaxValue, 1, true),
                null,
                null,
                null,
                "Name|Name|A"
            );
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void SearchFilterSortAndPaginate_with_invalid_filter_and_no_default_returns_unfiltered_result()
        {
            // Given
            var availableFilters = new List<FilterModel>
            {
                new FilterModel(
                    "Name",
                    "Name",
                    new List<FilterOptionModel> { new FilterOptionModel("A", "Name|Name|A", FilterStatus.Default) }
                ),
            };
            var options = new SearchSortFilterAndPaginateOptions(
                null,
                null,
                new FilterOptions("Name|INVALID|A", availableFilters),
                null
            );

            // When
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(allItems, options);

            // Then
            var expectedResult = new SearchSortFilterPaginationResult<SortableItem>(
                new PaginationResult<SortableItem>(allItems, 1, 1, int.MaxValue, 10, true),
                null,
                null,
                null,
                null
            );
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void SearchFilterSortAndPaginate_with_paginate_only_returns_expected_first_page()
        {
            // Given
            var options = new SearchSortFilterAndPaginateOptions(
                null,
                null,
                null,
                new PaginationOptions(1, 8)
            );

            // When
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(allItems, options);

            // Then
            var expectedResult = new SearchSortFilterPaginationResult<SortableItem>(
                new PaginationResult<SortableItem>(allItems.Take(8), 1, 2, 8, 10, true),
                null,
                null,
                null,
                null
            );
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void SearchFilterSortAndPaginate_with_paginate_only_returns_expected_second_page()
        {
            // Given
            var options = new SearchSortFilterAndPaginateOptions(
                null,
                null,
                null,
                new PaginationOptions(2, 8)
            );

            // When
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(allItems, options);

            // Then
            var expectedResult = new SearchSortFilterPaginationResult<SortableItem>(
                new PaginationResult<SortableItem>(allItems.Skip(8), 2, 2, 8, 10, true),
                null,
                null,
                null,
                null
            );
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100)]
        public void SearchFilterSortAndPaginate_with_paginate_only_returns_first_page_with_invalid_page_numbers(int pageNumber)
        {
            // Given
            var options = new SearchSortFilterAndPaginateOptions(
                null,
                null,
                null,
                new PaginationOptions(pageNumber, 5)
            );

            // When
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(allItems, options);

            // Then
            var expectedResult = new SearchSortFilterPaginationResult<SortableItem>(
                new PaginationResult<SortableItem>(allItems.Take(5), 1, 2, 5, 10, true),
                null,
                null,
                null,
                null
            );
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void SearchFilterSortAndPaginate_with_low_configured_item_limit_returns_javascript_enabled_false()
        {
            // Given
            var options = new SearchSortFilterAndPaginateOptions(
                null,
                null,
                null,
                new PaginationOptions(1, 8)
            );
            A.CallTo(() => configuration["JavascriptSearchSortFilterPaginateItemLimit"]).Returns("2");

            // When
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(allItems, options);

            // Then
            var expectedResult = new SearchSortFilterPaginationResult<SortableItem>(
                new PaginationResult<SortableItem>(allItems.Take(8), 1, 2, 8, 10, false),
                null,
                null,
                null,
                null
            );
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
