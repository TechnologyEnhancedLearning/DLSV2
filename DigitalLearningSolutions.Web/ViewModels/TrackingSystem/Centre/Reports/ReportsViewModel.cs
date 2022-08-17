namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.Helpers;

    public class ReportsViewModel
    {
        public ReportsViewModel(
            IEnumerable<PeriodOfActivity> activity,
            ReportsFilterModel filterModel,
            IEnumerable<EvaluationResponseBreakdown> evaluationResponseBreakdowns,
            DateTime startDate,
            DateTime endDate,
            bool hasActivity,
            string category
        )
        {
            UsageStatsTableViewModel = new UsageStatsTableViewModel(activity, startDate, endDate);
            ReportsFilterModel = filterModel;
            EvaluationSummaryBreakdown =
                evaluationResponseBreakdowns.Select(model => new EvaluationSummaryViewModel(model));
            HasActivity = hasActivity;
            Category = category;
        }

        public UsageStatsTableViewModel UsageStatsTableViewModel { get; set; }
        public ReportsFilterModel ReportsFilterModel { get; set; }
        public IEnumerable<EvaluationSummaryViewModel> EvaluationSummaryBreakdown { get; set; }
        public bool HasActivity { get; set; }
        public string Category { get; set; }
    }

    public class UsageStatsTableViewModel
    {
        public UsageStatsTableViewModel(IEnumerable<PeriodOfActivity> activity, DateTime startDate, DateTime endDate)
        {
            activity = activity.ToList();

            if (activity.Count() <= 1)
            {
                Rows = activity.Select(
                    p => new ActivityDataRowModel(p, DateHelper.StandardDateFormat, startDate, endDate)
                );
            }
            else
            {
                var first = activity.First();
                var firstRow = first.DateInformation.Interval == ReportInterval.Days
                    ? new ActivityDataRowModel(
                        first,
                        DateHelper.GetFormatStringForDateInTable(first.DateInformation.Interval)
                    )
                    : new ActivityDataRowModel(first, DateHelper.StandardDateFormat, startDate, true);

                var last = activity.Last();
                var lastRow = last.DateInformation.Interval == ReportInterval.Days
                    ? new ActivityDataRowModel(
                        last,
                        DateHelper.GetFormatStringForDateInTable(last.DateInformation.Interval)
                    )
                    : new ActivityDataRowModel(last, DateHelper.StandardDateFormat, endDate, false);

                var middleRows = activity.Skip(1).SkipLast(1).Select(
                    p => new ActivityDataRowModel(
                        p,
                        DateHelper.GetFormatStringForDateInTable(p.DateInformation.Interval)
                    )
                );

                Rows = middleRows.Prepend(firstRow).Append(lastRow).Reverse();
            }
        }

        public IEnumerable<ActivityDataRowModel> Rows { get; set; }
    }

    public class ActivityDataRowModel
    {
        public ActivityDataRowModel(PeriodOfActivity periodOfActivity, string format)
        {
            Period = periodOfActivity.DateInformation.GetDateLabel(format);
            Completions = periodOfActivity.Completions;
            Evaluations = periodOfActivity.Evaluations;
            Registrations = periodOfActivity.Registrations;
        }

        public ActivityDataRowModel(
            PeriodOfActivity periodOfActivity,
            string format,
            DateTime boundaryDate,
            bool startRangeFromTerminator
        )
        {
            Period = periodOfActivity.DateInformation.GetDateRangeLabel(format, boundaryDate, startRangeFromTerminator);
            Completions = periodOfActivity.Completions;
            Evaluations = periodOfActivity.Evaluations;
            Registrations = periodOfActivity.Registrations;
        }

        public ActivityDataRowModel(
            PeriodOfActivity periodOfActivity,
            string format,
            DateTime startDate,
            DateTime endDate
        )
        {
            Period = DateInformation.GetDateRangeLabel(format, startDate, endDate);
            Completions = periodOfActivity.Completions;
            Evaluations = periodOfActivity.Evaluations;
            Registrations = periodOfActivity.Registrations;
        }

        public string Period { get; set; }
        public int Completions { get; set; }
        public int Evaluations { get; set; }
        public int Registrations { get; set; }
    }

    public class ReportsFilterModel
    {
        public ReportsFilterModel(
            ActivityFilterData filterData,
            string jobGroupName,
            string courseCategoryName,
            string courseNameString,
            bool userManagingAllCourses
        )
        {
            JobGroupName = jobGroupName;
            CourseCategoryName = courseCategoryName;
            CourseName = courseNameString;
            ReportIntervalName = Enum.GetName(typeof(ReportInterval), filterData.ReportInterval)!;
            StartDate = filterData.StartDate.ToString(DateHelper.StandardDateFormat);
            EndDate = filterData.EndDate?.ToString(DateHelper.StandardDateFormat) ?? "Today";
            ShowCourseCategoryFilter = userManagingAllCourses;
            FilterValues = new Dictionary<string, string>
            {
                { "jobGroupId", filterData.JobGroupId?.ToString() ?? "" },
                { "courseCategoryId", filterData.CourseCategoryId?.ToString() ?? "" },
                { "customisationId", filterData.CustomisationId?.ToString() ?? "" },
                { "startDate", filterData.StartDate.ToString() },
                { "endDate", filterData.EndDate?.ToString() ?? "" },
                { "reportInterval", filterData.ReportInterval.ToString() },
            };
        }

        public string JobGroupName { get; set; }
        public string CourseCategoryName { get; set; }
        public string CourseName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ReportIntervalName { get; set; }
        public bool ShowCourseCategoryFilter { get; set; }

        public Dictionary<string, string> FilterValues { get; set; }
    }
}
