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
            int averageDuration = 45,
            string centreName = "Centre",
            string? bannerText = "Banner",
            bool includeCertification = false,
            DateTime? completed = null,
            int maxPostLearningAssessmentAttempts = 0,
            bool isAssessed = true,
            int postLearningAssessmentPassThreshold = 100,
            int diagnosticAssessmentCompletionThreshold = 85,
            int tutorialsCompletionThreshold = 0
        )
        {
            return new CourseContent(
                customisationId,
                applicationName,
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
                tutorialsCompletionThreshold
            );
        }
    }
}
