namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public class ReportsViewModel
    {
        public ReportsViewModel(IEnumerable<PeriodOfActivity> activity, ReportsFilterModel filterModel)
        {
            UsageStatsTableViewModel = new UsageStatsTableViewModel(activity);
            ReportsFilterModel = filterModel;
        }

        public UsageStatsTableViewModel UsageStatsTableViewModel { get; set; }
        public ReportsFilterModel ReportsFilterModel { get; set; }
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
            DateTime startDate,
            DateTime endDate,
            string jobGroupName,
            string courseCategoryName,
            string customisationName,
            ReportInterval interval,
            bool userManagingAllCourses
        )
        {
            JobGroupName = jobGroupName;
            CourseCategoryName = courseCategoryName;
            CustomisationName = customisationName;
            ReportIntervalName = Enum.GetName(typeof(ReportInterval), interval)!;
            DateRange =
                $"{startDate.ToString(DateHelper.StandardDateFormat)} - {endDate.ToString(DateHelper.StandardDateFormat)}";
            ShowCourseCategories = userManagingAllCourses;
        }

        public string JobGroupName { get; set; }
        public string CourseCategoryName { get; set; }
        public string CustomisationName { get; set; }
        public string DateRange { get; set; }
        public string ReportIntervalName { get; set; }
        public bool ShowCourseCategories { get; set; }
    }
}
