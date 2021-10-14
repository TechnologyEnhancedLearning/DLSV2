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
            var quarter = StartDate.Month / 3 + 1;

            format = format.Replace("Q", $"'Quarter' {quarter}").Replace("q", $"Q{quarter}");

            return StartDate.ToString(format);
        }

        public string GetDateRangeLabel(string format, DateTime rangeTerminator, bool startRangeFromTerminator)
        {
            var quarter = StartDate.Month / 3 + 1;

            format = format.Replace("Q", $"'Quarter' {quarter}").Replace("q", $"Q{quarter}");

            return startRangeFromTerminator
                ? rangeTerminator.ToString(format) + " - " + GetFinalDate().ToString(format)
                : StartDate.ToString(format) + " - " + rangeTerminator.ToString(format);
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
