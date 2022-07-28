namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Utilities;

    public static class DateValidator
    {
        private static readonly ClockUtility ClockUtility = new ClockUtility();

        public static DateValidationResult ValidateDate(
            int? day,
            int? month,
            int? year,
            string name = "Date",
            bool required = false,
            bool validateNonPast = true,
            bool validateNonFuture = false
        )
        {
            if (!day.HasValue && !month.HasValue && !year.HasValue)
            {
                return required
                    ? new DateValidationResult("Enter " + NameWithIndefiniteArticle(name))
                    : new DateValidationResult();
            }

            if (day.HasValue && month.HasValue && year.HasValue)
            {
                return ValidateDate(day.Value, month.Value, year.Value, name, validateNonPast, validateNonFuture);
            }

            var errorMessage = GetMissingValuesErrorMessage(day, month, year, name);
            return new DateValidationResult(!day.HasValue, !month.HasValue, !year.HasValue, errorMessage);
        }

        public static bool IsDateNull(int? day, int? month, int? year)
        {
            return day == null && month == null && year == null;
        }

        private static DateValidationResult ValidateDate(
            int day,
            int month,
            int year,
            string name,
            bool dateMustNotBeInPast,
            bool dateMustNotBeInFuture
        )
        {
            // note: the minimum year the DB can store is 1753
            var invalidDay = day < 1 || day > 31;
            var invalidMonth = month < 1 || month > 12;
            var invalidYear = year < 1 || year > 9999;

            if (invalidDay || invalidMonth || invalidYear)
            {
                var errorMessage = GetInvalidValuesErrorMessage(invalidDay, invalidMonth, invalidYear, name);
                return new DateValidationResult(invalidDay, invalidMonth, invalidYear, errorMessage);
            }

            try
            {
                var date = new DateTime(year, month, day);
                if (dateMustNotBeInPast && date < ClockUtility.UtcToday)
                {
                    return new DateValidationResult("Enter " + NameWithIndefiniteArticle(name) + " not in the past");
                }

                if (dateMustNotBeInFuture && date > ClockUtility.UtcToday)
                {
                    return new DateValidationResult("Enter " + NameWithIndefiniteArticle(name) + " not in the future");
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return new DateValidationResult("Enter a real date for " + name);
            }

            return new DateValidationResult();
        }

        private static string GetMissingValuesErrorMessage(int? day, int? month, int? year, string name)
        {
            var missingValues = new List<string>();

            if (!day.HasValue)
            {
                missingValues.Add("day");
            }

            if (!month.HasValue)
            {
                missingValues.Add("month");
            }

            if (!year.HasValue)
            {
                missingValues.Add("year");
            }

            return "Enter " + NameWithIndefiniteArticle(name) + " containing a " +
                   string.Join(" and a ", missingValues);
        }

        private static string GetInvalidValuesErrorMessage(
            bool invalidDay,
            bool invalidMonth,
            bool invalidYear,
            string name
        )
        {
            var invalidValues = new List<string>();

            if (invalidDay)
            {
                invalidValues.Add("day");
            }

            if (invalidMonth)
            {
                invalidValues.Add("month");
            }

            if (invalidYear)
            {
                invalidValues.Add("year");
            }

            if (invalidValues.Count == 3)
            {
                return "Enter a real date for " + name;
            }

            return "Enter " + NameWithIndefiniteArticle(name) + " containing a real " +
                   string.Join(" and ", invalidValues);
        }

        private static string NameWithIndefiniteArticle(string name)
        {
            return (StartsWithVowel(name) ? "an " : "a ") + name;
        }

        private static bool StartsWithVowel(string str)
        {
            return "aeiouAEIOU".Contains(str[0]);
        }

        public class DateValidationResult
        {
            public readonly string? ErrorMessage;
            public readonly bool HasDayError;
            public readonly bool HasMonthError;
            public readonly bool HasYearError;

            public DateValidationResult() { }

            public DateValidationResult(string errorMessage)
            {
                HasDayError = true;
                HasMonthError = true;
                HasYearError = true;
                ErrorMessage = errorMessage;
            }

            public DateValidationResult(bool hasDayError, bool hasMonthError, bool hasYearError, string? errorMessage)
            {
                HasDayError = hasDayError;
                HasMonthError = hasMonthError;
                HasYearError = hasYearError;
                ErrorMessage = errorMessage;
            }

            public List<ValidationResult> ToValidationResultList(
                string dayId,
                string monthId,
                string yearId
            )
            {
                var results = new List<ValidationResult>();
                var errorMessageAdded = false;

                if (HasDayError)
                {
                    results.Add(new ValidationResult(ErrorMessage, new[] { dayId }));
                    errorMessageAdded = true;
                }

                if (HasMonthError)
                {
                    // Should only add error message once per date to avoid duplicates in error summary component, but still highlight all inputs with errors
                    var errorMessage = !errorMessageAdded ? ErrorMessage : "";
                    results.Add(new ValidationResult(errorMessage, new[] { monthId }));
                    errorMessageAdded = true;
                }

                if (HasYearError)
                {
                    // Should only add error message once per date to avoid duplicates in error summary component, but still highlight all inputs with errors
                    var errorMessage = !errorMessageAdded ? ErrorMessage : "";
                    results.Add(new ValidationResult(errorMessage, new[] { yearId }));
                }

                return results;
            }
        }
    }
}
