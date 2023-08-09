namespace DigitalLearningSolutions.Data.Models.Supervisor
{
    using System;
    public class DelegateSelfAssessment
    {
        public int ID { get; set; }
        public int SelfAssessmentID { get; set; }
        public int DelegateUserID { get; set; }
        public string? RoleName { get; set; }
        public bool SupervisorSelfAssessmentReview { get; set; }
        public bool SupervisorResultsReview { get; set; }
        public string? SupervisorRoleTitle { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime LastAccessed { get; set; }
        public DateTime? SignedOffDate { get; set; }
        public bool SignedOff { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public int LaunchCount { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? ProfessionalGroup { get; set; }
        public string? QuestionLabel { get; set; }
        public string? DescriptionLabel { get; set; }
        public string? ReviewerCommentsLabel { get; set; }
        public string? SubGroup { get; set; }
        public string? RoleProfile { get; set; }
        public int SignOffRequested { get; set; }
        public int ResultsVerificationRequests { get; set; }
        public bool IsSupervisorResultsReviewed { get; set; }
        public bool IsAssignedToSupervisor { get; set; }
        public bool NonReportable { get; set; }
    }
}
