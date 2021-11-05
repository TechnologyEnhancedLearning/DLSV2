namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditCourseDetailsViewModel
    {
        public EditCourseDetailsViewModel() { }

        public EditCourseDetailsViewModel(CourseDetails courseDetails)
        {
            CustomisationId = courseDetails.CustomisationId;
            CentreId = courseDetails.CentreId;
            ApplicationId = courseDetails.ApplicationId;
            CustomisationName = GetPartOfCustomisationName(courseDetails.CustomisationName, 0)!;
            CustomisationNameSuffix = GetPartOfCustomisationName(courseDetails.CustomisationName, 1);
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

        public EditCourseDetailsViewModel(
            int customisationId,
            int centreId,
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
            CustomisationId = customisationId;
            CentreId = centreId;
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

        public int CustomisationId { get; set; }

        public int CentreId { get; set; }

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

        private static string? GetPartOfCustomisationName(string customisationName, int index)
        {
            if (customisationName.Contains(" - "))
            {
                return customisationName.Split(" - ")[index];
            }

            return index == 0 ? customisationName : null;
        }
    }
}
