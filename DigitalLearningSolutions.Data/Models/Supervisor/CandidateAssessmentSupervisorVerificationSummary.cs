namespace DigitalLearningSolutions.Data.Models.Supervisor
{
    public class CandidateAssessmentSupervisorVerificationSummary
    {
        public int ID { get; set; }
        public string? Forename { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public bool AdminActive { get; set; }
        public int VerifiedCount { get; set; }
    }
}
