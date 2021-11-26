namespace DigitalLearningSolutions.Data.Models.SelfAssessments.New
{
    public class AssessmentQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string? MaxValueDescription { get; set; }
        public string? MinValueDescription { get; set; }
        public int AssessmentQuestionInputTypeID { get; set; }
        public bool IncludeComments { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string? ScoringInstructions { get; set; }
        public int AddedByAdminId { get; set; }
        public string? CommentsPrompt { get; set; }
        public string? CommentsHint { get; set; }

        public SelfAssessmentResult LatestSelfAssessmentResult { get; set; }
    }
}
