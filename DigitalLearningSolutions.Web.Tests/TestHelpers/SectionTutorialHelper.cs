namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public class SectionTutorialHelper
    {
        public static SectionTutorial CreateDefaultSectionTutorial(
            string tutorialName = "Name",
            int tutStat = 0,
            string completionStatus = "Not started",
            int tutTime = 10,
            int averageTutMins = 20,
            int tutorialId = 1,
            bool customisationTutorialStatus = true,
            int currentScore = 3,
            int possibleScore = 5,
            bool tutorialDiagnosticStatus = true,
            int tutorialDiagnosticAttempts = 1
        )
        {
            return new SectionTutorial(
                tutorialName,
                tutStat,
                completionStatus,
                tutTime,
                averageTutMins,
                tutorialId,
                customisationTutorialStatus,
                currentScore,
                possibleScore,
                tutorialDiagnosticStatus,
                tutorialDiagnosticAttempts
            );
        }
    }
}
