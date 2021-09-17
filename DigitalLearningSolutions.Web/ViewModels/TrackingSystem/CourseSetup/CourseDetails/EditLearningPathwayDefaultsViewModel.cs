namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditLearningPathwayDefaultsViewModel
    {
        public EditLearningPathwayDefaultsViewModel(CourseDetails courseDetails)
        {
            CustomisationId = courseDetails.CustomisationId;
            CompleteWithinMonths = courseDetails.CompleteWithinMonths;
            CompletionValidFor = courseDetails.ValidityMonths;
            Mandatory = courseDetails.Mandatory;
            AutoRefresh = courseDetails.AutoRefresh;
        }

        public int CustomisationId { get; set; }
        public int CompleteWithinMonths { get; set; }
        public int CompletionValidFor { get; set; }
        public bool Mandatory { get; set; }
        public bool AutoRefresh { get; set; }
    }
}
