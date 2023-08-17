namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Utilities;

    public interface IPlatformReportsService
    {
        PlatformUsageSummary GetPlatformUsageSummary();
        IEnumerable<SelfAssessmentActivityInPeriod> GetSelfAssessmentActivity(ActivityFilterData filterData, bool supervised);
        DateTime GetSelfAssessmentActivityStartDate(bool supervised);
        IEnumerable<PeriodOfActivity> GetFilteredCourseActivity(ActivityFilterData filterData);
        DateTime? GetStartOfCourseActivity();
    }

    public class PlatformReportsService : IPlatformReportsService
    {

        private readonly IPlatformReportsDataService platformReportsDataService;
        private readonly IClockUtility clockUtility;
        public PlatformReportsService(
            IPlatformReportsDataService platformReportsDataService,
            IClockUtility clockUtility
            )
        {
            this.platformReportsDataService = platformReportsDataService;
            this.clockUtility = clockUtility;
        }

        public PlatformUsageSummary GetPlatformUsageSummary()
        {
            return platformReportsDataService.GetPlatformUsageSummary();
        }
        public IEnumerable<SelfAssessmentActivityInPeriod> GetSelfAssessmentActivity(ActivityFilterData filterData, bool supervised)
        {
            var activityData = platformReportsDataService.GetSelfAssessmentActivity(
                filterData.CentreId,
                filterData.CentreTypeId,
                filterData.StartDate,
                filterData.EndDate,
                filterData.JobGroupId,
                filterData.CourseCategoryId,
                filterData.BrandId,
                filterData.RegionId,
                filterData.SelfAssessmentId,
                supervised
            ).OrderBy(x => x.ActivityDate);
            var dataByPeriod = GroupSelfAssessmentActivityData(activityData, filterData.ReportInterval);
            var dateSlots = DateHelper.GetPeriodsBetweenDates(
                filterData.StartDate,
                filterData.EndDate ?? clockUtility.UtcNow,
                filterData.ReportInterval
                );
            return dateSlots.Select(
                slot =>
                {
                    var dateInformation = new DateInformation(slot, filterData.ReportInterval);
                    var periodData = dataByPeriod.SingleOrDefault(
                        data => data.DateInformation.StartDate == slot.Date
                    );
                    return new SelfAssessmentActivityInPeriod(dateInformation, periodData);
                }
            );
        }
       
        private IEnumerable<SelfAssessmentActivityInPeriod> GroupSelfAssessmentActivityData(
           IEnumerable<SelfAssessmentActivity> activityData,
           ReportInterval interval
       )
        {
            var referenceDate = DateHelper.ReferenceDate;

            var groupedActivityLogs = interval switch
            {
                ReportInterval.Days => activityData.GroupBy(
                    x => new DateTime(x.ActivityDate.Year, x.ActivityDate.Month, x.ActivityDate.Day).Ticks
                ),
                ReportInterval.Weeks => activityData.GroupBy(
                    activityLog => referenceDate.AddDays((activityLog.ActivityDate - referenceDate).Days / 7 * 7).Ticks
                ),
                ReportInterval.Months => activityData.GroupBy(x => new DateTime(x.ActivityDate.Year, x.ActivityDate.Month, 1).Ticks),
                ReportInterval.Quarters => activityData.GroupBy(
                    x => new DateTime(x.ActivityDate.Year, GetFirstMonthOfQuarter((int)Math.Floor(((decimal)x.ActivityDate.Month + 2) / 3)), 1).Ticks
                ),
                _ => activityData.GroupBy(x => new DateTime(x.ActivityDate.Year, 1, 1).Ticks),
            };

            return groupedActivityLogs.Select(
                groupingOfLogs => new SelfAssessmentActivityInPeriod(
                    new DateInformation(
                        new DateTime(groupingOfLogs.Key),
                        interval
                    ),
                    groupingOfLogs.Count(activityLog => activityLog.Enrolled),
                    groupingOfLogs.Count(activityLog => activityLog.Completed)
                )
            );
        }
        private static int GetFirstMonthOfQuarter(int quarter)
        {
            return quarter * 3 - 2;
        }

        public DateTime GetSelfAssessmentActivityStartDate(bool supervised)
        {
            return platformReportsDataService.GetSelfAssessmentActivityStartDate(supervised);
        }

        public IEnumerable<PeriodOfActivity> GetFilteredCourseActivity(ActivityFilterData filterData)
        {
            var activityData = platformReportsDataService.GetFilteredCourseActivity(
                filterData.CentreId,
                filterData.CentreTypeId,
                filterData.StartDate,
                filterData.EndDate,
                filterData.JobGroupId,
                filterData.CourseCategoryId,
                filterData.BrandId,
                filterData.RegionId,
                filterData.CustomisationId
            ).OrderBy(x => x.LogDate);
            var dataByPeriod = GroupCourseActivityData(activityData, filterData.ReportInterval);
            var dateSlots = DateHelper.GetPeriodsBetweenDates(
                filterData.StartDate,
                filterData.EndDate ?? clockUtility.UtcNow,
                filterData.ReportInterval
                );
            return dateSlots.Select(
                slot =>
                {
                    var dateInformation = new DateInformation(slot, filterData.ReportInterval);
                    var periodData = dataByPeriod.SingleOrDefault(
                        data => data.DateInformation.StartDate == slot.Date
                    );
                    return new PeriodOfActivity(dateInformation, periodData);
                }
            );
        }
        
        private IEnumerable<PeriodOfActivity> GroupCourseActivityData(
            IEnumerable<ActivityLog> activityData,
            ReportInterval interval
        )
        {
            var referenceDate = DateHelper.ReferenceDate;

            var groupedActivityLogs = interval switch
            {
                ReportInterval.Days => activityData.GroupBy(
                    x => new DateTime(x.LogYear, x.LogMonth, x.LogDate.Day).Ticks
                ),
                ReportInterval.Weeks => activityData.GroupBy(
                    activityLog => referenceDate.AddDays((activityLog.LogDate - referenceDate).Days / 7 * 7).Ticks
                ),
                ReportInterval.Months => activityData.GroupBy(x => new DateTime(x.LogYear, x.LogMonth, 1).Ticks),
                ReportInterval.Quarters => activityData.GroupBy(
                    x => new DateTime(x.LogYear, GetFirstMonthOfQuarter(x.LogQuarter), 1).Ticks
                ),
                _ => activityData.GroupBy(x => new DateTime(x.LogYear, 1, 1).Ticks),
            };

            return groupedActivityLogs.Select(
                groupingOfLogs => new PeriodOfActivity(
                    new DateInformation(
                        new DateTime(groupingOfLogs.Key),
                        interval
                    ),
                    groupingOfLogs.Count(activityLog => activityLog.Registered),
                    groupingOfLogs.Count(activityLog => activityLog.Completed),
                    groupingOfLogs.Count(activityLog => activityLog.Evaluated)
                )
            );
        }

        public DateTime? GetStartOfCourseActivity()
        {
            return platformReportsDataService.GetStartOfCourseActivity();
        }
    }
}
