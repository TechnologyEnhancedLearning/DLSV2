namespace DigitalLearningSolutions.Web.Tests.ControllerHelpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class OldDateValidatorTests
    {
        [Test]
        public void Date_validator_returns_valid_for_valid_date()
        {
            // When
            var result = OldDateValidator.ValidateDate(1, 1, 3020);

            // Then
            result.DateValid.Should().BeTrue();
            result.DayValid.Should().BeTrue();
            result.MonthValid.Should().BeTrue();
            result.YearValid.Should().BeTrue();
        }

        [Test]
        public void Date_validator_returns_valid_for_empty_date()
        {
            // When
            var result = OldDateValidator.ValidateDate(0, 0, 0);

            // Then
            result.DateValid.Should().BeTrue();
            result.DayValid.Should().BeTrue();
            result.MonthValid.Should().BeTrue();
            result.YearValid.Should().BeTrue();
        }

        [Test]
        public void Date_validator_returns_invalid_for_a_date_with_day_missing()
        {
            // When
            var result = OldDateValidator.ValidateDate(0, 1, 2020);

            // Then
            result.DateValid.Should().BeFalse();
            result.DayValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Complete by date must include a day");
        }

        [Test]
        public void Date_validator_returns_invalid_for_a_date_with_day_and_month_missing()
        {
            // When
            var result = OldDateValidator.ValidateDate(0, 0, 2020);

            // Then
            result.DateValid.Should().BeFalse();
            result.DayValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Complete by date must include a day and month");
        }

        [Test]
        public void Date_validator_returns_invalid_for_a_date_with_day_and_year_missing()
        {
            // When
            var result = OldDateValidator.ValidateDate(0, 1, 0);

            // Then
            result.DateValid.Should().BeFalse();
            result.DayValid.Should().BeFalse();
            result.MonthValid.Should().BeFalse();
            result.YearValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Complete by date must include a day and year");
        }

        [Test]
        public void Date_validator_returns_invalid_for_a_date_with_month_missing()
        {
            // When
            var result = OldDateValidator.ValidateDate(1, 0, 2020);

            // Then
            result.DateValid.Should().BeFalse();
            result.MonthValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Complete by date must include a month");
        }

        [Test]
        public void Date_validator_returns_invalid_for_a_date_with_month_and_year_missing()
        {
            // When
            var result = OldDateValidator.ValidateDate(1, 0, 0);

            // Then
            result.DateValid.Should().BeFalse();
            result.DayValid.Should().BeFalse();
            result.MonthValid.Should().BeFalse();
            result.YearValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Complete by date must include a month and year");
        }

        [Test]
        public void Date_validator_returns_invalid_for_a_date_with_bad_day()
        {
            // When
            var result = OldDateValidator.ValidateDate(32, 1, 2020);

            // Then
            result.DateValid.Should().BeFalse();
            result.DayValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Complete by date must be a real date");
        }

        [Test]
        public void Date_validator_returns_invalid_for_a_date_with_bad_month()
        {
            // When
            var result = OldDateValidator.ValidateDate(31, 13, 2020);

            // Then
            result.DateValid.Should().BeFalse();
            result.MonthValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Complete by date must be a real date");
        }

        [Test]
        public void Date_validator_returns_invalid_for_a_date_with_bad_year()
        {
            // When
            var result = OldDateValidator.ValidateDate(31, 1, 20201);

            // Then
            result.DateValid.Should().BeFalse();
            result.YearValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Complete by date must be a real date");
        }

        [Test]
        public void Date_validator_returns_invalid_for_a_date_with_bad_date()
        {
            // When
            var result = OldDateValidator.ValidateDate(31, 2, 2020);

            // Then
            result.DateValid.Should().BeFalse();
            result.DayValid.Should().BeFalse();
            result.MonthValid.Should().BeFalse();
            result.YearValid.Should().BeFalse();
            result.ErrorMessage.Should().Be("Complete by date must be a real date");
        }
    }
}
