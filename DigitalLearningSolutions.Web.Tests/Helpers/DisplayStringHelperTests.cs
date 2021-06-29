namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class DisplayStringHelperTests
    {
        private const long Gibibyte = 1073741824;

        [Test]
        public void GenerateNumberWithLimitDisplayString_returns_expected_string_with_limit()
        {
            // When
            var result = DisplayStringHelper.FormatNumberWithLimit(1, 5);

            // Then
            result.Should().Be("1 / 5");
        }

        [Test]
        public void GenerateNumberWithLimitDisplayString_returns_expected_string_with_no_limit()
        {
            // When
            var result = DisplayStringHelper.FormatNumberWithLimit(1, -1);

            // Then
            result.Should().Be("1");
        }

        [Test]
        public void FormatBytesWithLimit_returns_expected_string_for_bytes()
        {
            // When
            var result = DisplayStringHelper.FormatBytesWithLimit(12, 120);

            // Then
            result.Should().Be("12B / 120B");
        }

        [Test]
        public void FormatBytesWithLimit_returns_expected_string_for_kilobytes()
        {
            // When
            var result = DisplayStringHelper.FormatBytesWithLimit(12, 1200);

            // Then
            result.Should().Be("12B / 1.2KiB");
        }

        [Test]
        public void FormatBytesWithLimit_returns_expected_string_for_gibibytes()
        {
            // When
            var result = DisplayStringHelper.FormatBytesWithLimit(12, Gibibyte);

            // Then
            result.Should().Be("12B / 1GiB");
        }

        [Test]
        public void FormatBytesWithLimit_returns_expected_string_when_less_than_next_size()
        {
            // Given
            var bytes = Gibibyte - 10;

            // When
            var result = DisplayStringHelper.FormatBytesWithLimit(12, bytes);

            // Then
            result.Should().Be("12B / 1024MiB");
        }

        [Test]
        public void FormatBytesWithLimit_throws_exception_with_negative_bytes()
        {
            // When
            Action action = () => DisplayStringHelper.FormatBytesWithLimit(-1, 0);

            // Then
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
