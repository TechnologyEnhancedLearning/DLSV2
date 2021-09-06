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
        IEnumerable<PeriodOfActivity> GetFilteredActivity(int centreId, ActivityFilterData filterData);
        string GetJobGroupNameForActivityFilter(int? jobGroupId);
        string GetCourseCategoryNameForActivityFilter(int? courseCategoryName);
        string GetCourseNameForActivityFilter(int? courseId);
    }

    public class ActivityService : IActivityService
    {
        private readonly IActivityDataService activityDataService;
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly ICourseDataService courseDataService;
        private readonly IJobGroupsDataService jobGroupsDataService;

        public ActivityService(
            IActivityDataService activityDataService,
            IJobGroupsDataService jobGroupsDataService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICourseDataService courseDataService
        )
        {
            this.activityDataService = activityDataService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.courseDataService = courseDataService;
        }

        public IEnumerable<PeriodOfActivity> GetFilteredActivity(int centreId, ActivityFilterData filterData)
        {
            var activityData = activityDataService
                .GetFilteredActivity(
                    centreId,
                    filterData.StartDate,
                    filterData.EndDate,
                    filterData.JobGroupId,
                    filterData.CourseCategoryId,
                    filterData.CustomisationId
                )
                .OrderBy(x => x.LogDate);

            var dataByPeriod = GroupActivityData(activityData, filterData.ReportInterval);

            var dateSlots = DateHelper.GetPeriodsBetweenDates(
                filterData.StartDate,
                filterData.EndDate,
                filterData.ReportInterval
            );

            return dateSlots.Select(
                slot =>
                {
                    var dateInformation = new DateInformation(slot, filterData.ReportInterval);
                    var periodData = dataByPeriod.SingleOrDefault(
                        data => data.DateInformation.Date == slot.Date
                    );
                    return new PeriodOfActivity(dateInformation, periodData);
                }
            );
        }

        public string GetJobGroupNameForActivityFilter(int? jobGroupId)
        {
            var jobGroupNameString = jobGroupId.HasValue
                ? jobGroupsDataService.GetJobGroupName(jobGroupId.Value)
                : "All";
            jobGroupNameString ??= "All";

            return jobGroupNameString;
        }

        public string GetCourseCategoryNameForActivityFilter(int? courseCategoryName)
        {
            var courseCategoryNameString = courseCategoryName.HasValue
                ? courseCategoriesDataService.GetCourseCategoryName(courseCategoryName.Value)
                : "All";
            courseCategoryNameString ??= "All";

            return courseCategoryNameString;
        }

        public string GetCourseNameForActivityFilter(int? courseId)
        {
            var courseNames = courseId.HasValue
                ? courseDataService.GetCourseNameAndApplication(courseId.Value)
                : null;
            var courseNameString = courseNames?.CourseName ?? "All";

            return courseNameString;
        }

        private IEnumerable<PeriodOfActivity> GroupActivityData(
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
                _ => activityData.GroupBy(x => new DateTime(x.LogYear, 1, 1).Ticks)
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

        private static int GetFirstMonthOfQuarter(int quarter)
        {
            return quarter * 3 - 2;
        }
    }
}
