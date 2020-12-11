namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public static class SectionContentHelper
    {
        public static SectionContent CreateDefaultSectionContent(
            string customisationName = "customisation name",
            string applicationName = "application name",
            string sectionName = "section name",
            int timeMins = 1,
            int averageSectionTime = 2,
            bool hasLearning = true,
            double percentComplete = 10,
            int diagAttempts = 5,
            int secScore = 10,
            int secOutOf = 14,
            string diagAssessPath = "Path to Diagnostic Assessment",
            string plAssessPath = "Path to Post Learning Assessment",
            int attemptsPl = 2,
            int plPassed = 1,
            bool diagStatus = true,
            bool isAssessed = true
        )
        {
            return new SectionContent(
                applicationName,
                customisationName,
                sectionName,
                timeMins,
                averageSectionTime,
                hasLearning,
                percentComplete,
                diagAttempts,
                secScore,
                secOutOf,
                diagAssessPath,
                plAssessPath,
                attemptsPl,
                plPassed,
                diagStatus,
                isAssessed
            );
        }
    }
}
