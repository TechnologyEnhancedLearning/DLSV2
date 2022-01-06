namespace DigitalLearningSolutions.Data.Models.SelfAssessments.Export
{
    using System;
    public class CandidateAssessmentExportSummary
    {
        public string? SelfAssessment { get; set; }
        public string? CandidateName { get; set; }
        public string? CandidatePrn { get; set; }
        public DateTime? StartDate { get; set; }
        public int QuestionCount { get; set; }
        public int SelfAssessmentResponseCount { get; set; }
        public int ResponsesVerifiedCount { get; set; }
        public int NoRequirementsSetCount { get; set; }
        public int NotMeetingCount { get; set; }
        public int PartiallyMeetingCount { get; set; }
        public int MeetingCount { get; set; }
        public DateTime? SignedOff { get; set; }
        public string? Signatory { get; set; }
        public string? SignatoryPrn { get; set; }
    }
}
