namespace DigitalLearningSolutions.Data.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DateHelper
    {
        public static string StandardDateFormat = "dd/MM/yyyy";

        public static IEnumerable<(int Month, int Year)> GetMonthsAndYearsBetweenDates(DateTime startDate, DateTime endDate)
        {
            var diffInMonths = (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month);
            var monthEnumerable = Enumerable.Range(startDate.Month, diffInMonths + 1);

            return monthEnumerable.Select(
                m =>
                {
                    var month = (m - 1) % 12 + 1;
                    var yearsToAdd = (m - 1) / 12;
                    return (month, startDate.Year + yearsToAdd);
                }
            );
        }
    }
}
