namespace DigitalLearningSolutions.Web.Tests.Attributes
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Attributes;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class WholeNumberWithinInclusiveRangeAttributeTests
    {
        private static IEnumerable<TestCaseData> WholeNumberWithinInclusiveRangeAttributeTestData
        {
            get
            {
                yield return new TestCaseData("1", 0, 10, true).SetName("Accepts_number_within_range");
                yield return new TestCaseData("0", 0, 10, true).SetName("Accepts_number_equal_to_lower_bound");
                yield return new TestCaseData("10", 0, 10, true).SetName("Accepts_number_equal_to_upper_bound");
                yield return new TestCaseData("-1", 0, 10, false).SetName("Rejects_number_less_than_lower_bound");
                yield return new TestCaseData("11", 0, 10, false).SetName("Rejects_number_greater_than_upper_bound");
                yield return new TestCaseData("1.1", 0, 10, false).SetName("Rejects_non_integer");
                yield return new TestCaseData("abc", 0, 10, false).SetName("Rejects_input_with_alphabetic_characters");
            }
        }

        [Test]
        [TestCaseSource(
            typeof(WholeNumberWithinInclusiveRangeAttributeTests),
            nameof(WholeNumberWithinInclusiveRangeAttributeTestData)
        )]
        public void Accepts_valid_inputs_and_rejects_invalid_inputs(
            string input,
            int lowerBound,
            int upperBound,
            bool expectedResult
        )
        {
            // Given
            var attribute = new WholeNumberWithinInclusiveRangeAttribute(lowerBound, upperBound, "Test");

            // When
            var result = attribute.IsValid(input);

            // Then
            result.Should().Be(expectedResult);
        }
    }
}
