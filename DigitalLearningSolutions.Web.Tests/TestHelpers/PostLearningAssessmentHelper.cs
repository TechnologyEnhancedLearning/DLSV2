namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
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
            bool plLocked = false,
            bool includeCertification = true,
            bool isAssessed = true,
            DateTime? completed = null,
            int maxPostLearningAssessmentAttempts = 0,
            int postLearningAssessmentPassThreshold = 100,
            int diagnosticAssessmentCompletionThreshold = 85,
            int tutorialsCompletionThreshold = 0,
            int? nextSectionId = 101,
            bool otherSectionsExist = true,
            bool otherItemsInSectionExist = true
        )
        {
            return new PostLearningAssessment(
                applicationName,
                customisationName,
                sectionName,
                bestScore,
                attemptsPl,
                plPasses,
                plLocked,
                includeCertification,
                isAssessed,
                completed,
                maxPostLearningAssessmentAttempts,
                postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold,
                nextSectionId,
                otherSectionsExist,
                otherItemsInSectionExist
            );
        }
    }
}
