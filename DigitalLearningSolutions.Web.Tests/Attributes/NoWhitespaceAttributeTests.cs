namespace DigitalLearningSolutions.Web.Tests.Attributes
{
    using DigitalLearningSolutions.Web.Attributes;
    using FluentAssertions;
    using NUnit.Framework;

    public class NoWhitespaceAttributeTests
    {
        [Test]
        public void Accepts_string_value_without_whitespace()
        {
            // Given
            const string value = "hello";
            var attribute = new NoWhitespaceAttribute();

            // When
            var result = attribute.IsValid(value);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void Accepts_empty_string_value()
        {
            // Given
            var value = string.Empty;
            var attribute = new NoWhitespaceAttribute();

            // When
            var result = attribute.IsValid(value);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void Accepts_null_value()
        {
            // Given
            string? value = null;
            var attribute = new NoWhitespaceAttribute();

            // When
            var result = attribute.IsValid(value);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void Rejects_non_string_value()
        {
            // Given
            const int value = 7;
            var attribute = new NoWhitespaceAttribute();

            // When
            var result = attribute.IsValid(value);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void Rejects_string_value_with_whitespace()
        {
            // Given
            const string value = "good morning";
            var attribute = new NoWhitespaceAttribute();

            // When
            var result = attribute.IsValid(value);

            // Then
            result.Should().BeFalse();
        }
    }
}
