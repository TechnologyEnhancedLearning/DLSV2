namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    using System;
    public class CurrentSelfAssessment : SelfAssessment
    {
        public string? UserBookmark { get; set; }
        public bool UnprocessedUpdates { get; set; }
        public int LaunchCount { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public bool IsSupervised { get; set; }
        public bool IsSupervisorResultsReviewed { get; set; }
        public bool IncludeRequirementsFilters { get; set; }
        public bool SupervisorSelfAssessmentReview { get; set; }
        public string? Vocabulary { get; set; }
        public string? VerificationRoleName { get; set; }
        public string? SignOffRoleName { get; set; }
        public string? SignOffRequestorStatement { get; set; }
        public bool EnforceRoleRequirementsForSignOff { get; set; }
        public string? ManageSupervisorsDescription { get; set; }
        public string? ReviewerCommentsLabel { get; set; }
        public bool NonReportable { get; set; }
        public int? SupervisorCount { get; set; }
        public bool IsSameCentre { get; set; }
        public int? DelegateUserId { get; set; }
        public string? DelegateName { get; set; }
        public string? EnrolledByFullName { get; set; }
        public bool SelfAssessmentProcessAgreed { get; set; }
    }
}
