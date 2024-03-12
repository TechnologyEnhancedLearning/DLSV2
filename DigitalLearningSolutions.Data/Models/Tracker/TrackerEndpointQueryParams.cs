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
        public int? TutorialStatus { get; set; }
        public double? TutorialTime { get; set; }
        public int? CandidateId { get; set; }
        public int? Version { get; set; }
        public int? TutorialId { get; set; }
        public int? Score { get; set; }
        public string? SuspendData { get; set; }
        public string? LessonLocation { get; set; }
    }
}
