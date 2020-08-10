namespace DigitalLearningSolutions.Data.Models
{
    using System.Collections.Generic;

    public class Competency
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string CompetencyGroup { get; set; }
        public List<AssessmentQuestion> AssessmentQuestions { get; } = new List<AssessmentQuestion>();
    }
}
