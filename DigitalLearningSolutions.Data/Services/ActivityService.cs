namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Helpers;

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

            var monthSlots = DateHelper.GetMonthsAndYearsBetweenDates(startTime, endTime).ToList();

            return monthSlots.Select(
                slot =>
                {
                    var monthData =
                        activityData.SingleOrDefault(data => data.Year == slot.Year && data.Month == slot.Month);
                    return new MonthOfActivity(slot, monthData);
                }
            );
        }
    }
}
