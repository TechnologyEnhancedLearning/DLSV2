namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;

    public class Competency
    {
        public int Id { get; set; }
        public int RowNo { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? QuestionLabel { get; set; }
        public string CompetencyGroup { get; set; }
        public int CompetencyGroupID { get; set; }
        public string? Vocabulary { get; set; }
        public bool Optional { get; set; }
        public bool AlwaysShowDescription { get; set; }
        public bool IncludedInSelfAssessment { get; set; }
        public DateTime? Verified { get; set; }
        public DateTime? Requested { get; set; }
        public bool? SignedOff { get; set; }
        public SelfAssessmentResponseStatus? ResponseStatus { get; set; }
        public List<AssessmentQuestion> AssessmentQuestions { get; set; } = new List<AssessmentQuestion>();
    }
}
