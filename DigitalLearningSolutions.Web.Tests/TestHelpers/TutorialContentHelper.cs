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
            string? supportingMaterialPath = "material",
            string? postLearningAssessmentPath = "/postLearningAssessment",
            int? subsequentTutorialId = 2,
            int? subsequentSectionId = 45
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
                supportingMaterialPath,
                postLearningAssessmentPath,
                subsequentTutorialId,
                subsequentSectionId
            );
        }

        public static TutorialContent CreateDefaultTutorialContent(
            string tutorialName = "tutorial",
            string applicationName = "application",
            string customisationName = "customisation",
            string tutorialPath = "tutorialPath",
            int currentVersion = 1
        )
        {
            return new TutorialContent(
                tutorialName,
                applicationName,
                customisationName,
                tutorialPath,
                currentVersion
            );
        }

        public static TutorialVideo CreateDefaultTutorialVideo(
            string tutorialName = "tutorial",
            string applicationName = "application",
            string customisationName = "customisation",
            string videoPath = "/video.mp4"
        )
        {
            return new TutorialVideo(
                tutorialName,
                applicationName,
                customisationName,
                videoPath
            );
        }
    }
}
