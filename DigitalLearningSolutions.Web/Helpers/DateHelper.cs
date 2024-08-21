namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Enums;
    using Microsoft.AspNetCore.Http;
    using System;

    public static class DateHelper
    {
        public static string StandardDateFormat = Data.Helpers.DateHelper.StandardDateFormat;

        public static string StandardDateAndTimeFormat = "dd/MM/yyyy HH:mm";
        public static string userTimeZone { get; set; }
        public static string DefaultTimeZone = "Europe/London";

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

        public static DateTime? GetLocalDateTime(DateTime? dateUtc)
        {
            if (dateUtc == null)
                return null;

            var accessor = new HttpContextAccessor();
            var timeZone = accessor.HttpContext.User.GetUserTimeZone(CustomClaimTypes.UserTimeZone);

            if (string.IsNullOrEmpty(timeZone))
                timeZone = DefaultTimeZone;

            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc((DateTime)dateUtc, timeZoneInfo);

            return localDateTime;
        }
    }
}
