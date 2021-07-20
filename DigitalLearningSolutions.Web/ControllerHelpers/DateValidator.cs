namespace DigitalLearningSolutions.Web.ControllerHelpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public static class DateValidator
    {
        public static DateValidationResult ValidateDate(
            int? day,
            int? month,
            int? year,
            string name = "Date",
            bool required = false
        )
        {
            if (!required && !day.HasValue && !month.HasValue && !year.HasValue)
            {
                return new DateValidationResult();
            }

            if (!day.HasValue || !month.HasValue || !year.HasValue)
            {
                var errorMessage = name + (required ? " is required" : " must have day, month and year");
                return new DateValidationResult(!day.HasValue, !month.HasValue, !year.HasValue, errorMessage);
            }

            return ValidateDate(day.Value, month.Value, year.Value, name);
        }

        private static DateValidationResult ValidateDate(int day, int month, int year, string name)
        {
            var invalidDay = day < 1 || day > 31;
            var invalidMonth = month < 1 || month > 12;
            var invalidYear = year < 1753 || year > 10000;

            if (invalidDay || invalidMonth || invalidYear)
            {
                return new DateValidationResult(invalidDay, invalidMonth, invalidYear, name + " must be a real date");
            }

            try
            {
                var date = new DateTime(year, month, day);
                if (date <= DateTime.Today)
                {
                    return new DateValidationResult(name + " must be in the future");
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return new DateValidationResult(name + " must be a real date");
            }

            return new DateValidationResult();
        }

        internal static List<ValidationResult> ToValidationResultList(
            DateValidationResult result,
            string dayId,
            string monthId,
            string yearId
        )
        {
            var results = new List<ValidationResult>();
            var errorMessageAdded = false;

            if (result.HasDayError)
            {
                results.Add(new ValidationResult(result.ErrorMessage, new[] { dayId }));
                errorMessageAdded = true;
            }

            if (result.HasMonthError)
            {
                results.Add(new ValidationResult(!errorMessageAdded ? result.ErrorMessage : "", new[] { monthId }));
                errorMessageAdded = true;
            }

            if (result.HasYearError)
            {
                results.Add(new ValidationResult(!errorMessageAdded ? result.ErrorMessage : "", new[] { yearId }));
            }

            return results;
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
        }
    }
}
