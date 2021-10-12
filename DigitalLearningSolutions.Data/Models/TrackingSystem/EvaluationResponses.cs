namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    public class EvaluationResponses
    {
        public EvaluationResponses(string response, int count, int totalResponses)
        {
            Response = response;
            Count = count;
            DecimalPercentage = (float)count / totalResponses;
        }

        public string Response { get; set; }

        public int Count { get; set; }

        public float DecimalPercentage { get; set; }
    }
}
