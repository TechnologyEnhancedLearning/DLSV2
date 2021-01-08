namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class AssessmentQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string MaxValueDescription { get; set; }
        public string MinValueDescription { get; set; }
        public int? Result { get; set; }
        public string? ScoringInstructions { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }
}
