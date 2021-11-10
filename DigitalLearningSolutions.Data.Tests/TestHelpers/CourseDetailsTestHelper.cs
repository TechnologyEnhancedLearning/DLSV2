namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.Courses;

    public static class CourseDetailsTestHelper
    {
        public static CourseDetails GetDefaultCourseDetails(
            int customisationId = 100,
            int centreId = 101,
            int applicationId = 1,
            string applicationName = "Entry Level - Win XP, Office 2003/07 OLD",
            string customisationName = "Standard",
            int currentVersion = 12,
            DateTime? createdDate = null,
            DateTime? lastAccessed = null,
            string? password = null,
            string? notificationEmails = null,
            bool postLearningAssessment = false,
            bool isAssessed = true,
            int tutCompletionThreshold = 100,
            bool diagAssess = false,
            int diagCompletionThreshold = 85,
            bool selfRegister = true,
            bool diagObjSelect = true,
            bool hideInLearnerPortal = false,
            bool active = false,
            int delegateCount = 25,
            int completedCount = 5,
            int completeWithinMonths = 0,
            int validityMonths = 0,
            bool mandatory = false,
            bool autoRefresh = false,
            int refreshToCustomisationId = 0,
            string? refreshToApplicationName = null,
            string? refreshToCustomisationName = null,
            int autoRefreshMonths = 0,
            bool applyLpDefaultsToSelfEnrol = false,
            int courseCategoryId = 2
        )
        {
            return new CourseDetails
            {
                CustomisationId = customisationId,
                CentreId = centreId,
                ApplicationId = applicationId,
                ApplicationName = applicationName,
                CustomisationName = customisationName,
                CurrentVersion = currentVersion,
                CreatedDate = createdDate ?? DateTime.UtcNow,
                LastAccessed = lastAccessed ?? DateTime.UtcNow,
                Password = password,
                NotificationEmails = notificationEmails,
                PostLearningAssessment = postLearningAssessment,
                IsAssessed = isAssessed,
                TutCompletionThreshold = tutCompletionThreshold,
                DiagAssess = diagAssess,
                DiagCompletionThreshold = diagCompletionThreshold,
                SelfRegister = selfRegister,
                DiagObjSelect = diagObjSelect,
                HideInLearnerPortal = hideInLearnerPortal,
                Active = active,
                DelegateCount = delegateCount,
                CompletedCount = completedCount,
                CompleteWithinMonths = completeWithinMonths,
                ValidityMonths = validityMonths,
                Mandatory = mandatory,
                AutoRefresh = autoRefresh,
                RefreshToCustomisationId = refreshToCustomisationId,
                RefreshToApplicationName = refreshToApplicationName,
                RefreshToCustomisationName = refreshToCustomisationName,
                AutoRefreshMonths = autoRefreshMonths,
                ApplyLpDefaultsToSelfEnrol = applyLpDefaultsToSelfEnrol,
                CourseCategoryId = courseCategoryId
            };
        }
    }
}
