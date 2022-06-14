namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse
{
    public class CourseDetailsData
    {
        public CourseDetailsData(
            int applicationId,
            string applicationName,
            string? customisationName,
            bool passwordProtected,
            string? password,
            bool receiveNotificationEmails,
            string? notificationEmails,
            bool postLearningAssessment,
            bool isAssessed,
            bool diagAssess,
            string? tutCompletionThreshold,
            string? diagCompletionThreshold
        )
        {
            ApplicationId = applicationId;
            ApplicationName = applicationName;
            CustomisationName = customisationName;
            PasswordProtected = passwordProtected;
            Password = password;
            ReceiveNotificationEmails = receiveNotificationEmails;
            NotificationEmails = notificationEmails;
            PostLearningAssessment = postLearningAssessment;
            IsAssessed = isAssessed;
            DiagAssess = diagAssess;
            TutCompletionThreshold = tutCompletionThreshold;
            DiagCompletionThreshold = diagCompletionThreshold;
        }

        public int ApplicationId { get; set; }

        public string ApplicationName { get; set; }

        public string? CustomisationName { get; set; }

        public bool PasswordProtected { get; set; }

        public string? Password { get; set; }

        public bool ReceiveNotificationEmails { get; set; }

        public string? NotificationEmails { get; set; }

        public bool PostLearningAssessment { get; set; }

        public bool IsAssessed { get; set; }

        public bool DiagAssess { get; set; }

        public string? TutCompletionThreshold { get; set; }

        public string? DiagCompletionThreshold { get; set; }
    }
}
