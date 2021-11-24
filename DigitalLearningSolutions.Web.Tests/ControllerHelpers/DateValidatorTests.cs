namespace DigitalLearningSolutions.Web.Tests.ControllerHelpers
{
    using System;
    using System.Linq;
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
        public void ValidateDate_returns_valid_for_todays_date()
        {
            // When
            var today = DateTime.Today;
            var result = DateValidator.ValidateDate(today.Day, today.Month, today.Year);

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
            result.ErrorMessage.Should().Be("Enter a Date");
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
            result.ErrorMessage.Should().Be("Enter a Date containing " + errorMessageEnding);
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
            result.ErrorMessage.Should().Be("Enter a real date for Date");
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
            result.ErrorMessage.Should().Be("Enter a Date containing a real " + errorMessageEnding);
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
            result.ErrorMessage.Should().Be("Enter a real date for Date");
        }

        [Test]
        public void ValidateDate_returns_appropriate_error_if_date_in_past()
        {
            // When
            var result = DateValidator.ValidateDate(1, 1, 2000);

            // Then
            result.HasDayError.Should().BeTrue();
            result.HasMonthError.Should().BeTrue();
            result.HasYearError.Should().BeTrue();
            result.ErrorMessage.Should().Be("Enter a Date not in the past");
        }

        [Test]
        public void ValidateDate_returns_appropriate_error_if_date_in_future()
        {
            // When
            var result = DateValidator.ValidateDate(1, 1, 3000, validateNonFuture: true);

            // Then
            result.HasDayError.Should().BeTrue();
            result.HasMonthError.Should().BeTrue();
            result.HasYearError.Should().BeTrue();
            result.ErrorMessage.Should().Be("Enter a Date not in the future");
        }

        [Test]
        public void ValidateDate_uses_name_correctly_in_error_message()
        {
            // When
            var result = DateValidator.ValidateDate(null, null, null, "entrance and exit an exit.", true);

            // Then
            result.HasDayError.Should().BeTrue();
            result.HasMonthError.Should().BeTrue();
            result.HasYearError.Should().BeTrue();
            result.ErrorMessage.Should().Be("Enter an entrance and exit an exit.");
        }

        [TestCase(true, false, false, "Day")]
        [TestCase(false, true, false, "Month")]
        [TestCase(false, false, true, "Year")]
        [TestCase(true, true, false, "Day", "Month")]
        [TestCase(true, false, true, "Day", "Year")]
        [TestCase(false, true, true, "Month", "Year")]
        [TestCase(true, true, true, "Day", "Month", "Year")]
        public void ToValidationResultList_includes_one_error_for_each_erroneous_part_of_date(
            bool hasDayError,
            bool hasMonthError,
            bool hasYearError,
            params string[] errorMemberNames
        )
        {
            // Given
            var dateValidationResult = new DateValidator.DateValidationResult(
                hasDayError,
                hasMonthError,
                hasYearError,
                "msg"
            );

            // When
            var result = dateValidationResult.ToValidationResultList("Day", "Month", "Year");

            // Then
            result.Should().HaveCount(errorMemberNames.Length);
            foreach (var memberName in errorMemberNames)
            {
                result.Should().Contain(
                    validationResult =>
                        validationResult.MemberNames.Count() == 1 && validationResult.MemberNames.Contains(memberName)
                );
            }
        }

        [Test]
        public void ToValidationResultList_includes_errors_in_appropriate_order()
        {
            // Given
            var dateValidationResult = new DateValidator.DateValidationResult(true, true, true, "msg");

            // When
            var result = dateValidationResult.ToValidationResultList("Day", "Month", "Year");

            // Then
            result[0].MemberNames.Should().Contain("Day");
            result[1].MemberNames.Should().Contain("Month");
            result[2].MemberNames.Should().Contain("Year");
        }

        [Test]
        public void ToValidationResultList_only_includes_message_for_first_erroneous_part_of_date()
        {
            // Given
            const string errorMessage = "msg";
            var dateValidationResult = new DateValidator.DateValidationResult(false, true, true, errorMessage);

            // When
            var result = dateValidationResult.ToValidationResultList("Day", "Month", "Year");

            // Then
            result[0].ErrorMessage.Should().Be(errorMessage);
            result[1].ErrorMessage.Should().Be(string.Empty);
        }

        [Test]
        public void ToValidationResultList_uses_correct_member_names_for_errors()
        {
            // Given
            var dateValidationResult = new DateValidator.DateValidationResult(true, true, true, "msg");
            const string dayId = "TheDay";
            const string monthId = "TheMonth";
            const string yearId = "TheYear";

            // When
            var result = dateValidationResult.ToValidationResultList(dayId, monthId, yearId);

            // Then
            result[0].MemberNames.Should().Contain(dayId);
            result[1].MemberNames.Should().Contain(monthId);
            result[2].MemberNames.Should().Contain(yearId);
        }
    }
}
