namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    class DurationFormattingHelperTests
    {
        [Test]
        public void FormatDuration_should_have_duration_for_0_minutes()
        {
            // Given
            const int duration = 0;

            // When
            var result = DurationFormattingHelper.FormatDuration(duration);

            // Then
            result.Should().Be("0 minutes");
        }

        [Test]
        public void FormatDuration_should_have_duration_for_1_minute()
        {
            // Given
            const int duration = 1;

            // When
            var result = DurationFormattingHelper.FormatDuration(duration);

            // Then
            result.Should().Be("1 minute");
        }

        [Test]
        public void FormatDuration_should_have_duration_for_under_an_hour()
        {
            // Given
            const int duration = 30;

            // When
            var result = DurationFormattingHelper.FormatDuration(duration);

            // Then
            result.Should().Be("30 minutes");
        }

        [Test]
        public void FormatDuration_should_have_duration_for_whole_number_of_hours()
        {
            // Given
            const int duration = 120;

            // When
            var result = DurationFormattingHelper.FormatDuration(duration);

            // Then
            result.Should().Be("2 hours");
        }

        [Test]
        public void FormatDuration_should_have_duration_for_one_hour_one_minute()
        {
            // Given
            const int duration = 61;

            // When
            var result = DurationFormattingHelper.FormatDuration(duration);

            // Then
            result.Should().Be("1 hour 1 minute");
        }


        [Test]
        public void FormatDuration_should_have_duration_for_multiple_hours()
        {
            // Given
            const int duration = 195;

            // When
            var result = DurationFormattingHelper.FormatDuration(duration);

            // Then
            result.Should().Be("3 hours 15 minutes");
        }

        [TestCase(0, "0 minutes")]
        [TestCase(1, "1 minute")]
        [TestCase(30, "30 minutes")]
        [TestCase(120, "2 hours")]
        [TestCase(61, "1 hour 1 minute")]
        [TestCase(195, "3 hours 15 minutes")]
        [TestCase(null, null)]
        public void FormatNullableDuration_should_format_durations(int? duration, string? expectedResult)
        {
            // When
            var result = DurationFormattingHelper.FormatNullableDuration(duration);

            // Then
            result.Should().Be(expectedResult);
        }
    }
}
