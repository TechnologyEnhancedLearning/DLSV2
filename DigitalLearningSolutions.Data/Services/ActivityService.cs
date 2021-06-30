namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IActivityService
    {
        public IEnumerable<MonthOfActivity> GetRecentActivity(int centreId);
    }

    public class ActivityService : IActivityService
    {
        private readonly IActivityDataService activityDataService;
        private readonly IClockService clockService;

        public ActivityService(IActivityDataService activityDataService, IClockService clockService)
        {
            this.activityDataService = activityDataService;
            this.clockService = clockService;
        }

        public IEnumerable<MonthOfActivity> GetRecentActivity(int centreId)
        {
            var endTime = clockService.UtcNow;
            var startTime = endTime.AddYears(-1);

            var activityData = activityDataService.GetActivityInRangeByMonth(centreId, startTime, endTime).ToList();

            var months = GenerateBlankMonthsBetweenDates(startTime, endTime).ToList();
            months.Reverse();

            foreach (var month in months)
            {
                var monthData =
                    activityData.SingleOrDefault(data => data.Year == month.Year && data.Month == month.Month);
                if (monthData != null)
                {
                    month.Completions = monthData.Completions;
                    month.Registrations = monthData.Registrations;
                    month.Evaluations = monthData.Evaluations;
                }
            }

            return months;
        }

        private IEnumerable<MonthOfActivity> GenerateBlankMonthsBetweenDates(DateTime startDate, DateTime endDate)
        {
            var diffInMonths = (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month);
            var monthEnumerable = Enumerable.Range(startDate.Month, diffInMonths + 1);

            return monthEnumerable.Select(
                m => new MonthOfActivity
                {
                    Month = (m - 1) % 12 + 1,
                    Year = startDate.Year + (m - 1) / 12
                }
            );
        }
    }
}
