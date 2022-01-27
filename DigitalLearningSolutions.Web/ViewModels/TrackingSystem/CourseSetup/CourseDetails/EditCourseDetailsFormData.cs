namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditCourseDetailsFormData
    {
        public EditCourseDetailsFormData() { }

        protected EditCourseDetailsFormData(EditCourseDetailsFormData formData)
        {
            ApplicationId = formData.ApplicationId;
            ApplicationName = formData.ApplicationName;
            CustomisationName = formData.CustomisationName;
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

        protected EditCourseDetailsFormData(CourseDetails courseDetails)
        {
            ApplicationId = courseDetails.ApplicationId;
            ApplicationName = courseDetails.ApplicationName;
            CustomisationName = courseDetails.CustomisationName;
            PasswordProtected = !string.IsNullOrEmpty(courseDetails.Password);
            Password = courseDetails.Password;
            ReceiveNotificationEmails = !string.IsNullOrEmpty(courseDetails.NotificationEmails);
            NotificationEmails = courseDetails.NotificationEmails;
            PostLearningAssessment = courseDetails.PostLearningAssessment;
            IsAssessed = courseDetails.IsAssessed;
            DiagAssess = courseDetails.DiagAssess;
            TutCompletionThreshold = courseDetails.TutCompletionThreshold.ToString();
            DiagCompletionThreshold = courseDetails.DiagCompletionThreshold.ToString();
        }

        public int ApplicationId { get; set; }

        public string ApplicationName { get; set; }

        public string? CustomisationName { get; set; }

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

        public void ConvertCustomisationNameToEmptyStringIfNull()
        {
            if (string.IsNullOrWhiteSpace(CustomisationName))
            {
                CustomisationName = string.Empty;
            }
        }
    }
}
