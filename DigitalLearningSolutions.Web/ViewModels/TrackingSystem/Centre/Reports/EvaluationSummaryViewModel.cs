namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System.Collections.Generic;
    using System.Linq;

    public class EvaluationSummaryViewModel
    {
        public EvaluationSummaryViewModel(string question, Dictionary<string, int> counts, int noResponseCount)
        {
            Question = question;
            var totalResponses = counts.Sum(x => x.Value);
            if (totalResponses > noResponseCount)
            {
                Data = counts
                    .Select(x => new KeyValuePair<string, float>(x.Key, 100f * x.Value / totalResponses))
                    .ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public string Question { get; set; }

        public Dictionary<string, float>? Data { get; set; }
    }
}
