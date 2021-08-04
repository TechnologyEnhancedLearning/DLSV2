namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.Helpers;

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

        public void GetFilteredActivity(int centreId, ActivityFilterData filterData)
        {
            var activityData = activityDataService.GetRawActivity(centreId, filterData);

            var dataByPeriod = GroupActivityData(activityData, filterData.ReportInterval, filterData.EndDate);

            // now need to deal with empty periods
        }

        private IEnumerable<PeriodOfActivity> GroupActivityData(IEnumerable<ActivityLog> activityData, ReportInterval interval, DateTime endDate)
        {
            IEnumerable<IGrouping<DateInformation, ActivityLog>> groupedData;

            switch (interval)
            {
                case ReportInterval.Days:
                    groupedData = activityData.GroupBy(x => new DateInformation
                    {
                        Day = x.LogDate.Day,
                        Month = x.LogMonth,
                        Year = x.LogYear
                    });
                    break;
                case ReportInterval.Weeks:
                    var referenceDate = new DateTime(1905, 1, 1);
                    groupedData = activityData.GroupBy(x => new DateInformation
                    {
                        WeekBeginning = referenceDate.AddDays((x.LogDate - referenceDate).Days / 7 * 7)
                    });
                    break;
                case ReportInterval.Months:
                    groupedData = activityData.GroupBy(x => new DateInformation
                    {
                        Month = x.LogMonth,
                        Year = x.LogYear
                    });
                    break;
                case ReportInterval.Quarters:
                    groupedData = activityData.GroupBy(x => new DateInformation
                    {
                        Quarter = x.LogQuarter,
                        Year = x.LogYear
                    });
                    break;
                default:
                    groupedData = activityData.GroupBy(x => new DateInformation
                    {
                        Year = x.LogYear
                    });
                    break;
            }

            return groupedData.Select(
                x => new PeriodOfActivity
                {
                    DateInformation = x.Key,
                    Registrations = x.Sum(y => y.Registration),
                    Completions = x.Sum(y => y.Completion),
                    Evaluations = x.Sum(y => y.Evaluation)
                }
            );
        }
    }

    public class DateInformation
    {
        public ReportInterval Interval { get; set; }
        public int Day { get; set; }
        public DateTime? WeekBeginning { get; set; }
        public int Month { get; set; }
        public int Quarter { get; set; }
        public int Year { get; set; }
    }

    public class PeriodOfActivity
    {
        public DateInformation DateInformation { get; set; }
        public int Completions { get; set; }
        public int Evaluations { get; set; }
        public int Registrations { get; set; }
    }
}
