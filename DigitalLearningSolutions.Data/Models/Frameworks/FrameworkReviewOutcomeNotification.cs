namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class FrameworkReviewOutcomeNotification : FrameworkReview
    {
        public string FrameworkName { get; set; }
        public string? OwnerEmail { get; set; }
        public string? OwnerFirstName { get; set; }
        public string? ReviewerFirstName { get; set; }
        public string? ReviewerLastName { get; set; }
        public bool? ReviewerActive { get; set; }
    }
}
