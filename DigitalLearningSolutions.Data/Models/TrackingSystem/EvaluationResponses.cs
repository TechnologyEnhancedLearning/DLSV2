namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    public class EvaluationResponses
    {
        public EvaluationResponses(string response, int count, int totalResponses)
        {
            Response = response;
            Count = count;
            Percentage = (float)count / totalResponses;
        }

        public string Response { get; set; }

        public int Count { get; set; }

        public float Percentage { get; set; }
    }
}
