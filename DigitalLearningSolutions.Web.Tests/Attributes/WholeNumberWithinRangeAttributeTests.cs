namespace DigitalLearningSolutions.Web.Tests.Attributes
{
    using DigitalLearningSolutions.Web.Attributes;
    using FluentAssertions;
    using NUnit.Framework;

    public class WholeNumberWithinRangeAttributeTests
    {
        [Test]
        public void Accepts_number_within_range()
        {
            // Given
            const string value = "1";
            var attribute = new WholeNumberWithinRangeAttribute(0, 10, "Test");

            // When
            var result = attribute.IsValid(value);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void Rejects_number_below_lower_bound()
        {
            // Given
            const string value = "-1";
            var attribute = new WholeNumberWithinRangeAttribute(0, 10, "Test");

            // When
            var result = attribute.IsValid(value);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void Rejects_number_above_upper_bound()
        {
            // Given
            const string value = "11";
            var attribute = new WholeNumberWithinRangeAttribute(0, 10, "Test");

            // When
            var result = attribute.IsValid(value);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void Rejects_non_whole_number()
        {
            // Given
            const string value = "1.1";
            var attribute = new WholeNumberWithinRangeAttribute(0, 10, "Test");

            // When
            var result = attribute.IsValid(value);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void Rejects_value_with_alphabetic_characters()
        {
            // Given
            const string value = "abc";
            var attribute = new WholeNumberWithinRangeAttribute(0, 10, "Test");

            // When
            var result = attribute.IsValid(value);

            // Then
            result.Should().BeFalse();
        }
    }
}
