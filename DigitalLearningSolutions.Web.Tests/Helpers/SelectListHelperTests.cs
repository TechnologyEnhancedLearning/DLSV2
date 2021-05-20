﻿namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class SelectListHelperTests
    {
        private const string Item1 = "item1";
        private const string Item2 = "item2";

        [Test]
        public void GetOptionsWithSelectedText_returns_populated_list()
        {
            // Given
            var inputList = new List<(int id, string name)>
            {
                (1, Item1),
                (2, Item2)
            };

            // When
            var result = SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(inputList, Item1);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Count().Should().Be(2);

                result.ElementAt(0).Value.Should().Be("1");
                result.ElementAt(0).Text.Should().Be(Item1);
                result.ElementAt(0).Selected.Should().BeTrue();

                result.ElementAt(1).Value.Should().Be("2");
                result.ElementAt(1).Text.Should().Be(Item2);
                result.ElementAt(1).Selected.Should().BeFalse();
            }
        }

        [Test]
        public void GetOptionsWithSelectedValue_returns_populated_list()
        {
            // Given
            var inputList = new List<(int id, string name)>
            {
                (1, Item1),
                (2, Item2)
            };

            // When
            var result = SelectListHelper.MapOptionsToSelectListItemsWithSelectedValue(inputList, 1);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Count().Should().Be(2);

                result.ElementAt(0).Value.Should().Be("1");
                result.ElementAt(0).Text.Should().Be(Item1);
                result.ElementAt(0).Selected.Should().BeTrue();

                result.ElementAt(1).Value.Should().Be("2");
                result.ElementAt(1).Text.Should().Be(Item2);
                result.ElementAt(1).Selected.Should().BeFalse();
            }
        }

        [Test]
        public void GetOptionsWithSelectedValue_with_list_of_strings_returns_populated_list()
        {
            // Given
            var inputList = new List<string>
            {
                Item1,
                Item2
            };

            // When
            var result = SelectListHelper.MapOptionsToSelectListItemsWithSelectedValue(inputList, Item1);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Count().Should().Be(2);

                result.ElementAt(0).Value.Should().Be(Item1);
                result.ElementAt(0).Text.Should().Be(Item1);
                result.ElementAt(0).Selected.Should().BeTrue();

                result.ElementAt(1).Value.Should().Be(Item2);
                result.ElementAt(1).Text.Should().Be(Item2);
                result.ElementAt(1).Selected.Should().BeFalse();
            }
        }
    }
}
