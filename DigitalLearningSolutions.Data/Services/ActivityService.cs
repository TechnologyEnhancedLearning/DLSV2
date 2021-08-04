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
                    var periodData = dataByPeriod.SingleOrDefault();
                    return new PeriodOfActivity(slot, periodData);
                });
        }

        private IEnumerable<PeriodOfActivity> GroupActivityData(IEnumerable<ActivityLog> activityData, ReportInterval interval)
        {
            IEnumerable<IGrouping<DateInformation, ActivityLog>> groupedData;

            if (Equals(interval, ReportInterval.Days))
            {
                groupedData = activityData.GroupBy(x => new DateInformation
                {
                    Interval = interval,
                    Day = x.LogDate.Day,
                    Month = x.LogMonth,
                    Year = x.LogYear
                });
            }
            else if (Equals(interval, ReportInterval.Weeks))
            {
                var referenceDate = new DateTime(1905, 1, 1);
                groupedData = activityData.GroupBy(x => new DateInformation
                {
                    Interval = interval,
                    WeekBeginning = referenceDate.AddDays((x.LogDate - referenceDate).Days / 7 * 7)
                });
            }
            else if (Equals(interval, ReportInterval.Months))
            {
                groupedData = activityData.GroupBy(x => new DateInformation
                {
                    Interval = interval,
                    Month = x.LogMonth,
                    Year = x.LogYear
                });
            }
            else if (Equals(interval, ReportInterval.Quarters))
            {
                groupedData = activityData.GroupBy(x => new DateInformation
                {
                    Interval = interval,
                    Quarter = x.LogQuarter,
                    Year = x.LogYear
                });
            }
            else
            {
                groupedData = activityData.GroupBy(x => new DateInformation
                {
                    Interval = interval,
                    Year = x.LogYear
                });
            }

            return groupedData.Select(
                x => new PeriodOfActivity(
                    x.Key,
                    x.Sum(y => y.Registration),
                    x.Sum(y => y.Completion),
                    x.Sum(y => y.Evaluation)
                    )
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
        public PeriodOfActivity(DateInformation slot, int registrations, int completions, int evaluations)
        {
            DateInformation = slot;
            Registrations = registrations;
            Completions = completions;
            Evaluations = evaluations;
        }

        public PeriodOfActivity(DateInformation slot, PeriodOfActivity? data)
        {
            DateInformation = slot;
            Completions = data?.Completions ?? 0;
            Evaluations = data?.Evaluations ?? 0;
            Registrations = data?.Registrations ?? 0;
        }

        public DateInformation DateInformation { get; set; }
        public int Completions { get; set; }
        public int Evaluations { get; set; }
        public int Registrations { get; set; }
    }
}
