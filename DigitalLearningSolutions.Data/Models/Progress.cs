namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class Progress
    {
        public int ProgressId { get; set; }
        public int CandidateId { get; set; }
        public int CustomisationId { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? RemovedDate { get; set; }
        public int SupervisorAdminId { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public DateTime SubmittedTime { get; set; }
        public int CustomisationVersion { get; set; }
        public int EnrollmentMethodId { get; set; }
        public int EnrolledByAdminId { get; set; }
        public bool SystemRefreshed { get; set; }
        public int? DiagnosticScore { get; set; }
    }
}
