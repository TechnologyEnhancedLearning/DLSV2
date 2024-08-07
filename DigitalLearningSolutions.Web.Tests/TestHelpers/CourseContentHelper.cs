namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.CourseContent;
    using DigitalLearningSolutions.Data.Models.Courses;

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
            string? courseSettings = null,
            string? password = null,
            bool passwordSubmitted = false
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
                courseSettings,
                password,
                passwordSubmitted
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

        public static Customisation CreateDefaultCourse()
        {
            Customisation customisation = new Customisation();
            customisation.CentreId = 1;
            customisation.ApplicationId = 1;
            customisation.CustomisationName = "Customisation";
            customisation.Password = null;
            customisation.SelfRegister = true;
            customisation.TutCompletionThreshold = 0;
            customisation.IsAssessed = true;
            customisation.DiagCompletionThreshold = 100;
            customisation.DiagObjSelect = true;
            customisation.HideInLearnerPortal = false;
            customisation.NotificationEmails = null;
            customisation.CustomisationId = 1;
            customisation.Active = true;

            return customisation;
        }
    }
}
