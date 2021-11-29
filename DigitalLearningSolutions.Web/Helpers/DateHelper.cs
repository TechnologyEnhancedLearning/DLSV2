namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Enums;

    public static class DateHelper
    {
        public static string StandardDateFormat = Data.Helpers.DateHelper.StandardDateFormat;

        public static string StandardDateAndTimeFormat = "dd/MM/yyyy HH:mm";

        public static string GetFormatStringForGraphLabel(ReportInterval interval)
        {
            return interval switch
            {
                ReportInterval.Days => "d/M/y",
                ReportInterval.Weeks => "wc d/M/y",
                ReportInterval.Months => "MMM yyyy",
                ReportInterval.Quarters => "yyyy q",
                _ => "yyyy"
            };
        }

        public static string GetFormatStringForDateInTable(ReportInterval interval)
        {
            return interval switch
            {
                ReportInterval.Days => "d/MM/yyyy",
                ReportInterval.Weeks => "'Week commencing' d/MM/yyyy",
                ReportInterval.Months => "MMMM, yyyy",
                ReportInterval.Quarters => "Q, yyyy",
                _ => "yyyy"
            };
        }
    }
}
