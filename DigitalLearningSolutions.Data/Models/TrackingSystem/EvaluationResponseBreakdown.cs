namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    using System.Collections.Generic;
    using System.Linq;

    public class EvaluationResponseBreakdown
    {
        public EvaluationResponseBreakdown(string question, IEnumerable<(string response, int count)> responseCounts)
        {
            Question = question;
            var totalResponses = responseCounts.Sum(x => x.count);
            ResponseCounts = totalResponses != 0 ? responseCounts : null;
        }

        public string Question { get; set; }

        public IEnumerable<(string response, int count)>? ResponseCounts { get; set; }

        public int TotalResponses => ResponseCounts?.Sum(x => x.count) ?? 0;
    }
}
