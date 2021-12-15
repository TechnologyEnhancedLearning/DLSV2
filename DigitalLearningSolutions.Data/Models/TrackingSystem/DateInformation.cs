namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class DateInformation
    {
        public DateInformation(DateTime startDate, ReportInterval interval)
        {
            StartDate = startDate;
            Interval = interval;
        }

        public ReportInterval Interval { get; set; }
        public DateTime StartDate { get; set; }

        public string GetDateLabel(string format)
        {
            format = ConvertQuartersInFormatString(format, StartDate);

            return StartDate.ToString(format);
        }

        public string GetDateRangeLabel(string format, DateTime rangeTerminator, bool startRangeFromTerminator)
        {
            var dateFromThisObject = startRangeFromTerminator ? GetFinalDate() : StartDate;
            var formatForDateFromThisObject = ConvertQuartersInFormatString(format, dateFromThisObject);

            var formatForTerminator = ConvertQuartersInFormatString(format, rangeTerminator);

            return startRangeFromTerminator
                ? rangeTerminator.ToString(formatForTerminator) + " to " +
                  dateFromThisObject.ToString(formatForDateFromThisObject)
                : dateFromThisObject.ToString(formatForDateFromThisObject) + " to " +
                  rangeTerminator.ToString(formatForTerminator);
        }

        public static string GetDateRangeLabel(string format, DateTime startDate, DateTime endDate)
        {
            var startFormat = ConvertQuartersInFormatString(format, startDate);
            var endFormat = ConvertQuartersInFormatString(format, endDate);

            return startDate.ToString(startFormat) + " to " + endDate.ToString(endFormat);
        }

        private static string ConvertQuartersInFormatString(string format, DateTime date)
        {
            var quarter = date.Month / 3 + 1;

            return format.Replace("Q", $"'Quarter' {quarter}").Replace("q", $"Q{quarter}");
        }

        private DateTime GetFinalDate()
        {
            return Interval switch
            {
                ReportInterval.Days => StartDate,
                ReportInterval.Weeks => StartDate.AddDays(6),
                ReportInterval.Months => StartDate.AddMonths(1).AddDays(-1),
                ReportInterval.Quarters => StartDate.AddMonths(3).AddDays(-1),
                _ => StartDate.AddYears(1).AddDays(-1)
            };
        }
    }
}
