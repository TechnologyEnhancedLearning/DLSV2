namespace DigitalLearningSolutions.Data.Models.Supervisor
{
    public class CandidateAssessmentSupervisor
    {
        public int ID { get; set; }
        public int CandidateAssessmentID { get; set; }
        public int SupervisorDelegateId { get; set; }
        public int SelfAssessmentSupervisorRoleID { get; set; }
    }
}
