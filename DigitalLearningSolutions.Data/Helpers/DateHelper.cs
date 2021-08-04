﻿namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;

    public static class DateHelper
    {
        public static IEnumerable<(int Month, int Year)> GetMonthsAndYearsBetweenDates2(DateTime startDate, DateTime endDate)
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

        public static IEnumerable<DateInformation> GetPeriodsBetweenDates(
            DateTime startDate,
            DateTime endDate,
            ReportInterval interval
        )
        {

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
                        Day = day.Day,
                        Month = day.Month,
                        Year = day.Year
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
                        WeekBeginning = referenceDate.AddDays(w * 7),
                    };
                }
            );

            return rawWeekSlots.Where(w => w.WeekBeginning?.AddDays(7) > startDate);
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
                        Month = month,
                        Year = startDate.Year + yearsToAdd
                    };
                }
            );
        }

        private static IEnumerable<DateInformation> GetQuartersBetweenDates(
            DateTime startDate,
            DateTime endDate
        )
        {
            var diffInQuarters = (endDate.Year - startDate.Year) * 4 + (endDate.Month / 3 - startDate.Month / 3);
            var quarterEnumerable = Enumerable.Range(startDate.Month, diffInQuarters + 1);

            return quarterEnumerable.Select(
                q =>
                {
                    var quarter = (q - 1) % 4 + 1;
                    var yearsToAdd = (q - 1) / 4;
                    return new DateInformation
                    {
                        Interval = ReportInterval.Quarters,
                        Quarter = quarter,
                        Year = startDate.Year + yearsToAdd
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
                        Year = y
                    };
                }
            );
        }
    }
}
