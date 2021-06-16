﻿namespace DigitalLearningSolutions.Data.Services
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

        public ActivityService(IActivityDataService activityDataService)
        {
            this.activityDataService = activityDataService;
        }

        public IEnumerable<MonthOfActivity> GetRecentActivity(int centreId)
        {
            var date = DateTime.Now;
            var currentYear = date.Year;
            var monthsToDate = Enumerable.Range(1, date.Month);

            var activity = activityDataService.GetActivityForMonthsInYear(currentYear, monthsToDate);

            if (date.Month < 12)
            {
                var monthsLastYear = Enumerable.Range(date.Month + 1, 12);
                var lastYearActivity = activityDataService.GetActivityForMonthsInYear(currentYear - 1, monthsLastYear);
                activity = lastYearActivity.Concat(activity);
            }

            return activity;
        }
    }
}
