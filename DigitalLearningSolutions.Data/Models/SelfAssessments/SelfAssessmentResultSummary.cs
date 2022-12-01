namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public class SelfAssessmentResultSummary : DelegateSelfAssessment
    {
        public int CandidateAssessmentSupervisorVerificationId { get; set; }
        public int CompetencyAssessmentQuestionCount { get; set; }
        public int ResultCount { get; set; }
        public int VerifiedCount { get; set; }
        public int UngradedCount { get; set; }
        public int MeetingCount { get; set; }
        public int PartiallyMeetingCount { get; set; }
        public int NotMeetingCount { get; set; }
        public string? SignOffSupervisorStatement { get; set; }
    }
}
