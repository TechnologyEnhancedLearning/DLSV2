namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IActivityService
    {
        IEnumerable<PeriodOfActivity> GetFilteredActivity(int centreId, ActivityFilterData filterData);

        (string jobGroupName, string courseCategoryName, string courseName) GetFilterNames(
            ActivityFilterData filterData
        );

        ReportsFilterOptions GetFilterOptions(int centreId, int? courseCategoryId);

        DateTime GetStartOfActivityForCentre(int centreId);

        byte[] GetActivityDataFileForCentre(int centreId, ActivityFilterData filterData);

        (DateTime startDate, DateTime? endDate)? GetValidatedUsageStatsDateRange(
            string startDateString,
            string endDateString,
            int centreId
        );
    }

    public class ActivityService : IActivityService
    {
        private const string SheetName = "Usage Statistics";
        private static readonly XLTableTheme TableTheme = XLTableTheme.TableStyleLight9;
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
                filterData.EndDate ?? DateTime.UtcNow,
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

        public (string jobGroupName, string courseCategoryName, string courseName) GetFilterNames(
            ActivityFilterData filterData
        )
        {
            return (GetJobGroupNameForActivityFilter(filterData.JobGroupId),
                GetCourseCategoryNameForActivityFilter(filterData.CourseCategoryId),
                GetCourseNameForActivityFilter(filterData.CustomisationId));
        }

        public ReportsFilterOptions GetFilterOptions(int centreId, int? courseCategoryId)
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var courseCategories = courseCategoriesDataService
                .GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
                .Select(cc => (cc.CourseCategoryID, cc.CategoryName));
            var courses = courseDataService
                .GetCentrallyManagedAndCentreCourses(centreId, courseCategoryId)
                .OrderBy(c => c.CourseName)
                .Select(c => (c.CustomisationId, c.CourseName));

            return new ReportsFilterOptions(jobGroups, courseCategories, courses);
        }

        public DateTime GetStartOfActivityForCentre(int centreId)
        {
            return activityDataService.GetStartOfActivityForCentre(centreId);
        }

        public byte[] GetActivityDataFileForCentre(int centreId, ActivityFilterData filterData)
        {
            using var workbook = new XLWorkbook();

            var activityData = GetFilteredActivity(centreId, filterData).Select(
                p => new { Period = p.DateInformation.Date, p.Registrations, p.Completions, p.Evaluations }
            );

            var sheet = workbook.Worksheets.Add(SheetName);
            var table = sheet.Cell(1, 1).InsertTable(activityData);
            table.Theme = TableTheme;
            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public (DateTime startDate, DateTime? endDate)? GetValidatedUsageStatsDateRange(
            string startDateString,
            string endDateString,
            int centreId
        )
        {
            var parsedStartDate = DateTime.Parse(startDateString);
            var start = GetStartOfActivityForCentre(centreId);
            if (parsedStartDate < GetStartOfActivityForCentre(centreId))
            {
                return null;
            }

            if (DateTime.TryParse(endDateString, out var parsedEndDate) &&
                (parsedEndDate < parsedStartDate || parsedEndDate > DateTime.Now))
            {
                return null;
            }

            return (parsedStartDate, parsedEndDate > DateTime.MinValue ? (DateTime?)parsedEndDate : null);
        }

        private string GetJobGroupNameForActivityFilter(int? jobGroupId)
        {
            var jobGroupName = jobGroupId.HasValue
                ? jobGroupsDataService.GetJobGroupName(jobGroupId.Value)
                : "All";
            return jobGroupName ?? "All";
        }

        private string GetCourseCategoryNameForActivityFilter(int? courseCategoryId)
        {
            var courseCategoryName = courseCategoryId.HasValue
                ? courseCategoriesDataService.GetCourseCategoryName(courseCategoryId.Value)
                : "All";
            return courseCategoryName ?? "All";
        }

        private string GetCourseNameForActivityFilter(int? courseId)
        {
            var courseNames = courseId.HasValue
                ? courseDataService.GetCourseNameAndApplication(courseId.Value)
                : null;
            return courseNames?.CourseName ?? "All";
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
