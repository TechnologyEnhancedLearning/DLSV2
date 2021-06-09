namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class GenericSortingHelperTests
    {
        private readonly IQueryable<SortableItem> inputItems = new[] { new SortableItem("b"), new SortableItem("a"), new SortableItem("c") }.AsQueryable();
        private readonly IQueryable<SortableItem> ascendingExpectedItems = new[] { new SortableItem("a"), new SortableItem("b"), new SortableItem("c") }.AsQueryable();
        private readonly IQueryable<SortableItem> descendingExpectedItems = new[] { new SortableItem("c"), new SortableItem("b"), new SortableItem("a") }.AsQueryable();

        [Test]
        public void SortAllItems_by_ascending_returns_item_in_expected_order()
        {
            // When
            var result = GenericSortingHelper.SortAllItems(inputItems, nameof(SortableItem.Name), "Ascending");

            // Then
            result.Should().BeEquivalentTo(ascendingExpectedItems);
        }

        [Test]
        public void SortAllItems_by_descending_returns_item_in_expected_order()
        {
            // When
            var result = GenericSortingHelper.SortAllItems(inputItems, nameof(SortableItem.Name), "Descending");

            // Then
            result.Should().BeEquivalentTo(descendingExpectedItems);
        }

        private class SortableItem : BaseSearchableItem
        {
            public SortableItem(string name)
            {
                Name = name;
            }

            public string Name { get; set; }

            public override string SearchableName
            {
                get => SearchableNameValue ?? Name;
                set => SearchableNameValue = value;
            }
        }
    }
}
