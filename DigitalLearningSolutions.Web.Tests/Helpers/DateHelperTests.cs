namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class DateHelperTests
    {
        [Test]
        public void StandardDateFormat_has_expected_format()
        {
            // Given
            var date = new DateTime(2021, 2, 1, 15, 30, 12);

            // When
            var result = date.ToString(DateHelper.StandardDateFormat);

            // Then
            result.Should().Be("01/02/2021");
        }

        [Test]
        public void StandardDateAndTimeFormat_has_expected_format()
        {
            // Given
            var date = new DateTime(2021, 2, 1, 15, 30, 12);

            // When
            var result = date.ToString(DateHelper.StandardDateAndTimeFormat);

            // Then
            result.Should().Be("01/02/2021 15:30");
        }
    }
}
