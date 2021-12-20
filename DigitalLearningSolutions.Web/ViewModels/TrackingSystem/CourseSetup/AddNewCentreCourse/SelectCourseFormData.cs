namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.ComponentModel.DataAnnotations;

    public class SelectCourseFormData
    {
        public SelectCourseFormData() { }

        protected SelectCourseFormData(SelectCourseFormData formData)
        {
            CustomisationId = formData.CustomisationId;
        }

        [Required(ErrorMessage = "Select a course")]
        public int? CustomisationId { get; set; }
    }
}
