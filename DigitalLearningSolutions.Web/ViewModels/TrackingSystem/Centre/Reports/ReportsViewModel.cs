namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public class ReportsViewModel
    {
        public UsageStatsTableViewModel UsageStatsTableViewModel { get; set; }

        public ReportsViewModel(IEnumerable<MonthOfActivity> monthsOfActivity)
        {
            UsageStatsTableViewModel = new UsageStatsTableViewModel(monthsOfActivity);
        }
    }

    public class UsageStatsTableViewModel
    {
        public UsageStatsTableViewModel(IEnumerable<MonthOfActivity> monthsOfActivity)
        {
            Rows = monthsOfActivity.Select(m => new UsageStatsTableRow(m));
        }

        public IEnumerable<UsageStatsTableRow> Rows { get; set; }
    }

    public class UsageStatsTableRow
    {
        public UsageStatsTableRow(MonthOfActivity monthOfActivity)
        {
            Period = DateTime.Parse($"{monthOfActivity.Year}-{monthOfActivity.Month}-01").ToString("MMMM yyyy");
            Completions = monthOfActivity.Completions;
            Evaluations = monthOfActivity.Evaluations;
            Registrations = monthOfActivity.Registrations;
        }

        public string Period { get; set; }
        public int Completions { get; set; }
        public int Evaluations { get; set; }
        public int Registrations { get; set; }
    }
}
