namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class FilteringHelperTests
    {
        private const string FilterStringAlphaBravoCharlie = "Group|Name|Alpha╡Group|Name|Bravo╡Group|Name|Charlie";
        private const string CookieName = "TestFilterCookie";
        private static readonly SortableItem ItemA1 = new SortableItem("a", 1);
        private static readonly SortableItem ItemA3 = new SortableItem("a", 3);
        private static readonly SortableItem ItemB2 = new SortableItem("b", 2);
        private static readonly IQueryable<SortableItem> InputItems = new[] { ItemA1, ItemA3, ItemB2 }.AsQueryable();
        private HttpRequest httpRequest = null!;

        [SetUp]
        public void Setup()
        {
            httpRequest = A.Fake<HttpRequest>();
        }

        [Test]
        public void FilterItems_returns_expected_items_with_single_filter()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemA3 }.AsQueryable();
            const string filterString = "Name|Name|a";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterString);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void FilterItems_returns_expected_items_with_multiple_filters()
        {
            // Given
            var expectedItems = new[] { ItemA1 }.AsQueryable();
            const string filterString = "Name|Name|a╡Number|Number|1";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterString);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void FilterItems_with_filters_in_same_group_returns_union_of_filters()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemA3, ItemB2 }.AsQueryable();
            const string filterString = "Group|Name|a╡Group|Name|b";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterString);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void FilterItems_with_filters_in_same_group_returns_no_duplicates()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemA3 }.AsQueryable();
            const string filterString = "Group|Name|a╡Group|Name|a";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterString);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void FilterItems_with_some_filters_in_same_group_returns_expected_items()
        {
            // Given
            var expectedItems = new[] { ItemA3 }.AsQueryable();
            const string filterString = "Group|Name|a╡Group|Name|b╡Number|Number|3";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterString);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void FilterItems_returns_expected_items_with_filter_with_bracket_in_it()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemA3 }.AsQueryable();
            const string filterString = "Name(Field name)|Name|a";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterString);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void AddNewFilterToFilterString_doesnt_append_with_null_new_filter()
        {
            // When
            var result = FilteringHelper.AddNewFilterToFilterString("Test", null);

            // Then
            result.Should().Be("Test");
        }

        [TestCase("Test", "Test")]
        [TestCase(FilterStringAlphaBravoCharlie, "Group|Name|Bravo")]
        public void AddNewFilterToFilterString_doesnt_append_with_new_filter_already_in_filterString(
            string filterString,
            string newFilterToAdd
        )
        {
            // When
            var result = FilteringHelper.AddNewFilterToFilterString(filterString, newFilterToAdd);

            // Then
            result.Should().Be(filterString);
        }

        [Test]
        public void AddNewFilterToFilterString_returns_new_filter_if_existingFilterString_is_null()
        {
            // When
            var result = FilteringHelper.AddNewFilterToFilterString(null, "Test");

            // Then
            result.Should().Be("Test");
        }

        [Test]
        public void AddNewFilterToFilterString_appends_new_filter()
        {
            // When
            var result = FilteringHelper.AddNewFilterToFilterString("Test", "Filter");

            // Then
            result.Should().Be("Test╡Filter");
        }

        [TestCase(FilterStringAlphaBravoCharlie, "Group|Name|A")]
        [TestCase(FilterStringAlphaBravoCharlie, "p|Name|B")]
        [TestCase(FilterStringAlphaBravoCharlie, "p|Name|Charlie")]
        public void AddNewFilterToFilterString_appends_new_filter_even_if_substring(
            string filterString,
            string newFilterToAdd
        )
        {
            // When
            var result = FilteringHelper.AddNewFilterToFilterString(filterString, newFilterToAdd);

            // Then
            result.Should().Be($"{filterString}╡{newFilterToAdd}");
        }

        [Test]
        public void GetFilterString_with_no_parameters_returns_cookie_value()
        {
            // Given
            const string cookieValue = "Cookie Value";
            A.CallTo(() => httpRequest.Cookies.ContainsKey(CookieName)).Returns(true);
            A.CallTo(() => httpRequest.Cookies[CookieName]).Returns(cookieValue);

            // When
            var result = FilteringHelper.GetFilterString(null, null, false, httpRequest, CookieName);

            // Then
            result.Should().Be(cookieValue);
        }

        [Test]
        public void GetFilterString_with_newFilterToAdd_and_cookie_returns_new_filter()
        {
            // Given
            const string cookieValue = "Cookie Value";
            const string newFilter = "newFilter";
            A.CallTo(() => httpRequest.Cookies.ContainsKey(CookieName)).Returns(true);
            A.CallTo(() => httpRequest.Cookies[CookieName]).Returns(cookieValue);

            // When
            var result = FilteringHelper.GetFilterString(null, newFilter, false, httpRequest, CookieName);

            // Then
            result.Should().Be(newFilter);
        }

        [Test]
        public void GetFilterString_with_EmptyFiltersCookieValue_returns_null()
        {
            // Given
            A.CallTo(() => httpRequest.Cookies.ContainsKey(CookieName)).Returns(true);
            A.CallTo(() => httpRequest.Cookies[CookieName]).Returns(FilteringHelper.EmptyFiltersCookieValue);

            // When
            var result = FilteringHelper.GetFilterString(null, null, false, httpRequest, CookieName);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetFilterString_with_no_parameters_and_no_cookies_returns_defaultFilterValue()
        {
            // When
            var result = FilteringHelper.GetFilterString(null, null, false, httpRequest, CookieName, "default-filter");

            // Then
            result.Should().Be("default-filter");
        }

        [Test]
        public void GetFilterString_with_clearFilters_true_returns_null()
        {
            // When
            var result = FilteringHelper.GetFilterString("FilterString", null, true, httpRequest, CookieName);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetFilterString_with_clearFilters_true_does_not_append_new_filter()
        {
            // When
            var result = FilteringHelper.GetFilterString("FilterString", "newFilter", true, httpRequest, CookieName);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetFilterString_with_existingFilterString_and_newFilterToAdd_returns_combined_filter_by()
        {
            // When
            var result = FilteringHelper.GetFilterString(
                "existing-filter-string",
                "filter-value",
                false,
                httpRequest,
                CookieName
            );

            // Then
            result.Should().Be("existing-filter-string╡filter-value");
        }

        [Test]
        public void FilterOrResetFilterToDefault_returns_expected_items_with_valid_filter()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemA3 }.AsQueryable();
            const string filterString = "Name|Name|a";
            var availableFilters = new List<FilterModel>
            {
                new FilterModel(
                    "Name",
                    "Name",
                    new List<FilterOptionModel> { new FilterOptionModel("A", "Name|Name|a", FilterStatus.Default) }
                ),
            };
            var filterOptions = new FilterOptions(filterString, availableFilters);

            // When
            var (resultItems, resultString) = FilteringHelper.FilterOrResetFilterToDefault(InputItems, filterOptions);

            // Then
            resultItems.Should().BeEquivalentTo(expectedItems);
            resultString.Should().BeEquivalentTo(filterString);
        }

        [Test]
        public void FilterOrResetFilterToDefault_returns_expected_default_filtered_items_with_invalid_filter()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemA3 }.AsQueryable();
            const string defaultFilterString = "Name|Name|a";
            const string invalidFilterString = "Name|INVALID|a";
            var availableFilters = new List<FilterModel>
            {
                new FilterModel(
                    "Name",
                    "Name",
                    new List<FilterOptionModel> { new FilterOptionModel("A", "Name|Name|A", FilterStatus.Default) }
                ),
            };
            var filterOptions = new FilterOptions(invalidFilterString, availableFilters, defaultFilterString);

            // When
            var (resultItems, resultString) = FilteringHelper.FilterOrResetFilterToDefault(InputItems, filterOptions);

            // Then
            resultItems.Should().BeEquivalentTo(expectedItems);
            resultString.Should().BeEquivalentTo(defaultFilterString);
        }

        [Test]
        public void FilterOrResetFilterToDefault_returns_expected_unfiltered_items_with_invalid_filter_and_no_default()
        {
            // Given
            const string invalidFilterString = "Name|INVALID|a";
            var availableFilters = new List<FilterModel>
            {
                new FilterModel(
                    "Name",
                    "Name",
                    new List<FilterOptionModel> { new FilterOptionModel("A", "Name|Name|A", FilterStatus.Default) }
                ),
            };
            var filterOptions = new FilterOptions(invalidFilterString, availableFilters);

            // When
            var (resultItems, resultString) = FilteringHelper.FilterOrResetFilterToDefault(InputItems, filterOptions);

            // Then
            resultItems.Should().BeEquivalentTo(InputItems);
            resultString.Should().BeNull();
        }
    }
}
