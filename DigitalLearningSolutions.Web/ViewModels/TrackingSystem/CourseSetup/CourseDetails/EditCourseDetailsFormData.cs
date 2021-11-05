namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditCourseDetailsFormData
    {
        public EditCourseDetailsFormData() { }

        public EditCourseDetailsFormData(
            int applicationId,
            string customisationName,
            string? customisationNameSuffix,
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
            CustomisationName = customisationName;
            CustomisationNameSuffix = customisationNameSuffix;
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

        protected EditCourseDetailsFormData(EditCourseDetailsFormData formData)
        {
            ApplicationId = formData.ApplicationId;
            CustomisationName = formData.CustomisationName;
            CustomisationNameSuffix = formData.CustomisationNameSuffix;
            PasswordProtected = formData.PasswordProtected;
            Password = formData.Password;
            ReceiveNotificationEmails = formData.ReceiveNotificationEmails;
            NotificationEmails = formData.NotificationEmails;
            PostLearningAssessment = formData.PostLearningAssessment;
            IsAssessed = formData.IsAssessed;
            DiagAssess = formData.DiagAssess;
            TutCompletionThreshold = formData.TutCompletionThreshold;
            DiagCompletionThreshold = formData.DiagCompletionThreshold;
        }

        public int ApplicationId { get; set; }

        public string CustomisationName { get; set; }

        public string? CustomisationNameSuffix { get; set; }

        public bool PasswordProtected { get; set; }

        [Required(ErrorMessage = "Enter a password for the course")]
        [MaxLength(50, ErrorMessage = "The password must be 50 characters or fewer")]
        public string? Password { get; set; }

        public bool ReceiveNotificationEmails { get; set; }

        [Required(ErrorMessage = "Enter an email for receiving the notifications")]
        [MaxLength(500, ErrorMessage = "Email address must be 500 characters or fewer")]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(CommonValidationErrorMessages.WhitespaceInEmail)]
        public string? NotificationEmails { get; set; }

        public bool PostLearningAssessment { get; set; }

        public bool IsAssessed { get; set; }

        public bool DiagAssess { get; set; }

        [WholeNumberWithinInclusiveRange(0, 100, "Enter a whole number from 0 to 100")]
        public string? TutCompletionThreshold { get; set; }

        [WholeNumberWithinInclusiveRange(0, 100, "Enter a whole number from 0 to 100")]
        public string? DiagCompletionThreshold { get; set; }
    }
}
