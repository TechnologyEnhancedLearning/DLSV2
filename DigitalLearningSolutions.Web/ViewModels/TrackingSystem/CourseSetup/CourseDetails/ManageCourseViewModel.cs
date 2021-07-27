namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class ManageCourseViewModel
    {
        public ManageCourseViewModel(CourseDetails courseDetails)
        {
            CustomisationId = courseDetails.CustomisationId;
            Active = courseDetails.Active;
            CourseName = courseDetails.CourseName;
            SummaryViewModel = new CourseSummaryViewModel(courseDetails);
            DetailsViewModel = new CourseDetailsViewModel(courseDetails);
            OptionsViewModel = new CourseOptionsViewModel(courseDetails);
            LearningPortalDefaultsViewModel = new LearningPathwayDefaultsViewModel(courseDetails);
        }

        public int CustomisationId { get; set; }
        public bool Active { get; set; }
        public string CourseName { get; set; }
        public CourseSummaryViewModel SummaryViewModel { get; set; }
        public CourseDetailsViewModel DetailsViewModel { get; set; }
        public CourseOptionsViewModel OptionsViewModel { get; set; }
        public LearningPathwayDefaultsViewModel LearningPortalDefaultsViewModel { get; set; }
    }
}
