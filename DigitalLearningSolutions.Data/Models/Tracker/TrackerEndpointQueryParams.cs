namespace DigitalLearningSolutions.Data.Models.Tracker
{
    public class TrackerEndpointQueryParams
    {
        public string? Action { get; set; }
        public int? CustomisationId { get; set; }
        public int? SectionId { get; set; }
        public bool? IsPostLearning { get; set; }
    }
}
