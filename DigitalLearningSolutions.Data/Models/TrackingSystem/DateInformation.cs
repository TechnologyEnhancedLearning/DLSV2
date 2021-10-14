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
                ? rangeTerminator.ToString(formatForTerminator) + " - " + dateFromThisObject.ToString(formatForDateFromThisObject)
                : dateFromThisObject.ToString(formatForDateFromThisObject) + " - " + rangeTerminator.ToString(formatForTerminator);
        }

        public string GetDateRangeLabel(string format, DateTime startDate, DateTime endDate)
        {
            var startFormat = ConvertQuartersInFormatString(format, startDate);
            var endFormat = ConvertQuartersInFormatString(format, endDate);

            return startDate.ToString(startFormat) + " - " + endDate.ToString(endFormat);
        }

        private string ConvertQuartersInFormatString(string format, DateTime date)
        {
            var quarter = date.Month / 3 + 1;

            return format.Replace("Q", $"'Quarter' {quarter}").Replace("q", $"Q{quarter}");
        }

        private DateTime GetFinalDate()
        {
            switch (Interval)
            {
                case ReportInterval.Days:
                    return StartDate;
                case ReportInterval.Weeks:
                    return StartDate.AddDays(6);
                case ReportInterval.Months:
                    return StartDate.AddMonths(1).AddDays(-1);
                case ReportInterval.Quarters:
                    return StartDate.AddMonths(3).AddDays(-1);
                default:
                    return StartDate.AddYears(1).AddDays(-1);
            }
        }
    }
}
