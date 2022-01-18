namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    using System;

    public class SelfAssessmentResult
    {
        public int Id { get; set; }

        public int CandidateId { get; set; }

        public int SelfAssessmentId { get; set; }

        public int CompetencyId { get; set; }

        public int AssessmentQuestionId { get; set; }

        public int Result { get; set; }

        public DateTime DateTime { get; set; }

        public string? SupportingComments { get; set; }
    }
}
