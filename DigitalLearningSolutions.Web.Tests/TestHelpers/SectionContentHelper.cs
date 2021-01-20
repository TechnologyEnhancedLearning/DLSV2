namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public static class SectionContentHelper
    {
        public static SectionContent CreateDefaultSectionContent(
            string customisationName = "customisation name",
            string applicationName = "application name",
            string sectionName = "section name",
            bool hasLearning = true,
            int diagAttempts = 5,
            int secScore = 10,
            int secOutOf = 14,
            string? diagAssessPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course308/Diagnostic/02-DIAG-Entering-data/itspplayer.html",
            string? plAssessPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course308/PLAssess/02-PLA-Entering-data/itspplayer.html",
            int attemptsPl = 2,
            int plPasses = 1,
            bool diagStatus = true,
            bool isAssessed = true,
            string? consolidationPath = "https://www.dls.nhs.uk/tracking/MOST/Word07Core/cons/WC07-Exercise_1.zip",
            string? courseSettings = null,
            bool includeCertification = true,
            DateTime? completed = null,
            int maxPostLearningAssessmentAttempts = 0,
            int postLearningAssessmentPassThreshold = 100,
            int diagnosticAssessmentCompletionThreshold = 85,
            int tutorialsCompletionThreshold = 0,
            bool otherSectionsExist = true,
            int? nextSectionId = null
        )
        {
            return new SectionContent(
                applicationName,
                customisationName,
                sectionName,
                hasLearning,
                diagAttempts,
                secScore,
                secOutOf,
                diagAssessPath,
                plAssessPath,
                attemptsPl,
                plPasses,
                diagStatus,
                isAssessed,
                consolidationPath,
                courseSettings,
                includeCertification,
                completed,
                maxPostLearningAssessmentAttempts,
                postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold,
                otherSectionsExist,
                nextSectionId
            );
        }

        public static SectionTutorial CreateDefaultSectionTutorial(
            string tutorialName = "tutorial name",
            int tutStat = 2,
            string completionStatus = "Completed",
            int tutTime = 30,
            int averageTutMins = 45,
            int id = 1,
            bool status = true,
            int currentScore = 5,
            int possibleScore = 7
        )
        {
            return new SectionTutorial(
                tutorialName,
                tutStat,
                completionStatus,
                tutTime,
                averageTutMins,
                id,
                status,
                currentScore,
                possibleScore
            );
        }
    }
}
