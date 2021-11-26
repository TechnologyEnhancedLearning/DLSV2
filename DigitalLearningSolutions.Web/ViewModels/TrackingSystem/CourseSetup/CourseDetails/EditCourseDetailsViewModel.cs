namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class EditCourseDetailsViewModel : EditCourseDetailsFormData
    {
        public EditCourseDetailsViewModel() { }

        public EditCourseDetailsViewModel(
            EditCourseDetailsFormData formData,
            int customisationId
        ) : base(formData)
        {
            CustomisationId = customisationId;
        }

        public EditCourseDetailsViewModel(
            CourseDetails courseDetails,
            int customisationId
        ) : base(courseDetails)
        {
            CustomisationId = customisationId;
        }

        public int CustomisationId { get; set; }
    }
}
