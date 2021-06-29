namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class FilteringHelperTests
    {
        [Test]
        public void FilterItems_returns_expected_items_with_single_filter()
        {
            // Given
            var inputItems = QueryableHelper.GetListOfSortableItems("a", 1, "a", 3, "b", 2);
            var expectedItems = new[] { new SortableItem("a", 1), new SortableItem("a", 3) }.AsQueryable();
            var filterBy = "Name|a";

            // When
            var result = FilteringHelper.FilterItems(inputItems, filterBy);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void FilterItems_returns_expected_items_with_multiple_filters()
        {
            // Given
            var inputItems = QueryableHelper.GetListOfSortableItems("a", 1, "a", 3, "b", 2);
            var expectedItems = new[] { new SortableItem("a", 1) }.AsQueryable();
            var filterBy = "Name|a\r\nNumber|1";

            // When
            var result = FilteringHelper.FilterItems(inputItems, filterBy);

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }
    }
}
