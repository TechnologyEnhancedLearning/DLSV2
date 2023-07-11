namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;

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
            UsageStatsTableViewModel = new ActivityTableViewModel(activity, startDate, endDate);
            ReportsFilterModel = filterModel;
            EvaluationSummaryBreakdown =
                evaluationResponseBreakdowns.Select(model => new EvaluationSummaryViewModel(model));
            HasActivity = hasActivity;
            Category = category;
        }

        public ActivityTableViewModel UsageStatsTableViewModel { get; set; }
        public ReportsFilterModel ReportsFilterModel { get; set; }
        public IEnumerable<EvaluationSummaryViewModel> EvaluationSummaryBreakdown { get; set; }
        public bool HasActivity { get; set; }
        public string Category { get; set; }
    }
}
