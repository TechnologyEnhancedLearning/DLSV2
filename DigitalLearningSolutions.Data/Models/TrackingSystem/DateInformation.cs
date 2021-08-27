namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class DateInformation
    {
        public DateInformation(DateTime date, ReportInterval interval)
        {
            Date = date;
            Interval = interval;
        }

        public ReportInterval Interval { get; set; }
        public DateTime Date { get; set; }

        public string GetDateLabel(string format)
        {
            var quarter = Date.Month / 3 + 1;

            format = format.Replace("Q", $"Quarter {quarter}").Replace("q", $"Q{quarter}");

            return Date.ToString(format);
        }

        public string GetFormatStringForUsageStatsGraph()
        {
            return Interval switch
            {
                ReportInterval.Days => "d/M/y",
                ReportInterval.Weeks => "wc d/M/y",
                ReportInterval.Months => "MMM yyyy",
                ReportInterval.Quarters => "yyyy q",
                _ => "yyyy"
            };
        }

        public string GetFormatStringForUsageStatsTable()
        {
            return Interval switch
            {
                ReportInterval.Days => "d/MM/yyyy",
                ReportInterval.Weeks => "Week commencing d/MM/yyyy",
                ReportInterval.Months => "MMMM, yyyy",
                ReportInterval.Quarters => "Q, yyyy",
                _ => "yyyy"
            };
        }
    }
}
