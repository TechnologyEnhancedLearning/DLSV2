namespace DigitalLearningSolutions.Data.Models.PostLearningAssessment
{
    public class PostLearningAssessment
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public int PostLearningScore { get; }
        public int PostLearningAttempts { get; }
        public bool PostLearningPassed { get; }
        public bool PostLearningLocked { get; }
        public int? NextSectionId { get; }

        public PostLearningAssessment(
            string applicationName,
            string customisationName,
            string sectionName,
            int bestScore,
            int attemptsPl,
            int plPasses,
            bool plLocked,
            int? nextSectionId
        )
        {
            CourseTitle = $"{applicationName} - {customisationName}";
            SectionName = sectionName;
            PostLearningScore = bestScore;
            PostLearningAttempts = attemptsPl;
            PostLearningPassed = plPasses > 0;
            PostLearningLocked = plLocked;
            NextSectionId = nextSectionId;
        }
    }
}
