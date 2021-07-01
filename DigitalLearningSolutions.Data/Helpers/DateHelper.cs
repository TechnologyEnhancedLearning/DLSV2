namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DateHelper
    {
        public static IEnumerable<(int Month, int Year)> GetMonthsAndYearsBetweenDates(DateTime startDate, DateTime endDate)
        {
            var diffInMonths = (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month);
            var monthEnumerable = Enumerable.Range(startDate.Month, diffInMonths + 1);

            return monthEnumerable.Select(
                m => ((m - 1) % 12 + 1, startDate.Year + (m - 1) / 12)
            );
        }
    }
}
