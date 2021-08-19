namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IActivityService
    {
        public IEnumerable<PeriodOfActivity> GetFilteredActivity(int centreId, ActivityFilterData filterData);
    }

    public class ActivityService : IActivityService
    {
        private readonly IActivityDataService activityDataService;
        private readonly DateTime referenceDate = new DateTime(1905, 1, 1);

        public ActivityService(IActivityDataService activityDataService)
        {
            this.activityDataService = activityDataService;
        }

        public IEnumerable<PeriodOfActivity> GetFilteredActivity(int centreId, ActivityFilterData filterData)
        {
            var activityData = activityDataService.GetRawActivity(centreId, filterData);

            var dataByPeriod = GroupActivityData(activityData, filterData.ReportInterval);

            var dateSlots = DateHelper.GetPeriodsBetweenDates(
                filterData.StartDate,
                filterData.EndDate,
                filterData.ReportInterval
            );

            return dateSlots.Select(
                slot =>
                {
                    var periodData = dataByPeriod.SingleOrDefault(
                        data => data.DateInformation.Date == slot.Date
                    );
                    return new PeriodOfActivity(slot, periodData);
                }
            );
        }

        private IEnumerable<PeriodOfActivity> GroupActivityData(
            IEnumerable<ActivityLog> activityData,
            ReportInterval interval
        )
        {
            IEnumerable<IGrouping<long, ActivityLog>> groupedData;

            switch (interval)
            {
                case ReportInterval.Days:
                    groupedData = activityData.GroupBy(x => new DateTime(x.LogYear, x.LogMonth, x.LogDate.Day).Ticks);
                    break;
                case ReportInterval.Weeks:
                    groupedData = activityData.GroupBy(
                        x => referenceDate.AddDays((x.LogDate - referenceDate).Days / 7 * 7).Ticks
                    );
                    break;
                case ReportInterval.Months:
                    groupedData = activityData.GroupBy(x => new DateTime(x.LogYear, x.LogMonth, 1).Ticks);
                    break;
                case ReportInterval.Quarters:
                    groupedData = activityData.GroupBy(x => new DateTime(x.LogYear, x.LogQuarter * 3 - 2, 1).Ticks);
                    break;
                default:
                    groupedData = activityData.GroupBy(x => new DateTime(x.LogYear, 1, 1).Ticks);
                    break;
            }

            return groupedData.Select(
                x => new PeriodOfActivity(
                    new DateInformation
                    {
                        Interval = interval,
                        Date = new DateTime(x.Key)
                    },
                    x.Count(y => y.Registered),
                    x.Count(y => y.Completed),
                    x.Count(y => y.Evaluated)
                )
            );
        }
    }
}
