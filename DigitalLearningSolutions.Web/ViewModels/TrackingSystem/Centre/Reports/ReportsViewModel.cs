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
            EvaluationSummaryData evaluationSummaryData
        )
        {
            UsageStatsTableViewModel = new UsageStatsTableViewModel(activity);
            ReportsFilterModel = filterModel;
            EvaluationSummaryViewModels =
                EvaluationSummaryMappingHelper.MapDataToEvaluationSummaryViewModels(evaluationSummaryData);
        }

        public UsageStatsTableViewModel UsageStatsTableViewModel { get; set; }
        public ReportsFilterModel ReportsFilterModel { get; set; }
        public IEnumerable<EvaluationSummaryViewModel> EvaluationSummaryViewModels { get; set; }
    }

    public class UsageStatsTableViewModel
    {
        public UsageStatsTableViewModel(IEnumerable<PeriodOfActivity> activity)
        {
            Rows = activity.Reverse().Select(
                p => new ActivityDataRowModel(p, DateHelper.GetFormatStringForDateInTable(p.DateInformation.Interval))
            );
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
            DateRange =
                $"{filterData.StartDate.ToString(DateHelper.StandardDateFormat)} - {filterData.EndDate.ToString(DateHelper.StandardDateFormat)}";
            ShowCourseCategoryFilter = userManagingAllCourses;
        }

        public string JobGroupName { get; set; }
        public string CourseCategoryName { get; set; }
        public string CourseName { get; set; }
        public string DateRange { get; set; }
        public string ReportIntervalName { get; set; }
        public bool ShowCourseCategoryFilter { get; set; }
    }
}
