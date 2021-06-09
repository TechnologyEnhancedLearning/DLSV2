namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IActivityService
    {
        public IEnumerable<MonthOfActivity> GetRecentActivity(
            int months,
            int centreId
        );
    }

    public class ActivityService : IActivityService
    {
        private readonly IActivityDataService activityDataService;

        public ActivityService(IActivityDataService activityDataService)
        {
            this.activityDataService = activityDataService;
        }

        public IEnumerable<MonthOfActivity> GetRecentActivity(int months, int centreId)
        {
            throw new NotImplementedException();
        }
    }
}
