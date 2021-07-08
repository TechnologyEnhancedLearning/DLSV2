namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class FilteringHelperTests
    {
        private static readonly SortableItem ItemA1 = new SortableItem("a", 1);
        private static readonly SortableItem ItemA3 = new SortableItem("a", 3);
        private static readonly SortableItem ItemB2 = new SortableItem("b", 2);
        private static readonly IQueryable<SortableItem> InputItems = new[] { ItemA1, ItemA3, ItemB2 }.AsQueryable();

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
            var filterBy = "Name|Name|a\r\nNumber|Number|1";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterBy);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void FilterItems_with_filters_in_same_category_returns_union_of_filters()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemA3, ItemB2 }.AsQueryable();
            var filterBy = "Category|Name|a\r\nCategory|Name|b";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterBy);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void FilterItems_with_filters_in_same_category_returns_no_duplicates()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemA3 }.AsQueryable();
            var filterBy = "Category|Name|a\r\nCategory|Name|a";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterBy);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void FilterItems_with_some_filters_in_same_category_returns_expected_items()
        {
            // Given
            var expectedItems = new[] { ItemA3 }.AsQueryable();
            var filterBy = "Category|Name|a\r\nCategory|Name|b\r\nNumber|Number|3";

            // When
            var result = FilteringHelper.FilterItems(InputItems, filterBy);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }
    }
}
