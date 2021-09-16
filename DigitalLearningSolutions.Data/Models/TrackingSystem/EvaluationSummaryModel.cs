namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    using System.Collections.Generic;
    using System.Linq;

    public class EvaluationSummaryModel
    {
        public EvaluationSummaryModel(string question, Dictionary<string, int> responseCounts)
        {
            Question = question;
            var totalResponses = responseCounts.Sum(x => x.Value);
            ResponseCounts = totalResponses != 0 ? responseCounts : null;
        }

        public string Question { get; set; }

        public Dictionary<string, int>? ResponseCounts { get; set; }

        public int TotalResponses => ResponseCounts?.Sum(x => x.Value) ?? 0;
    }
}
