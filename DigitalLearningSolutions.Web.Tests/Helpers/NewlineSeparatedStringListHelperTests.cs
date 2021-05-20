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
        private readonly List<string> items = new List<string> { Item1, Item2 };

        public void Setup()
        {

        }

        [Test]
        public void RemoveStringFromNewlineSeparatedList_returns_expected_values()
        {
            // When
            var (resultString, resultList) = NewlineSeparatedStringListHelper.RemoveStringFromNewlineSeparatedList(ItemList, 1);

            // Then
            using (new AssertionScope())
            {
                resultList.Count.Should().Be(1);
                resultString.Should().BeEquivalentTo(Item1);
            }
        }

        [Test]
        public void AddStringToNewlineSeparatedList_returns_expected_values()
        {
            // When
            var (resultString, resultList) = NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(Item1, Item2);

            // Then
            using (new AssertionScope())
            {
                resultList.Count.Should().Be(2);
                resultList.Should().BeEquivalentTo(items);
                resultString.Should().BeEquivalentTo(ItemList);
            }
        }

        [Test]
        public void JoinNewlineSeparatedList_returns_expected_values()
        {
            // When
            var resultString = NewlineSeparatedStringListHelper.JoinNewlineSeparatedList(items);

            // Then
            using (new AssertionScope())
            {
                resultString.Should().BeEquivalentTo(ItemList);
            }
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
    }
}
