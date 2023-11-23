namespace DigitalLearningSolutions.Data.Models.SessionData.SelfAssessments
{
    public class SessionAddSupervisor
    {
        public int SelfAssessmentID { get; set; }
        public string SelfAssessmentName { get; set; }
        public int SupervisorAdminId { get; set; }
        public string? SupervisorEmail { get; set; }
        public int? SelfAssessmentSupervisorRoleId { get; set; }
        public int? CentreID { get; set; }
    }
}
