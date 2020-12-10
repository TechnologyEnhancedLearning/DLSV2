namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.TutorialContent;

    internal class TutorialContentHelper
    {
        public static TutorialInformation CreateDefaultTutorialInformation(
            int id = 1,
            string name = "Tutorial",
            string customisationName = "Customisation",
            string applicationName = "Application",
            string status = "Complete",
            int timeSpent = 30,
            int averageTutorialDuration = 60,
            int currentScore = 9,
            int possibleScore = 24,
            bool canShowDiagnosticStatus = true,
            int attemptCount = 3,
            string? objectives = "objectives",
            string? videoPath = "video",
            string? tutorialPath = "tutorial",
            string? supportingMaterialPath = "material"
        )
        {
            return new TutorialInformation(
                id,
                name,
                applicationName,
                customisationName,
                status,
                timeSpent,
                averageTutorialDuration,
                currentScore,
                possibleScore,
                canShowDiagnosticStatus,
                attemptCount,
                objectives,
                videoPath,
                tutorialPath,
                supportingMaterialPath
            );
        }
    }
}
