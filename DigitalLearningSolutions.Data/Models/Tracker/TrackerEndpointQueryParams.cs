namespace DigitalLearningSolutions.Data.Models.Tracker
{
    public class TrackerEndpointQueryParams
    {
        public string? Action { get; set; }
        public int? CustomisationId { get; set; }
        public int? SectionId { get; set; }
        public string? IsPostLearning { get; set; }

        public int? ProgressId { get; set; }

        public string? DiagnosticOutcome { get; set; }
    }
}
