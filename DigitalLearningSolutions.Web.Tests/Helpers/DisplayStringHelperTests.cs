namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class DisplayStringHelperTests
    {
        private const long Gigabyte = 1073741824;

        [Test]
        public void GenerateNumberWithLimitDisplayString_returns_expected_string_with_limit()
        {
            // When
            var result = DisplayStringHelper.GenerateNumberWithLimitDisplayString(1, 5);

            // Then
            result.Should().Be("1 / 5");
        }

        [Test]
        public void GenerateNumberWithLimitDisplayString_returns_expected_string_with_no_limit()
        {
            // When
            var result = DisplayStringHelper.GenerateNumberWithLimitDisplayString(1, -1);

            // Then
            result.Should().Be("1");
        }

        [Test]
        public void GenerateBytesLimitDisplayString_returns_expected_string_for_bytes()
        {
            // When
            var result = DisplayStringHelper.GenerateBytesLimitDisplayString(12, 120);

            // Then
            result.Should().Be("12B / 120B");
        }

        [Test]
        public void GenerateBytesLimitDisplayString_returns_expected_string_for_kilobytes()
        {
            // When
            var result = DisplayStringHelper.GenerateBytesLimitDisplayString(12, 1200);

            // Then
            result.Should().Be("12B / 1.2KiB");
        }

        [Test]
        public void GenerateBytesLimitDisplayString_returns_expected_string_for_gigabytes()
        {
            // When
            var result = DisplayStringHelper.GenerateBytesLimitDisplayString(12, Gigabyte);

            // Then
            result.Should().Be("12B / 1GiB");
        }

        [Test]
        public void GenerateBytesLimitDisplayString_returns_expected_string_when_less_than_next_size()
        {
            // Given
            var bytes = Gigabyte - 10;

            // When
            var result = DisplayStringHelper.GenerateBytesLimitDisplayString(12, bytes);

            // Then
            result.Should().Be("12B / 1024MiB");
        }
    }
}
