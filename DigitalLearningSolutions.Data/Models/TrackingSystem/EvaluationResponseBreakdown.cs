namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    using System.Collections.Generic;
    using System.Linq;

    public class EvaluationResponseBreakdown
    {
        public EvaluationResponseBreakdown(string question, IEnumerable<(string response, int count)> responseCounts)
        {
            Question = question;
            var responses = responseCounts.ToList();
            var totalResponses = responses.Sum(x => x.count);
            Responses = totalResponses != 0
                ? responses.Select(x => new EvaluationResponses(x.response, x.count, totalResponses))
                : null;
        }

        public string Question { get; set; }

        public IEnumerable<EvaluationResponses>? Responses { get; set; }

        public int TotalResponses => Responses?.Sum(x => x.Count) ?? 0;
    }
}
