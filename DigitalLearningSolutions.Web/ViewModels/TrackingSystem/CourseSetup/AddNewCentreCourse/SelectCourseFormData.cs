namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.ComponentModel.DataAnnotations;

    public class SelectCourseFormData
    {
        public SelectCourseFormData() { }

        protected SelectCourseFormData(SelectCourseFormData formData)
        {
            ApplicationId = formData.ApplicationId;
        }

        [Required(ErrorMessage = "Select a course")]
        public int? ApplicationId { get; set; }
    }
}
