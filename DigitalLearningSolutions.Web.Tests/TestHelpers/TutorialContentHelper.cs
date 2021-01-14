﻿namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.TutorialContent;

    internal class TutorialContentHelper
    {
        public static TutorialInformation CreateDefaultTutorialInformation(
            int id = 1,
            string name = "Tutorial",
            string sectionName = "Section",
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
            string? courseSettings = null,
            int? nextTutorialId = 2,
            int? nextSectionId = 45
        )
        {
            return new TutorialInformation(
                id,
                name,
                sectionName,
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
                courseSettings,
                nextTutorialId,
                nextSectionId
            );
        }

        public static TutorialContent CreateDefaultTutorialContent(
            string tutorialName = "tutorial",
            string sectionName = "section",
            string applicationName = "application",
            string customisationName = "customisation",
            string tutorialPath = "tutorialPath",
            int currentVersion = 1
        )
        {
            return new TutorialContent(
                tutorialName,
                sectionName,
                applicationName,
                customisationName,
                tutorialPath,
                currentVersion
            );
        }

        public static TutorialVideo CreateDefaultTutorialVideo(
            string tutorialName = "tutorial",
            string sectionName = "section",
            string applicationName = "application",
            string customisationName = "customisation",
            string videoPath = "/video.mp4"
        )
        {
            return new TutorialVideo(
                tutorialName,
                sectionName,
                applicationName,
                customisationName,
                videoPath
            );
        }
    }
}
