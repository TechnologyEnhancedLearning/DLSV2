namespace DigitalLearningSolutions.Data.Models.SessionData.SelfAssessments
{
    using System;
    public class SessionAddSupervisor
    {
        public SessionAddSupervisor()
        {
            Id = new Guid();
        }
        public Guid Id { get; set; }
        public int SelfAssessmentID { get; set; }
        public string SelfAssessmentName { get; set; }
        public int SupervisorAdminId { get; set; }
        public string? SupervisorEmail { get; set; }
        public int? SelfAssessmentSupervisorRoleId { get; set; }
    }
}
