namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Services;

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
}
