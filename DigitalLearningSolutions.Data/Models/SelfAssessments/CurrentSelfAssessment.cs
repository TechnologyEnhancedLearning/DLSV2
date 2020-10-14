namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class CurrentSelfAssessment : SelfAssessment
    {
        public string? UserBookmark { get; set; }
        public bool UnprocessedUpdates { get; set; }
        public int LaunchCount { get; set; }
    }
}
