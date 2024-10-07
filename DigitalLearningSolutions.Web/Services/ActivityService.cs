namespace DigitalLearningSolutions.Web.Services
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
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Configuration;

    public interface IActivityService
    {
        IEnumerable<PeriodOfActivity> GetFilteredActivity(int centreId, ActivityFilterData filterData);
        DateTime? GetActivityStartDateForCentre(int centreId, int? courseCategoryId = null);
        byte[] GetActivityDataFileForCentre(int centreId, ActivityFilterData filterData);
        (DateTime startDate, DateTime? endDate)? GetValidatedUsageStatsDateRange(
            string startDateString,
            string endDateString,
            int centreId
        );
        string GetCourseCategoryNameForActivityFilter(int? categoryId);
    }

    public class ActivityService : IActivityService
    {
        private const string SheetName = "Usage summary";
        private static readonly XLTableTheme TableTheme = XLTableTheme.TableStyleLight9;
        private readonly IActivityDataService activityDataService;
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly ICourseDataService courseDataService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IClockUtility clockUtility;
        private readonly IConfiguration configuration;
        private readonly ICentreRegistrationPromptsService registrationPromptsService;

        public ActivityService(
            IActivityDataService activityDataService,
            IJobGroupsDataService jobGroupsDataService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICourseDataService courseDataService,
            IClockUtility clockUtility,
            IConfiguration configuration,
            ICentreRegistrationPromptsService registrationPromptsService
        )
        {
            this.activityDataService = activityDataService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.courseDataService = courseDataService;
            this.clockUtility = clockUtility;
            this.configuration = configuration;
            this.registrationPromptsService = registrationPromptsService;
        }
        public int GetActivityDetailRowCount(int centreId, ActivityFilterData filterData)
        {
            return activityDataService.GetActivityDetailRowCount(centreId,
                                filterData.StartDate,
                                filterData.EndDate,
                                filterData.JobGroupId,
                                filterData.CourseCategoryId,
                                filterData.CustomisationId);
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
                            filterData.EndDate ?? clockUtility.UtcNow
                        ),
                        p.Enrolments,
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
                    first.Enrolments,
                    first.Completions,
                    first.Evaluations
                );

                var last = activityData.Last();
                var lastRow = new WorkbookRow(
                    last.DateInformation.GetDateRangeLabel(
                        DateHelper.StandardDateFormat,
                        filterData.EndDate ?? clockUtility.UtcNow,
                        false
                    ),
                    last.Enrolments,
                    last.Completions,
                    last.Evaluations
                );

                var middleRows = activityData.Skip(1).SkipLast(1).Select(
                    p => new WorkbookRow(
                        p.DateInformation.GetDateLabel(DateHelper.StandardDateFormat),
                        p.Enrolments,
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
            AddActivityDetailSheet(workbook, centreId, filterData);
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

            if (endDateIsSet && (endDate < startDate || endDate > clockUtility.UtcNow))
            {
                return null;
            }

            return (startDate, endDateIsSet ? endDate : (DateTime?)null);
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
                    groupingOfLogs.Sum(activityLog => activityLog.Registered),
                    groupingOfLogs.Sum(activityLog => activityLog.Completed),
                    groupingOfLogs.Sum(activityLog => activityLog.Evaluated)
                )
            );
        }
        private void AddActivityDetailSheet(XLWorkbook workbook, int centreId, ActivityFilterData filterData)
        {
            var itemsPerPage = Data.Extensions.ConfigurationExtensions.GetExportQueryRowLimit(configuration);
            var resultCount = GetActivityDetailRowCount(centreId, filterData);
            if (resultCount == 0)
            {
                return;
            }
            int totalRun = (int)(resultCount / itemsPerPage) + ((resultCount % itemsPerPage) > 0 ? 1 : 0);
            int currentRun = 1;
            List<ActivityLogDetail> activityLogDetails = new List<ActivityLogDetail>();
            while (totalRun >= currentRun)
            {
                activityLogDetails.AddRange(activityDataService.GetFilteredActivityDetail(
                    centreId,
                    filterData.StartDate,
                    filterData.EndDate,
                    filterData.JobGroupId,
                    filterData.CourseCategoryId,
                    filterData.CustomisationId,
                    itemsPerPage,
                    currentRun));
                currentRun++;
            }
            var customRegistrationPrompts = registrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);
            var sheet = workbook.Worksheets.Add("Usage detail");
            var table = sheet.Cell(1, 1).InsertTable(activityLogDetails);
            table.Theme = TableTheme;
            table.Field(0).Name = "Date";
            foreach (var prompt in customRegistrationPrompts.CustomPrompts)
            {
                var promptName = prompt.PromptText;
                var fieldNum = prompt.RegistrationField.ToString().Last();
                var promptLabel = "Answer" + fieldNum;
                table.Field(promptLabel).Name = promptName;
            }
            for (int i = 1; i < 7; i++)
            {
                var answerLabel = "Answer" + i.ToString();

                if (table.Fields.Any(f => f.Name == answerLabel))
                {
                    table.Field(answerLabel).Delete();
                }
            }
            sheet.Columns().AdjustToContents();
        }
        private static int GetFirstMonthOfQuarter(int quarter)
        {
            return quarter * 3 - 2;
        }

        public string GetCourseCategoryNameForActivityFilter(int? courseCategoryId)
        {
            var courseCategoryName = courseCategoryId.HasValue
                ? courseCategoriesDataService.GetCourseCategoryName(courseCategoryId.Value)
                : "All";
            return courseCategoryName ?? "All";
        }

        private class WorkbookRow
        {
            public WorkbookRow(string period, int enrolments, int completions, int evaluations)
            {
                Period = period;
                Enrolments = enrolments;
                Completions = completions;
                Evaluations = evaluations;
            }

            public string Period { get; }
            public int Enrolments { get; }
            public int Completions { get; }
            public int Evaluations { get; }
        }
    }
}
