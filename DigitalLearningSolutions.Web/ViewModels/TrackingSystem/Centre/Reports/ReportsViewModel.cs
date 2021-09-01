namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public class ReportsViewModel
    {
        public ReportsViewModel(IEnumerable<PeriodOfActivity> activity)
        {
            UsageStatsTableViewModel = new UsageStatsTableViewModel(activity);

            EvaluationSummaryViewModels = new List<EvaluationSummaryViewModel>();
        }

        public UsageStatsTableViewModel UsageStatsTableViewModel { get; set; }
        public List<EvaluationSummaryViewModel> EvaluationSummaryViewModels { get; set; }
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
}
