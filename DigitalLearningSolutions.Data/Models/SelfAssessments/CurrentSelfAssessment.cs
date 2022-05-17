namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    using System;
    public class CurrentSelfAssessment : SelfAssessment
    {
        public int CandidateAssessmentId { get; set; }
        public string? UserBookmark { get; set; }
        public bool UnprocessedUpdates { get; set; }
        public int LaunchCount { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public bool IsSupervised { get; set; }
        public bool IsSupervisorResultsReviewed { get; set; }
        public bool SupervisorSelfAssessmentReview { get; set; }
        public string? Vocabulary { get; set; }
        public string? VerificationRoleName { get; set; }
        public string? SignOffRoleName { get; set; }
        public string? SignOffRequestorStatement { get; set; }
        public bool EnforceRoleRequirementsForSignOff { get; set; }
        public string? ManageSupervisorsDescription { get; set; }
    }
}
