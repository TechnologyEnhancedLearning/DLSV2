namespace DigitalLearningSolutions.Data.Models.SessionData.Tracking.Delegate.Enrol
{
    using System;
    public class SessionEnrolDelegate
    {
        public SessionEnrolDelegate()
        {
            Id = new Guid();
        }
        public Guid Id { get; set; }
        public int? AssessmentID { get; set; }
        public string? AssessmentName { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public int? SelfAssessmentSupervisorRoleId { get; set; }
        public int? SupervisorID { get; set; }
        public string? SupervisorName { get; set; }
        public bool IsSelfAssessment { get; set; }
        public int AssessmentVersion { get; set; }
    }
}
