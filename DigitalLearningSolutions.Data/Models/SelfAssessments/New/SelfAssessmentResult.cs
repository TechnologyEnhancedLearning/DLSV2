namespace DigitalLearningSolutions.Data.Models.SelfAssessments.New
{
    using System;

    public class SelfAssessmentResult
    {
        public int Id { get; set; }
        public int DelegateId { get; set; }
        public int SelfAssessmentId { get; set; }
        public int CompetencyId { get; set; }
        public int AssessmentQuestionId { get; set; }
        public int Result { get; set; }
        public DateTime DateTime { get; set; }
        public string? SupportingComments { get; set; }
    }
}
