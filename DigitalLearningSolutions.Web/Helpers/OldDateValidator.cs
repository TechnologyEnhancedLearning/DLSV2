namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Utilities;

    // This helper class is currently used in LearningPortal pages
    // Switch over to the new DateValidator class in HEEDLS-560

    public static class OldDateValidator
    {
        private static readonly IClockUtility ClockUtility = new ClockUtility();

        public class ValidationResult
        {
            public ValidationResult()
            {
                DateValid = true;
                ErrorMessage = "";
                DayValid = true;
                MonthValid = true;
                YearValid = true;
            }

            public ValidationResult(int day, int month, int year)
            {
                DateValid = true;
                ErrorMessage = "";
                DayValid = true;
                MonthValid = true;
                YearValid = true;
                Day = day;
                Month = month;
                Year = year;
            }

            public bool DateValid { get; set; }
            public string ErrorMessage { get; set; }
            public bool DayValid { get; set; }
            public bool MonthValid { get; set; }
            public bool YearValid { get; set; }
            public int? Day { get; }
            public int? Month { get; }
            public int? Year { get; }
        }

        public static ValidationResult ValidateDate(int day, int month, int year)
        {
            if (day == 0 && month == 0 && year == 0)
            {
                return new ValidationResult(day, month, year);
            }

            try
            {
                if (year < 1753)
                {
                    // The minimum year the DB can store is 1753
                    throw new ArgumentOutOfRangeException();
                }

                var newDate = new DateTime(year, month, day);
                if (newDate <= ClockUtility.UtcToday)
                {
                    return new ValidationResult(day, month, year)
                    {
                        DateValid = false,
                        DayValid = false,
                        MonthValid = false,
                        YearValid = false,
                        ErrorMessage = "Complete by date must be in the future"
                    };
                }
                return new ValidationResult(day, month, year);
            }
            catch (ArgumentOutOfRangeException)
            {
                return GetValidationError(day, month, year, "Complete by date");
            }
        }

        private static ValidationResult GetValidationError(int day, int month, int year, string name)
        {
            var error = new ValidationResult(day, month, year) { DateValid = false };

            error.DayValid = DayIsValid(day);
            error.MonthValid = MonthIsValid(month);
            error.YearValid = YearIsValid(year);
            if (
                !error.DayValid && !error.MonthValid
                || !error.DayValid && !error.YearValid
                || !error.MonthValid && !error.YearValid
                || error.DayValid && error.MonthValid && error.YearValid
            )
            {
                error.DayValid = false;
                error.MonthValid = false;
                error.YearValid = false;
            }

            error.ErrorMessage = ConstructErrorMessage(day, month, year, name);

            return error;
        }

        private static bool DayIsValid(int day)
        {
            return day > 0 && day < 32;
        }

        private static bool MonthIsValid(int day)
        {
            return day > 0 && day < 13;
        }

        private static bool YearIsValid(int year)
        {
            return year > 1752 && year < 10000;
        }

        private static string ConstructErrorMessage(int day, int month, int year, string name)
        {
            List<string> emptyElements = new List<string>();

            if (day == 0)
            {
                emptyElements.Add("day");
            }

            if (month == 0)
            {
                emptyElements.Add("month");
            }

            if (year == 0)
            {
                emptyElements.Add("year");
            }

            if (emptyElements.Any())
            {
                return name + " must include a " + string.Join(" and ", emptyElements);
            }

            return name + " must be a real date";
        }
    }
}
