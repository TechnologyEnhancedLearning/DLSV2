namespace DigitalLearningSolutions.Data.Models.PostLearningAssessment
{
    public class OldPostLearningAssessmentScores
    {
        public int SectionID { get; set; }
        public int MaxScorePL { get; set; }
        public int AttemptsPL { get; set; }
        public bool PLPassed { get; set; }
        public bool PLLocked { get; set; }
    }
}
