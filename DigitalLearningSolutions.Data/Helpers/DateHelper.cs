namespace DigitalLearningSolutions.Data.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;

    public static class DateHelper
    {
        public static string StandardDateFormat = "dd/MM/yyyy";

        public static DateTime ReferenceDate => new DateTime(1905, 1, 1);

        public static IEnumerable<DateTime> GetPeriodsBetweenDates(
            DateTime startDate,
            DateTime endDate,
            ReportInterval interval
        )
        {
            return interval switch
            {
                ReportInterval.Days => GetDaysBetweenDates(startDate, endDate),
                ReportInterval.Weeks => GetWeeksBetweenDates(startDate, endDate),
                ReportInterval.Months => GetMonthsBetweenDates(startDate, endDate),
                ReportInterval.Quarters => GetQuartersBetweenDates(startDate, endDate),
                _ => GetYearsBetweenDates(startDate, endDate)
            };
        }

        private static IEnumerable<DateTime> GetDaysBetweenDates(
            DateTime startDate,
            DateTime endDate
        )
        {
            var diffInDays = (endDate - startDate).Days;
            var daysDifferenceEnumerable = Enumerable.Range(0, diffInDays + 1);

            return daysDifferenceEnumerable.Select(
                d =>
                {
                    var day = startDate.AddDays(d);
                    return new DateTime(day.Year, day.Month, day.Day);
                }
            );
        }

        private static IEnumerable<DateTime> GetWeeksBetweenDates(
            DateTime startDate,
            DateTime endDate
        )
        {
            var referenceDate = ReferenceDate;

            var diffInWeeks = (endDate - referenceDate).Days / 7;
            var weeksDifferenceEnumerable = Enumerable.Range(0, diffInWeeks + 1);

            var rawWeekSlots = weeksDifferenceEnumerable.Select(
                w => referenceDate.AddDays(w * 7)
            );

            return rawWeekSlots.Where(w => w.AddDays(7) > startDate);
        }

        private static IEnumerable<DateTime> GetMonthsBetweenDates(
            DateTime startDate,
            DateTime endDate
        )
        {
            var diffInMonths = (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month);
            var monthEnumerable = Enumerable.Range(startDate.Month, diffInMonths + 1);

            return monthEnumerable.Select(
                m =>
                {
                    var month = (m - 1) % 12 + 1;
                    var yearsToAdd = (m - 1) / 12;
                    return new DateTime(startDate.AddYears(yearsToAdd).Year, month, 1);
                }
            );
        }

        private static IEnumerable<DateTime> GetQuartersBetweenDates(
            DateTime startDate,
            DateTime endDate
        )
        {
            var diffInQuartersFromYears = (endDate.Year - startDate.Year) * 4;
            var diffInQuarters = diffInQuartersFromYears +
                                 (ConvertMonthToQuarter(endDate.Month) - ConvertMonthToQuarter(startDate.Month));
            var quarterEnumerable = Enumerable.Range(ConvertMonthToQuarter(startDate.Month), diffInQuarters + 1);

            return quarterEnumerable.Select(
                q =>
                {
                    var quarter = (q - 1) % 4 + 1;
                    var yearsToAdd = (q - 1) / 4;
                    return new DateTime(startDate.AddYears(yearsToAdd).Year, quarter * 3 - 2, 1);
                }
            );
        }

        private static IEnumerable<DateTime> GetYearsBetweenDates(
            DateTime startDate,
            DateTime endDate
        )
        {
            var diffInYears = endDate.Year - startDate.Year;
            var yearEnumerable = Enumerable.Range(startDate.Year, diffInYears + 1);

            return yearEnumerable.Select(
                y => new DateTime(y, 1, 1)
            );
        }

        private static int ConvertMonthToQuarter(int month)
        {
            return (month - 1) / 3 + 1;
        }
    }
}
