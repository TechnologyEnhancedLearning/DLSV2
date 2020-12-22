namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.CourseCompletion;

    internal class CourseCompletionHelper
    {
        public static CourseCompletion CreateDefaultCourseCompletion(
            int customisationId = 1,
            string applicationName = "Application",
            string customisationName = "Customisation",
            bool includeCertification = true,
            DateTime? completed = null,
            DateTime? evaluated = null,
            int maxPostLearningAssessmentAttempts = 0,
            bool isAssessed = true,
            int postLearningAssessmentPassThreshold = 100,
            int diagnosticAssessmentCompletionThreshold = 95,
            int tutorialsCompletionThreshold = 90,
            int? diagnosticScore = 85,
            int diagnosticAttempts = 3,
            int percentageTutorialsCompleted = 75,
            int postLearningPasses = 5,
            int sectionCount = 10
        )
        {
            return new CourseCompletion(
                customisationId,
                applicationName,
                customisationName,
                includeCertification,
                completed,
                evaluated,
                maxPostLearningAssessmentAttempts,
                isAssessed,
                postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold,
                diagnosticScore,
                diagnosticAttempts,
                percentageTutorialsCompleted,
                postLearningPasses,
                sectionCount
            );
        }
    }
}
