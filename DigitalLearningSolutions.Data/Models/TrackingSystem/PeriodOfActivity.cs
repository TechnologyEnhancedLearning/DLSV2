namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class DateInformation
    {
        public ReportInterval Interval { get; set; }
        public DateTime? Date { get; set; }

        public string GetDateLabel(bool shortForm)
        {
            string formatString;

            var quarter = Date?.Month / 3 + 1;

            switch (Interval)
            {
                case ReportInterval.Days:
                    formatString = shortForm ? "y/M/d" : "yyyy/MM/d";
                    break;
                case ReportInterval.Weeks:
                    formatString = shortForm ? "wc y/M/d" : "Week commencing yyyy/MM/d";
                    break;
                case ReportInterval.Months:
                    formatString = shortForm ? "MMM yyyy" : "MMMM, yyyy";
                    break;
                case ReportInterval.Quarters:
                    formatString = shortForm ? $"yyyy Q{quarter}" : $"Quarter {quarter}, yyyy";
                    break;
                default:
                    formatString = "yyyy";
                    break;
            }

            return Date?.ToString(formatString) ?? "";
        }
    }

    public class PeriodOfActivity
    {
        public PeriodOfActivity(DateInformation date, int registrations, int completions, int evaluations)
        {
            DateInformation = date;
            Registrations = registrations;
            Completions = completions;
            Evaluations = evaluations;
        }

        public PeriodOfActivity(DateInformation date, PeriodOfActivity? data)
        {
            DateInformation = date;
            Completions = data?.Completions ?? 0;
            Evaluations = data?.Evaluations ?? 0;
            Registrations = data?.Registrations ?? 0;
        }

        public DateInformation DateInformation { get; set; }
        public int Completions { get; set; }
        public int Evaluations { get; set; }
        public int Registrations { get; set; }
    }
}
