namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    using System.Collections.Generic;

    public class Competency
    {
        public int Id { get; set; }
        public int RowNo { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CompetencyGroup { get; set; }
        public string? Vocabulary { get; set; }
        public bool Optional { get; set; }
        public List<AssessmentQuestion> AssessmentQuestions { get; set; } = new List<AssessmentQuestion>();
    }
}
