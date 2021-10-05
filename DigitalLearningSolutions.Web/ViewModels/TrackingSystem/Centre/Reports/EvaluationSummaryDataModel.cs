namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public class EvaluationSummaryDataModel
    {
        public EvaluationSummaryDataModel(EvaluationResponseBreakdown model)
        {
            Id = new string(model.Question.Where(char.IsLetter).ToArray());
            ResponseCounts = model.ResponseCounts?.Select(c => new ResponseCount(c.response, c.count));
        }

        public string Id { get; }

        public IEnumerable<ResponseCount>? ResponseCounts { get; }
    }

    public class ResponseCount
    {
        public ResponseCount(string name, int count)
        {
            Name = new string(name.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
            Count = count;
        }

        public string Name { get; }

        public int Count { get; }
    }
}
