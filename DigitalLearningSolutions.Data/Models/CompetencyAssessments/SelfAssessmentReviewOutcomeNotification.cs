

namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    public  class SelfAssessmentReviewOutcomeNotification: SelfAssessmentReview
    {
        public string SelfAssessmentName { get; set; } = string.Empty;
        public string? OwnerEmail { get; set; }
        public string? OwnerFirstName { get; set; }
        public string? ReviewerFirstName { get; set; }
        public string? ReviewerLastName { get; set; }
        public bool? ReviewerActive { get; set; }
    }
}
