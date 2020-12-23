namespace DigitalLearningSolutions.Data.Models.PostLearningAssessment
{
    public class PostLearningAssessment
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public string PostLearningAssessmentPath { get; }
        public int PostLearningScore { get; }
        public int PostLearningAttempts { get; }
        public bool PostLearningPassed { get; }

        public PostLearningAssessment(
            string applicationName,
            string customisationName,
            string sectionName,
            string plAssessPath,
            int maxScorePl,
            int attemptsPl,
            int plPasses
        )
        {
            CourseTitle = $"{applicationName} - {customisationName}";
            SectionName = sectionName;
            PostLearningAssessmentPath = plAssessPath;
            PostLearningScore = maxScorePl;
            PostLearningAttempts = attemptsPl;
            PostLearningPassed = plPasses > 0;
        }
    }
}
