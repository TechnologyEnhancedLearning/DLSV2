namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public class ReportsViewModel
    {
        public UsageStatsTableViewModel UsageStatsTableViewModel { get; set; }

        public ReportsViewModel(IEnumerable<PeriodOfActivity> activity)
        {
            UsageStatsTableViewModel = new UsageStatsTableViewModel(activity);
        }
    }

    public class UsageStatsTableViewModel
    {
        public UsageStatsTableViewModel(IEnumerable<PeriodOfActivity> activity)
        {
            var periodicData = activity;
            periodicData = periodicData.Reverse();
            Rows = periodicData.Select(p => new ActivityDataRowModel(p, p.DateInformation.GetFormatStringForUsageStatsTable()));
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
}
