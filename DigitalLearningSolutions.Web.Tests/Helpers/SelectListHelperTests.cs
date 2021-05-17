namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class SelectListHelperTests
    {
        [Test]
        public void GetOptionsWithSelectedText_returns_populated_list()
        {
            // Given
            var item1 = "item1";
            var item2 = "item2";
            var inputList = new List<(int id, string name)>
            {
                (1, item1),
                (2, item2)
            };

            // When
            var result = SelectListHelper.GetOptionsWithSelectedText(inputList, item1);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Count().Should().Be(2);

                result.SingleOrDefault(i => i.Value == 1.ToString()).Should().NotBeNull();
                result.SingleOrDefault(i => i.Value == 1.ToString()).Selected.Should().BeTrue();

                result.SingleOrDefault(i => i.Value == 2.ToString()).Should().NotBeNull();
                result.SingleOrDefault(i => i.Value == 2.ToString()).Selected.Should().BeFalse();
            }
        }

        [Test]
        public void GetOptionsWithSelectedValue_returns_populated_list()
        {
            // Given
            var item1 = "item1";
            var item2 = "item2";
            var inputList = new List<(int id, string name)>
            {
                (1, item1),
                (2, item2)
            };

            // When
            var result = SelectListHelper.GetOptionsWithSelectedValue(inputList, 1);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Count().Should().Be(2);

                result.SingleOrDefault(i => i.Value == 1.ToString()).Should().NotBeNull();
                result.SingleOrDefault(i => i.Value == 1.ToString()).Selected.Should().BeTrue();

                result.SingleOrDefault(i => i.Value == 2.ToString()).Should().NotBeNull();
                result.SingleOrDefault(i => i.Value == 2.ToString()).Selected.Should().BeFalse();
            }
        }

        [Test]
        public void GetOptionsWithSelectedValue_with_list_of_strings_returns_populated_list()
        {
            // Given
            var item1 = "item1";
            var item2 = "item2";
            var inputList = new List<string>
            {
                item1,
                item2
            };

            // When
            var result = SelectListHelper.GetOptionsWithSelectedValue(inputList, item1);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Count().Should().Be(2);

                result.SingleOrDefault(i => i.Value == item1).Should().NotBeNull();
                result.SingleOrDefault(i => i.Value == item1).Selected.Should().BeTrue();

                result.SingleOrDefault(i => i.Value == item2).Should().NotBeNull();
                result.SingleOrDefault(i => i.Value == item2).Selected.Should().BeFalse();
            }
        }
    }
}
