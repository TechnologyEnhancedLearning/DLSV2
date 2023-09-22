using DigitalLearningSolutions.Data.Enums;
using System;

namespace DigitalLearningSolutions.Web.Helpers
{
    public static class ReportValidationHelper
    {
        public static bool IsPeriodCompatibleWithDateRange(ReportInterval reportInterval, DateTime startDate, DateTime? endDate)
        {
            if (endDate == null)
            {
                endDate = DateTime.Now;
            }

            int rowLimit = 250;
            int differenceInDays = (int)(endDate.Value - startDate).TotalDays;

            switch (reportInterval)
            {
                case ReportInterval.Days:
                    return (differenceInDays) <= rowLimit;
                case ReportInterval.Weeks:
                    return (differenceInDays / 7) <= rowLimit;
                case ReportInterval.Months:
                    return (differenceInDays / 30) <= rowLimit;
                case ReportInterval.Quarters:
                    return (differenceInDays / 90) <= rowLimit;
                case ReportInterval.Years:
                    return (differenceInDays / 365) <= rowLimit;
                default:
                    throw new ArgumentException("Invalid report interval");
            }
        }
    }
}
