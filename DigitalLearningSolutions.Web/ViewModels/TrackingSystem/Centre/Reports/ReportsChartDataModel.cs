using DigitalLearningSolutions.Data.Models.TrackingSystem;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;

    public class ReportsChartDataModel
    {
        public ReportsChartDataModel(IEnumerable<PeriodOfActivity> activity, IEnumerable<EvaluationResponseBreakdown> evaluationResponseBreakdowns)
        {
            ActivityGraphData = activity.Select(
                p => new ActivityDataRowModel(p, DateHelper.GetFormatStringForGraphLabel(p.DateInformation.Interval))
            );
            EvaluationSummariesData = evaluationResponseBreakdowns.Select(e => new EvaluationSummaryDataModel(e));
        }

        public IEnumerable<ActivityDataRowModel> ActivityGraphData { get; }
        public IEnumerable<EvaluationSummaryDataModel> EvaluationSummariesData { get; }
    }
}
