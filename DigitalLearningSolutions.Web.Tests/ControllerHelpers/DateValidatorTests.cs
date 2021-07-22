namespace DigitalLearningSolutions.Web.Tests.ControllerHelpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class DateValidatorTests
    {
        [Test]
        public void ValidateDate_returns_valid_for_valid_date()
        {
            // When
            var result = DateValidator.ValidateDate(1, 1, 3000);

            // Then
            result.HasDayError.Should().BeFalse();
            result.HasMonthError.Should().BeFalse();
            result.HasYearError.Should().BeFalse();
            result.ErrorMessage.Should().BeNull();
        }

        [Test]
        public void ValidateDate_returns_valid_for_empty_non_required_date()
        {
            // When
            var result = DateValidator.ValidateDate(null, null, null, required: false);

            // Then
            result.HasDayError.Should().BeFalse();
            result.HasMonthError.Should().BeFalse();
            result.HasYearError.Should().BeFalse();
            result.ErrorMessage.Should().BeNull();
        }

        [Test]
        public void ValidateDate_returns_appropriate_error_for_empty_required_date()
        {
            // When
            var result = DateValidator.ValidateDate(null, null, null, required: true);

            // Then
            result.HasDayError.Should().BeTrue();
            result.HasMonthError.Should().BeTrue();
            result.HasYearError.Should().BeTrue();
            result.ErrorMessage.Should().Be("Date is required");
        }

        [TestCase(null, 1, 3000, "a day")]
        [TestCase(1, null, 3000, "a month")]
        [TestCase(1, 1, null, "a year")]
        [TestCase(null, null, 3000, "a day and a month")]
        [TestCase(null, 1, null, "a day and a year")]
        [TestCase(1, null, null, "a month and a year")]
        public void ValidateDate_returns_appropriate_error_if_some_values_missing(
            int? day,
            int? month,
            int? year,
            string errorMessageEnding
        )
        {
            // When
            var result = DateValidator.ValidateDate(day, month, year);

            // Then
            result.HasDayError.Should().Be(!day.HasValue);
            result.HasMonthError.Should().Be(!month.HasValue);
            result.HasYearError.Should().Be(!year.HasValue);
            result.ErrorMessage.Should().Be("Date must include " + errorMessageEnding);
        }

        [Test]
        public void ValidateDate_returns_appropriate_error_if_all_values_invalid()
        {
            // When
            var result = DateValidator.ValidateDate(0, 0, 0, required: true);

            // Then
            result.HasDayError.Should().BeTrue();
            result.HasMonthError.Should().BeTrue();
            result.HasYearError.Should().BeTrue();
            result.ErrorMessage.Should().Be("Date must be a real date");
        }

        [TestCase(0, 1, 3000, true, false, false, "day")]
        [TestCase(32, 1, 3000, true, false, false, "day")]
        [TestCase(1, 0, 3000, false, true, false, "month")]
        [TestCase(1, 13, 3000, false, true, false, "month")]
        [TestCase(1, 1, 0, false, false, true, "year")]
        [TestCase(1, 1, 10000, false, false, true, "year")]
        [TestCase(0, 0, 3000, true, true, false, "day and month")]
        [TestCase(0, 1, 0, true, false, true, "day and year")]
        [TestCase(1, 0, 0, false, true, true, "month and year")]
        public void ValidateDate_returns_appropriate_error_if_some_values_invalid(
            int day,
            int month,
            int year,
            bool dayError,
            bool monthError,
            bool yearError,
            string errorMessageEnding
        )
        {
            // When
            var result = DateValidator.ValidateDate(day, month, year);

            // Then
            result.HasDayError.Should().Be(dayError);
            result.HasMonthError.Should().Be(monthError);
            result.HasYearError.Should().Be(yearError);
            result.ErrorMessage.Should().Be("Date must include a real " + errorMessageEnding);
        }

        [Test]
        public void ValidateDate_returns_appropriate_error_if_date_not_real()
        {
            // When
            var result = DateValidator.ValidateDate(30, 2, 3000);

            // Then
            result.HasDayError.Should().BeTrue();
            result.HasMonthError.Should().BeTrue();
            result.HasYearError.Should().BeTrue();
            result.ErrorMessage.Should().Be("Date must be a real date");
        }

        [Test]
        public void ValidateDate_returns_appropriate_error_if_date_not_in_future()
        {
            // When
            var result = DateValidator.ValidateDate(1, 1, 2000);

            // Then
            result.HasDayError.Should().BeTrue();
            result.HasMonthError.Should().BeTrue();
            result.HasYearError.Should().BeTrue();
            result.ErrorMessage.Should().Be("Date must be in the future");
        }

        [Test]
        public void ValidateDate_uses_name_correctly_in_error_message()
        {
            // When
            var result = DateValidator.ValidateDate(null, null, null, "What's required", true);

            // Then
            result.HasDayError.Should().BeTrue();
            result.HasMonthError.Should().BeTrue();
            result.HasYearError.Should().BeTrue();
            result.ErrorMessage.Should().Be("What's required is required");
        }
    }
}
