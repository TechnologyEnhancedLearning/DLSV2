namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.PostLearningAssessment;

    public static class PostLearningAssessmentHelper
    {
        public static PostLearningAssessment CreateDefaultPostLearningAssessment(
            string applicationName = "application name",
            string customisationName = "customisation name",
            string sectionName = "section name",
            string plAssessPath = "https://www.dls.nhs.uk/tracking/MOST/Excel07Core/Assess/L2_Excel_2007_Post_12.dcr",
            int maxScorePl = 50,
            int attemptsPl = 2,
            int plPasses = 1,
            bool plLocked = false
        )
        {
            return new PostLearningAssessment(
                applicationName,
                customisationName,
                sectionName,
                plAssessPath,
                maxScorePl,
                attemptsPl,
                plPasses,
                plLocked
            );
        }
    }
}
