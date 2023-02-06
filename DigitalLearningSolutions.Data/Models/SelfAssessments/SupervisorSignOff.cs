namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    using System;
    public class SupervisorSignOff
    {
        public int ID { get; set; }
        public int CandidateAssessmentSupervisorID { get; set; }
        public string? SupervisorName { get; set; }
        public string? SupervisorEmail { get; set; }
        public string? SupervisorRoleName { get; set; }
        public DateTime? Requested { get; set; }
        public DateTime? EmailSent { get; set; }
        public DateTime? Verified { get; set; }
        public string? Comments { get; set; }
        public bool SignedOff { get; set; }
        public DateTime? Removed { get; set; }
    }
}
