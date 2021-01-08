namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    class DurationFormattingHelperTests
    {
        [Test]
        public void FormatDuration_should_have_averageDuration_for_0_minutes()
        {
            // Given
            const int averageDuration = 0;

            // When
            var result = DurationFormattingHelper.FormatDuration(averageDuration);

            // Then
            result.Should().Be("0 minutes");
        }

        [Test]
        public void FormatDuration_should_have_averageDuration_for_1_minute()
        {
            // Given
            const int averageDuration = 1;

            // When
            var result = DurationFormattingHelper.FormatDuration(averageDuration);

            // Then
            result.Should().Be("1 minute");
        }

        [Test]
        public void FormatDuration_should_have_averageDuration_for_under_an_hour()
        {
            // Given
            const int averageDuration = 30;

            // When
            var result = DurationFormattingHelper.FormatDuration(averageDuration);

            // Then
            result.Should().Be("30 minutes");
        }

        [Test]
        public void FormatDuration_should_have_averageDuration_for_whole_number_of_hours()
        {
            // Given
            const int averageDuration = 120;

            // When
            var result = DurationFormattingHelper.FormatDuration(averageDuration);

            // Then
            result.Should().Be("2 hours");
        }

        [Test]
        public void FormatDuration_should_have_averageDuration_for_one_hour_one_minute()
        {
            // Given
            const int averageDuration = 61;

            // When
            var result = DurationFormattingHelper.FormatDuration(averageDuration);

            // Then
            result.Should().Be("1 hour 1 minute");
        }


        [Test]
        public void FormatDuration_should_have_averageDuration_for_multiple_hours()
        {
            // Given
            const int averageDuration = 195;

            // When
            var result = DurationFormattingHelper.FormatDuration(averageDuration);

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
        public void FormatNullableDuration_should_format_durations(int? averageDuration, string? expectedResult)
        {
            // When
            var result = DurationFormattingHelper.FormatNullableDuration(averageDuration);

            // Then
            result.Should().Be(expectedResult);
        }
    }
}
