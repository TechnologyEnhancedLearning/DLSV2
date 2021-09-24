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

            format = format.Replace("Q", $"'Quarter' {quarter}").Replace("q", $"Q{quarter}");

            return Date.ToString(format);
        }
    }
}
