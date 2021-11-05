namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class EditCourseDetailsViewModel : EditCourseDetailsFormData
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
            EditCourseDetailsFormData formData,
            int customisationId,
            int centreId
        ) : base(formData)
        {
            CustomisationId = customisationId;
            CentreId = centreId;
        }

        public int CustomisationId { get; set; }

        public int CentreId { get; set; }

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
