namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Attributes;

    public class EditCourseDetailsViewModel
    {
        public EditCourseDetailsViewModel(CourseDetails courseDetails)
        {
            CustomisationId = courseDetails.CustomisationId;
            CustomisationName = courseDetails.CustomisationName;
            Password = courseDetails.Password;
            NotificationEmails = courseDetails.NotificationEmails;
            PostLearningAssessment = courseDetails.PostLearningAssessment;
            IsAssessed = courseDetails.IsAssessed;
            DiagAssess = courseDetails.DiagAssess;
            TutCompletionThreshold = courseDetails.TutCompletionThreshold;
            DiagCompletionThreshold = courseDetails.DiagCompletionThreshold;
        }

        public int CustomisationId { get; set; }

        public string CustomisationName { get; set; }

        public string? Password { get; set; }

        public string? NotificationEmails { get; set; }

        public bool PostLearningAssessment { get; set; }

        public bool IsAssessed { get; set; }

        public bool DiagAssess { get; set; }

        [WholeNumberWithinInclusiveRange(0, 48, "Enter a whole number from 0 to 100")]
        public int TutCompletionThreshold { get; set; }

        [WholeNumberWithinInclusiveRange(0, 48, "Enter a whole number from 0 to 100")]
        public int DiagCompletionThreshold { get; set; }
    }
}
