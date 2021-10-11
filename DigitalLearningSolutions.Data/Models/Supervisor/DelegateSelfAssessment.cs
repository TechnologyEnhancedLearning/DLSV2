﻿namespace DigitalLearningSolutions.Data.Models.Supervisor
{
    using System;
    public class DelegateSelfAssessment
    {
        public int ID { get; set; }
        public int SelfAssessmentID { get; set; }
        public string? RoleName { get; set; }
        public bool SupervisorSelfAssessmentReview { get; set; }
        public bool SupervisorResultsReview { get; set; }
        public string? SupervisorRoleTitle { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime LastAccessed { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public int LaunchCount { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? ProfessionalGroup { get; set; }
        public string? SubGroup { get; set; }
        public string? RoleProfile { get; set; }
        public int VerificationRequested { get; set; }
        public int ResultsVerificationRequests { get; set; }
    }
}
