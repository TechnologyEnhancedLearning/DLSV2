namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class CourseDetailsViewModel
    {
        public CourseDetailsViewModel(CourseDetails courseDetails)
        {
            CustomisationId = courseDetails.CustomisationId;
            Password = courseDetails.Password;
            NotificationEmails = courseDetails.NotificationEmails;
            PostLearningAssessment = courseDetails.PostLearningAssessment;
            IsAssessed = courseDetails.IsAssessed;
            DiagAssess = courseDetails.DiagAssess;
            TutCompletionThreshold = courseDetails.TutCompletionThreshold;
            DiagCompletionThreshold = courseDetails.DiagCompletionThreshold;
        }

        public int CustomisationId { get; }
        public string? Password { get; set; }
        public string? NotificationEmails { get; set; }
        public bool PostLearningAssessment { get; set; }
        public bool IsAssessed { get; set; }
        public bool DiagAssess { get; set; }
        public int TutCompletionThreshold { get; set; }
        public int DiagCompletionThreshold { get; set; }
    }
}
