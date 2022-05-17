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
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IActivityService
    {
        IEnumerable<PeriodOfActivity> GetFilteredActivity(int centreId, ActivityFilterData filterData);

        (string jobGroupName, string courseCategoryName, string courseName) GetFilterNames(
            ActivityFilterData filterData
        );

        ReportsFilterOptions GetFilterOptions(int centreId, int? courseCategoryId);

        DateTime? GetActivityStartDateForCentre(int centreId, int? courseCategoryId = null);

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
                        data => data.DateInformation.StartDate == slot.Date
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

            var availableCourses = courseDataService
                .GetCoursesAvailableToCentreByCategory(centreId, courseCategoryId);
            var historicalCourses = courseDataService
                .GetCoursesEverUsedAtCentreByCategory(centreId, courseCategoryId);

            var courses = availableCourses.Union(historicalCourses, new CourseEqualityComparer())
                .OrderByDescending(c => c.Active)
                .ThenBy(c => c.CourseName)
                .Select(c => (c.CustomisationId, c.CourseNameWithInactiveFlag));

            return new ReportsFilterOptions(jobGroups, courseCategories, courses);
        }

        public DateTime? GetActivityStartDateForCentre(int centreId, int? courseCategoryId = null)
        {
            var activityStart = activityDataService.GetStartOfActivityForCentre(centreId, courseCategoryId);
            return activityStart?.Date ?? activityStart;
        }

        public byte[] GetActivityDataFileForCentre(int centreId, ActivityFilterData filterData)
        {
            using var workbook = new XLWorkbook();

            var activityData = GetFilteredActivity(centreId, filterData).ToList();

            IEnumerable<WorkbookRow> workbookData;

            if (activityData.Count() <= 1)
            {
                workbookData = activityData.Select(
                    p => new WorkbookRow(
                        DateInformation.GetDateRangeLabel(
                            DateHelper.StandardDateFormat,
                            filterData.StartDate,
                            filterData.EndDate ?? DateTime.Now
                        ),
                        p.Registrations,
                        p.Completions,
                        p.Evaluations
                    )
                );
            }
            else
            {
                var first = activityData.First();
                var firstRow = new WorkbookRow(
                    first.DateInformation.GetDateRangeLabel(DateHelper.StandardDateFormat, filterData.StartDate, true),
                    first.Registrations,
                    first.Completions,
                    first.Evaluations
                );

                var last = activityData.Last();
                var lastRow = new WorkbookRow(
                    last.DateInformation.GetDateRangeLabel(
                        DateHelper.StandardDateFormat,
                        filterData.EndDate ?? DateTime.Now,
                        true
                    ),
                    last.Registrations,
                    last.Completions,
                    last.Evaluations
                );

                var middleRows = activityData.Skip(1).SkipLast(1).Select(
                    p => new WorkbookRow(
                        p.DateInformation.GetDateLabel(DateHelper.StandardDateFormat),
                        p.Registrations,
                        p.Completions,
                        p.Evaluations
                    )
                );
                workbookData = middleRows.Prepend(firstRow).Append(lastRow);
            }

            var sheet = workbook.Worksheets.Add(SheetName);
            var table = sheet.Cell(1, 1).InsertTable(workbookData);
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
            var startDateInvalid = !DateTime.TryParse(startDateString, out var startDate);
            var activityStart = GetActivityStartDateForCentre(centreId);
            if (startDateInvalid || activityStart == null || startDate < activityStart)
            {
                return null;
            }

            var endDateIsSet = DateTime.TryParse(endDateString, out var endDate);

            if (endDateIsSet && (endDate < startDate || endDate > DateTime.Now))
            {
                return null;
            }

            return (startDate, endDateIsSet ? endDate : (DateTime?)null);
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

        private static int GetFirstMonthOfQuarter(int quarter)
        {
            return quarter * 3 - 2;
        }

        private class WorkbookRow
        {
            public WorkbookRow(string period, int registrations, int completions, int evaluations)
            {
                Period = period;
                Registrations = registrations;
                Completions = completions;
                Evaluations = evaluations;
            }

            public string Period { get; }
            public int Registrations { get; }
            public int Completions { get; }
            public int Evaluations { get; }
        }

        private class CourseEqualityComparer : IEqualityComparer<Course>
        {
            public bool Equals(Course? x, Course? y)
            {
                return x?.CustomisationId == y?.CustomisationId;
            }

            public int GetHashCode(Course obj)
            {
                return obj.CustomisationId;
            }
        }
    }
}
