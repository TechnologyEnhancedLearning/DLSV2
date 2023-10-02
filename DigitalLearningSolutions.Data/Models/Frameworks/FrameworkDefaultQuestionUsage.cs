namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class FrameworkDefaultQuestionUsage
    {
        public int ID { get; set; }
        public string Question { get; set; } = string.Empty;
        public int Competencies { get; set; }
        public int CompetencyAssessmentQuestions { get; set; }
    }
}
