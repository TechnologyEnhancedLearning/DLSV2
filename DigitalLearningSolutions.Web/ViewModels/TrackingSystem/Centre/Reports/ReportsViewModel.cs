namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public class ReportsViewModel
    {
        public UsageStatsTableViewModel UsageStatsTableViewModel { get; set; }
        public ActivityFilterModel ActivityFilterModel { get; set; }

        public ReportsViewModel(IEnumerable<PeriodOfActivity> activity, ActivityFilterModel filterModel)
        {
            UsageStatsTableViewModel = new UsageStatsTableViewModel(activity);
            ActivityFilterModel = filterModel;
        }
    }

    public class UsageStatsTableViewModel
    {
        public UsageStatsTableViewModel(IEnumerable<PeriodOfActivity> activity)
        {
            var periodicData = activity.ToList();
            periodicData.Reverse();
            Rows = periodicData.Select(m => new ActivityDataRowModel(m, false));
        }

        public IEnumerable<ActivityDataRowModel> Rows { get; set; }
    }

    public class ActivityDataRowModel
    {
        public ActivityDataRowModel(PeriodOfActivity periodOfActivity, bool shortForm)
        {
            Period = periodOfActivity.DateInformation.GetDateLabel(shortForm);
            Completions = periodOfActivity.Completions;
            Evaluations = periodOfActivity.Evaluations;
            Registrations = periodOfActivity.Registrations;
        }

        public string Period { get; set; }
        public int Completions { get; set; }
        public int Evaluations { get; set; }
        public int Registrations { get; set; }
    }

    public class ActivityFilterModel
    {
        public string? JobGroupName { get; set; }
        public string? CourseCategoryName { get; set; }
        public string? CustomisationName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ReportIntervalName { get; set; }
    }
}
