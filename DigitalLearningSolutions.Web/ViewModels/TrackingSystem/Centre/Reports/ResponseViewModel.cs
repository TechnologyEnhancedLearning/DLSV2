namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public class ResponseViewModel
    {
        public ResponseViewModel(string response, int count, string percentage)
        {
            Response = response;
            Count = count;
            Percentage = percentage;
        }

        public ResponseViewModel(EvaluationResponses evaluationResponses)
        {
            Response = evaluationResponses.Response;
            Count = evaluationResponses.Count;
            Percentage = evaluationResponses.Percentage.ToString("0.0%");
        }

        public string Response { get; set; }

        public int Count { get; set; }

        public string Percentage { get; set; }
    }
}
