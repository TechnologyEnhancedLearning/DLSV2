namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.CourseContent;

    internal class CourseContentHelper
    {
        public static CourseContent CreateDefaultCourseContent(
            int customisationId = 1,
            string customisationName = "Customisation",
            string applicationName = "Application",
            string? applicationInfo = null,
            int? averageDuration = 45,
            string centreName = "Centre",
            string? bannerText = "Banner",
            bool includeCertification = false,
            DateTime? completed = null,
            int maxPostLearningAssessmentAttempts = 0,
            bool isAssessed = true,
            int postLearningAssessmentPassThreshold = 100,
            int diagnosticAssessmentCompletionThreshold = 85,
            int tutorialsCompletionThreshold = 0,
            string? courseSettings = null
        )
        {
            return new CourseContent(
                customisationId,
                applicationName,
                applicationInfo,
                customisationName,
                averageDuration,
                centreName,
                bannerText,
                includeCertification,
                completed,
                maxPostLearningAssessmentAttempts,
                isAssessed,
                postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold,
                courseSettings
            );
        }

        public static CourseSection CreateDefaultCourseSection(
            string sectionName = "Section",
            int id = 1,
            bool hasLearning = true,
            double percentComplete = 25,
            int postLearningAssessmentsPassed = 0
        )
        {
            return new CourseSection(
                sectionName,
                id,
                hasLearning,
                percentComplete,
                postLearningAssessmentsPassed
            );
        }
    }
}
