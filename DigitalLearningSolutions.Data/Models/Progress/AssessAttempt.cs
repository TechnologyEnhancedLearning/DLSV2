namespace DigitalLearningSolutions.Data.Models.Progress
{
    using System;

    public class AssessAttempt
    {
        public int AssessAttemptId { get; set; }
        public int CandidateId { get; set; }
        public int CustomisationId { get; set; }
        public int CustomisationVersion { get; set; }
        public DateTime Date { get; set; }
        public int AssessInstance { get; set; }
        public int SectionNumber { get; set; }
        public int Score { get; set; }
        public bool Status { get; set; }
        public int ProgressId { get; set; }
    }
}
