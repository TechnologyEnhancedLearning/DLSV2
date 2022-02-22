namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class FilteringHelperTests
    {
        private const string FilterByAlphaBravoCharlie = "Group|Name|Alpha╡Group|Name|Bravo╡Group|Name|Charlie";
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
            var filterBy = "Name|Name|a";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterBy);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void FilterItems_returns_expected_items_with_multiple_filters()
        {
            // Given
            var expectedItems = new[] { ItemA1 }.AsQueryable();
            var filterBy = "Name|Name|a╡Number|Number|1";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterBy);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void FilterItems_with_filters_in_same_group_returns_union_of_filters()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemA3, ItemB2 }.AsQueryable();
            var filterBy = "Group|Name|a╡Group|Name|b";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterBy);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void FilterItems_with_filters_in_same_group_returns_no_duplicates()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemA3 }.AsQueryable();
            var filterBy = "Group|Name|a╡Group|Name|a";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterBy);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void FilterItems_with_some_filters_in_same_group_returns_expected_items()
        {
            // Given
            var expectedItems = new[] { ItemA3 }.AsQueryable();
            var filterBy = "Group|Name|a╡Group|Name|b╡Number|Number|3";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterBy);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void AddNewFilterToFilterBy_doesnt_append_with_null_new_filter()
        {
            // When
            var result = FilteringHelper.AddNewFilterToFilterBy("Test", null);

            // Then
            result.Should().Be("Test");
        }

        [TestCase("Test", "Test")]
        [TestCase(FilterByAlphaBravoCharlie, "Group|Name|Bravo")]
        public void AddNewFilterToFilterBy_doesnt_append_with_new_filter_already_in_filterBy(
            string filterBy,
            string newFilterValue
        )
        {
            // When
            var result = FilteringHelper.AddNewFilterToFilterBy(filterBy, newFilterValue);

            // Then
            result.Should().Be(filterBy);
        }

        [Test]
        public void AddNewFilterToFilterBy_returns_new_filter_if_filterBy_is_null()
        {
            // When
            var result = FilteringHelper.AddNewFilterToFilterBy(null, "Test");

            // Then
            result.Should().Be("Test");
        }

        [Test]
        public void AddNewFilterToFilterBy_appends_new_filter()
        {
            // When
            var result = FilteringHelper.AddNewFilterToFilterBy("Test", "Filter");

            // Then
            result.Should().Be("Test╡Filter");
        }

        [TestCase(FilterByAlphaBravoCharlie, "Group|Name|A")]
        [TestCase(FilterByAlphaBravoCharlie, "p|Name|B")]
        [TestCase(FilterByAlphaBravoCharlie, "p|Name|Charlie")]
        public void AddNewFilterToFilterBy_appends_new_filter_even_if_substring(string filterBy, string newFilterValue)
        {
            // When
            var result = FilteringHelper.AddNewFilterToFilterBy(filterBy, newFilterValue);

            // Then
            result.Should().Be($"{filterBy}╡{newFilterValue}");
        }

        [Test]
        public void GetFilterBy_with_no_parameters_returns_cookie_value()
        {
            // Given
            const string CookieValue = "Cookie Value";
            A.CallTo(() => httpRequest.Cookies.ContainsKey(CookieName)).Returns(true);
            A.CallTo(() => httpRequest.Cookies[CookieName]).Returns(CookieValue);

            // When
            var result = FilteringHelper.GetFilterBy(null, null, httpRequest, CookieName);

            // Then
            result.Should().Be(CookieValue);
        }

        [Test]
        public void GetFilterBy_with_no_parameters_and_no_cookies_returns_defaultFilterValue()
        {
            // When
            var result = FilteringHelper.GetFilterBy(null, null, httpRequest, CookieName, "default-filter");

            // Then
            result.Should().Be("default-filter");
        }

        [Test]
        public void GetFilterBy_with_CLEAR_filterBy_and_no_filterValue_returns_null()
        {
            // When
            var result = FilteringHelper.GetFilterBy("CLEAR", null, httpRequest, CookieName);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetFilterBy_with_CLEAR_filterBy_and_set_filterValue_returns_filterValue()
        {
            // When
            var result = FilteringHelper.GetFilterBy("CLEAR", "filter-value", httpRequest, CookieName);

            // Then
            result.Should().Be("filter-value");
        }

        [Test]
        public void GetFilterBy_with_filterBy_and_filterValue_returns_combined_filter_by()
        {
            // When
            var result = FilteringHelper.GetFilterBy("filter-by", "filter-value", httpRequest, CookieName);

            // Then
            result.Should().Be("filter-by╡filter-value");
        }
    }
}
