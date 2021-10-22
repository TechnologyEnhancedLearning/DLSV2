namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Attributes;

    public class EditCourseDetailsViewModel
    {
        public EditCourseDetailsViewModel() { }

        public EditCourseDetailsViewModel(CourseDetails courseDetails)
        {
            CentreId = courseDetails.CentreId;
            ApplicationId = courseDetails.ApplicationId;
            CustomisationName = courseDetails.CustomisationName.Contains(" - ")
                ? courseDetails.CustomisationName.Split(" - ")[0]
                : courseDetails.CustomisationName;
            CustomisationNameSuffix = courseDetails.CustomisationName.Contains(" - ")
                ? courseDetails.CustomisationName.Split(" - ")[1]
                : null;
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

        public int CentreId { get; }

        public int ApplicationId { get; }

        public string CustomisationName { get; set; }

        // TODO: If course name with suffix matches one in the db, add model error
        /*[UniqueCustomisationName(CentreId, ApplicationId, "The course name must be unique")]*/
        public string? CustomisationNameSuffix { get; set; }

        public bool PasswordProtected { get; set; }

        [Required(ErrorMessage = "Enter a password for the course")]
        public string? Password { get; set; }

        public bool ReceiveNotificationEmails { get; set; }

        [Required(ErrorMessage = "Enter an email for receiving the notifications")]
        public string? NotificationEmails { get; set; }

        public bool PostLearningAssessment { get; set; }

        public bool IsAssessed { get; set; }

        public bool DiagAssess { get; set; }

        [WholeNumberWithinInclusiveRange(0, 48, "Enter a whole number from 0 to 100")]
        public string? TutCompletionThreshold { get; set; }

        [WholeNumberWithinInclusiveRange(0, 48, "Enter a whole number from 0 to 100")]
        public string? DiagCompletionThreshold { get; set; }
    }
}
