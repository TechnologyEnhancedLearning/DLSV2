namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System.Collections.Generic;

    public class EvaluationSummaryViewModel
    {
        public EvaluationSummaryViewModel(string question, Dictionary<string, float>? data)
        {
            Question = question;
            Data = data;
        }

        public string Question { get; set; }

        public Dictionary<string, float>? Data { get; set; }
    }
}
