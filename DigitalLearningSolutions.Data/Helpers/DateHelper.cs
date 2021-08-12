namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public static class DateHelper
    {
        public static IEnumerable<DateInformation> GetPeriodsBetweenDates(
            DateTime startDate,
            DateTime endDate,
            ReportInterval interval
        )
        {
            switch (interval)
            {
                case ReportInterval.Days:
                    return GetDaysBetweenDates(startDate, endDate);
                case ReportInterval.Weeks:
                    return GetWeeksBetweenDates(startDate, endDate);
                case ReportInterval.Months:
                    return GetMonthsBetweenDates(startDate, endDate);
                case ReportInterval.Quarters:
                    return GetQuartersBetweenDates(startDate, endDate);
                default:
                    return GetYearsBetweenDates(startDate, endDate);
            }
        }

        private static IEnumerable<DateInformation> GetDaysBetweenDates(
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
                    return new DateInformation
                    {
                        Interval = ReportInterval.Days,
                        Date = new DateTime(day.Year, day.Month, day.Day)
                    };
                }
            );
        }

        private static IEnumerable<DateInformation> GetWeeksBetweenDates(
            DateTime startDate,
            DateTime endDate
        )
        {
            var referenceDate = new DateTime(1905, 1, 1);

            var diffInWeeks = (endDate - referenceDate).Days / 7;
            var weeksDifferenceEnumerable = Enumerable.Range(0, diffInWeeks + 1);

            var rawWeekSlots = weeksDifferenceEnumerable.Select(
                w =>
                {
                    return new DateInformation
                    {
                        Interval = ReportInterval.Weeks,
                        Date = referenceDate.AddDays(w * 7)
                    };
                }
            );

            return rawWeekSlots.Where(w => w.Date?.AddDays(7) > startDate);
        }

        private static IEnumerable<DateInformation> GetMonthsBetweenDates(
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
                    return new DateInformation
                    {
                        Interval = ReportInterval.Months,
                        Date = new DateTime(startDate.AddYears(yearsToAdd).Year, month, 1)
                    };
                }
            );
        }

        private static IEnumerable<DateInformation> GetQuartersBetweenDates(
            DateTime startDate,
            DateTime endDate
        )
        {
            var diffInQuarters = (endDate.Year - startDate.Year) * 4 +
                                 ((endDate.Month - 1) / 3 - (startDate.Month - 1) / 3);
            var quarterEnumerable = Enumerable.Range(startDate.Month, diffInQuarters + 1);

            return quarterEnumerable.Select(
                q =>
                {
                    var quarter = (q - 1) % 4 + 1;
                    var yearsToAdd = (q - 1) / 4;
                    return new DateInformation
                    {
                        Interval = ReportInterval.Quarters,
                        Date = new DateTime(startDate.AddYears(yearsToAdd).Year, quarter * 3 - 2, 1)
                    };
                }
            );
        }

        private static IEnumerable<DateInformation> GetYearsBetweenDates(
            DateTime startDate,
            DateTime endDate
        )
        {
            var yearEnumerable = Enumerable.Range(startDate.Year, endDate.Year - startDate.Year + 1);

            return yearEnumerable.Select(
                y =>
                {
                    return new DateInformation
                    {
                        Interval = ReportInterval.Years,
                        Date = new DateTime(y, 1, 1)
                    };
                }
            );
        }
    }
}
