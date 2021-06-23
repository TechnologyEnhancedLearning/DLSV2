namespace DigitalLearningSolutions.Web.Tests.Extensions
{
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class QueryableExtensionsTests
    {
        [Test]
        public void SortAllItems_by_name_ascending_returns_item_in_expected_order()
        {
            // Given
            var inputItems = QueryableHelper.GetListOfSortableItems("b", 2, "a", 1, "c", 3);
            var expectedItems = QueryableHelper.GetListOfSortableItems("a", 1, "b", 2, "c", 3);

            // When
            var result = inputItems.OrderBy("Name");

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void SortAllItems_by_name_descending_returns_item_in_expected_order()
        {
            // Given
            var inputItems = QueryableHelper.GetListOfSortableItems("b", 2, "a", 1, "c", 3);
            var expectedItems = QueryableHelper.GetListOfSortableItems("c", 3, "b", 2, "a", 1);

            // When
            var result = inputItems.OrderByDescending("Name");

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void SortAllItems_by_number_ascending_returns_item_in_expected_order()
        {
            // Given
            var inputItems = QueryableHelper.GetListOfSortableItems("b", 2, "a", 1, "c", 3);
            var expectedItems = QueryableHelper.GetListOfSortableItems("a", 1, "b", 2, "c", 3);

            // When
            var result = inputItems.OrderBy("Number");

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void SortAllItems_by_number_descending_returns_item_in_expected_order()
        {
            // Given
            var inputItems = QueryableHelper.GetListOfSortableItems("b", 2, "a", 1, "c", 3);
            var expectedItems = QueryableHelper.GetListOfSortableItems("c", 3, "b", 2, "a", 1);

            // When
            var result = inputItems.OrderByDescending("Number");

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void SortAllItems_by_name_then_number_ascending_returns_item_in_expected_order()
        {
            // Given
            var inputItems = QueryableHelper.GetListOfSortableItems("b", 2, "a", 1, "a", 3);
            var expectedItems = QueryableHelper.GetListOfSortableItems("a", 1, "a", 3, "b", 2);

            // When
            var result = inputItems.OrderBy("Name").ThenBy("Number"); 

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Test]
        public void SortAllItems_by_name_then_number_descending_returns_item_in_expected_order()
        {
            // Given
            var inputItems = QueryableHelper.GetListOfSortableItems("b", 2, "a", 1, "a", 3);
            var expectedItems = QueryableHelper.GetListOfSortableItems("b", 2, "a", 3, "a", 1);

            // When
            var result = inputItems.OrderByDescending("Name").ThenByDescending("Number");

            // Then
            result.Should().BeEquivalentTo(expectedItems);
        }
    }
}
