namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class GenericSortingHelperTests
    {
        private readonly SortableItem[] inputItems = new[] { new SortableItem("b"), new SortableItem("a"), new SortableItem("c") };
        private readonly SortableItem[] ascendingExpectedItems = new[] { new SortableItem("a"), new SortableItem("b"), new SortableItem("c") };
        private readonly SortableItem[] descendingExpectedItems = new[] { new SortableItem("c"), new SortableItem("b"), new SortableItem("a") };

        [Test]
        public void OrderBy_returns_item_in_expected_order()
        {
            // When
            var result = inputItems.OrderBy(nameof(SortableItem.Name)).ToArray();

            // Then
            result.Should().BeEquivalentTo(ascendingExpectedItems.AsQueryable());
        }

        [Test]
        public void OrderByDescending_returns_item_in_expected_order()
        {
            // When
            var result = inputItems.OrderByDescending(nameof(SortableItem.Name)).ToArray();

            // Then
            result.Should().BeEquivalentTo(descendingExpectedItems.AsQueryable());
        }

        [Test]
        public void SortAllItems_by_ascending_returns_item_in_expected_order()
        {
            // When
            var result = GenericSortingHelper.SortAllItems(inputItems, nameof(SortableItem.Name), "Ascending");

            // Then
            result.Should().BeEquivalentTo(ascendingExpectedItems.AsQueryable());
        }

        [Test]
        public void SortAllItems_by_descending_returns_item_in_expected_order()
        {
            // When
            var result = GenericSortingHelper.SortAllItems(inputItems, nameof(SortableItem.Name), "Descending");

            // Then
            result.Should().BeEquivalentTo(descendingExpectedItems.AsQueryable());
        }

        private class SortableItem
        {
            public SortableItem(string name)
            {
                Name = name;
            }

            public string Name { get; set; }
        }
    }
}
