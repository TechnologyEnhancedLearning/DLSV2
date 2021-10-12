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
            Responses = model.Responses
                .Select(x => new ResponseViewModel(x));
        }

        public string Question { get; set; }

        public IEnumerable<ResponseViewModel> Responses { get; set; }
    }
}
