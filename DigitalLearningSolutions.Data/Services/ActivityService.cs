﻿namespace DigitalLearningSolutions.Data.Services
{
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

            var monthsThisYear = Enumerable.Range(1, endTime.Month).Select(
                m => new MonthOfActivity
                {
                    Year = endTime.Year,
                    Month = m
                }
            );
            var monthsLastYear = Enumerable.Range(startTime.Month, 13 - startTime.Month).Select(
                m => new MonthOfActivity
                {
                    Year = startTime.Year,
                    Month = m
                }
            );

            var monthsOfActivity = monthsLastYear.Concat(monthsThisYear).ToList();
            monthsOfActivity.Reverse();

            foreach (var month in monthsOfActivity)
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

            return monthsOfActivity;
        }
    }
}
