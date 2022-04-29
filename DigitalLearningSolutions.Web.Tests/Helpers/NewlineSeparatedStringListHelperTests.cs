namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class NewlineSeparatedStringListHelperTests
    {
        private const string Item1 = "item1";
        private const string Item2 = "item2";
        private const string ItemList = "item1\r\nitem2";
        private const string ItemListWithWhitespaceInAnswers = "  item1  \r\n   item2   ";
        private const string ItemListWithWhitespaceItems = "item1\r\nitem2\r\n\r\n     ";
        private readonly List<string> items = new List<string> { Item1, Item2 };

        [Test]
        public void RemoveStringFromNewlineSeparatedList_returns_expected_values()
        {
            // When
            var resultString = NewlineSeparatedStringListHelper.RemoveStringFromNewlineSeparatedList(ItemList, 1);

            // Then
            resultString.Should().BeEquivalentTo(Item1);
        }

        [Test]
        public void AddStringToNewlineSeparatedList_returns_expected_values()
        {
            // When
            var resultString = NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(Item1, Item2);

            // Then
            resultString.Should().BeEquivalentTo(ItemList);
        }

        [Test]
        public void AddStringToNewlineSeparatedList_trims_answers_and_returns_expected_values()
        {
            // When
            var resultString = NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(" " + Item1 + " \n", " " + Item2 + " \r");

            // Then
            resultString.Should().BeEquivalentTo(ItemList);
        }

        [Test]
        public void JoinNewlineSeparatedList_returns_expected_values()
        {
            // When
            var resultString = NewlineSeparatedStringListHelper.JoinNewlineSeparatedList(items);

            // Then
            resultString.Should().BeEquivalentTo(ItemList);
        }

        [Test]
        public void SplitNewlineSeparatedList_returns_expected_values()
        {
            // When
            var resultList = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(ItemList);

            // Then
            using (new AssertionScope())
            {
                resultList.Count.Should().Be(2);
                resultList.Should().BeEquivalentTo(items);
            }
        }

        [Test]
        public void SplitNewlineSeparatedList_trims_answers_and_returns_expected_values()
        {
            // When
            var resultList = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(ItemListWithWhitespaceInAnswers);

            // Then
            using (new AssertionScope())
            {
                resultList.Count.Should().Be(2);
                resultList.Should().BeEquivalentTo(items);
            }
        }

        [Test]
        public void RemoveEmptyOptions_removes_expected_entries()
        {
            // When
            var resultString = NewlineSeparatedStringListHelper.RemoveEmptyOptions(ItemListWithWhitespaceItems);

            // Then
            resultString.Should().BeEquivalentTo(ItemList);
        }
    }
}
