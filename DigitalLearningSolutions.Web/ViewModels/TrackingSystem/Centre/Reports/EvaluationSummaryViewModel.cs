namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public class EvaluationSummaryViewModel
    {
        public EvaluationSummaryViewModel(EvaluationSummaryModel model)
        {
            Question = model.Question;
            if (model.ResponseCounts != null)
            {
                ResponsePercentages = model.ResponseCounts
                    .Select(x => new KeyValuePair<string, float>(x.Key, 100f * x.Value / model.TotalResponses))
                    .ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public string Question { get; set; }

        public Dictionary<string, float>? ResponsePercentages { get; set; }
    }
}
