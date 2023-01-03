namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    using System;

    public class CandidateAssessment
    {
        public int Id { get; set; }

        public int DelegateUserID { get; set; }

        public int SelfAssessmentId { get; set; }

        public DateTime? CompletedDate { get; set; }

        public DateTime? RemovedDate { get; set; }
    }
}
