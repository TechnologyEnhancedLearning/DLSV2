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
                    .Select(x => (x.response, FormatAsPercentageString(x.count, model.TotalResponses)));
            }
        }

        public string Question { get; set; }

        public IEnumerable<(string response, string percentageString)>? ResponsePercentages { get; set; }

        private static string FormatAsPercentageString(int count, int total)
        {
            return ((float)count / total).ToString("0.0%");
        }
    }
}
