namespace DigitalLearningSolutions.Data.Tests.Extensions
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class QueryableExtensionsTests
    {
        private static readonly SortableItem ItemA1 = new SortableItem("a", 1);
        private static readonly SortableItem ItemA3 = new SortableItem("a", 3);
        private static readonly SortableItem ItemB2 = new SortableItem("b", 2);
        private static readonly SortableItem ItemB3 = new SortableItem("b", 3);
        private static readonly SortableItem ItemC3 = new SortableItem("c", 3);
        private static readonly IQueryable<SortableItem> InputItems = new[] { ItemB2, ItemA1, ItemC3 }.AsQueryable();

        private static readonly IQueryable<SortableItem> ThenByInputItems =
            new[] { ItemB2, ItemA1, ItemA3 }.AsQueryable();

        [Test]
        public void SortAllItems_by_name_ascending_returns_item_in_expected_order()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemB2, ItemC3 }.AsQueryable();

            // When
            var result = InputItems.OrderBy("Name");

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void SortAllItems_by_name_descending_returns_item_in_expected_order()
        {
            // Given
            var expectedItems = new[] { ItemC3, ItemB2, ItemA1 }.AsQueryable();

            // When
            var result = InputItems.OrderByDescending("Name");

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void SortAllItems_by_number_ascending_returns_item_in_expected_order()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemB2, ItemC3 }.AsQueryable();

            // When
            var result = InputItems.OrderBy("Number");

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void SortAllItems_by_number_descending_returns_item_in_expected_order()
        {
            // Given
            var expectedItems = new[] { ItemC3, ItemB2, ItemA1 }.AsQueryable();

            // When
            var result = InputItems.OrderByDescending("Number");

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void SortAllItems_by_name_then_number_ascending_returns_item_in_expected_order()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemA3, ItemB2 }.AsQueryable();

            // When
            var result = ThenByInputItems.OrderBy("Name").ThenBy("Number");

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void SortAllItems_by_name_then_number_descending_returns_item_in_expected_order()
        {
            // Given
            var expectedItems = new[] { ItemB2, ItemA3, ItemA1 }.AsQueryable();

            // When
            var result = ThenByInputItems.OrderByDescending("Name").ThenByDescending("Number");

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void Where_returns_expected_items_for_string_property()
        {
            // Given
            var expectedItems = new[] { ItemA1, ItemA3 }.AsQueryable();

            // When
            var result = ThenByInputItems.Where("Name", "a");

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void Where_returns_expected_items_for_int_property()
        {
            // Given
            var inputItems = new[] { ItemA3, ItemB3, ItemA1 }.AsQueryable();
            var expectedItems = new[] { ItemA3, ItemB3 }.AsQueryable();

            // When
            var result = inputItems.Where("Number", 3);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }
    }
}
