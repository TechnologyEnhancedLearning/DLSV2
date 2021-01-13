namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.PostLearningAssessment;

    public static class PostLearningAssessmentHelper
    {
        public static PostLearningAssessment CreateDefaultPostLearningAssessment(
            string applicationName = "application name",
            string customisationName = "customisation name",
            string sectionName = "section name",
            int bestScore = 50,
            int attemptsPl = 2,
            int plPasses = 1,
            bool plLocked = false
        )
        {
            return new PostLearningAssessment(
                applicationName,
                customisationName,
                sectionName,
                bestScore,
                attemptsPl,
                plPasses,
                plLocked
            );
        }
    }
}
