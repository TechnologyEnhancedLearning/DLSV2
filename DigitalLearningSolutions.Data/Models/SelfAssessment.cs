namespace DigitalLearningSolutions.Data.Models
{
    public class SelfAssessment : CurrentLearningItem
    {
        public string Description { get; set; }
        public int NumberOfCompetencies { get; set; }
        public bool UseFilteredApi { get; set; }
        public string? UserBookmark { get; set; }
        public bool UnprocessedUpdates { get; set; }
    }
}
