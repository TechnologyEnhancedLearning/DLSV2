namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class EditLearningPathwayDefaultsViewModel
    {
        public EditLearningPathwayDefaultsViewModel() { }

        public EditLearningPathwayDefaultsViewModel(CourseDetails courseDetails)
        {
            CustomisationId = courseDetails.CustomisationId;
            CompleteWithinMonths = courseDetails.CompleteWithinMonths;
            ValidityMonths = courseDetails.ValidityMonths;
            Mandatory = courseDetails.Mandatory;
            AutoRefresh = courseDetails.AutoRefresh;
        }

        public int CustomisationId { get; set; }
        public int CompleteWithinMonths { get; set; }
        public int ValidityMonths { get; set; }
        public bool Mandatory { get; set; }
        public bool AutoRefresh { get; set; }
    }
}
