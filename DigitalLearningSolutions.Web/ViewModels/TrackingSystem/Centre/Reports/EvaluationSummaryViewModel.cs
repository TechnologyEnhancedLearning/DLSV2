namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public class EvaluationSummaryViewModel
    {
        public EvaluationSummaryViewModel(EvaluationResponseBreakdown model)
        {
            Question = model.Question;
            if (model.ResponseCounts != null)
            {
                ResponsePercentages = model.ResponseCounts
                    .Select(x => (question: x.response, percentage: 100f * x.count / model.TotalResponses));
            }
        }

        public string Question { get; set; }

        public IEnumerable<(string response, float percentage)>? ResponsePercentages { get; set; }
    }
}
