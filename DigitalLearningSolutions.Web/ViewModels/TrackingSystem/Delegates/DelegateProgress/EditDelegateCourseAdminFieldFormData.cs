namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class EditDelegateCourseAdminFieldFormData
    {
        public EditDelegateCourseAdminFieldFormData() { }

        protected EditDelegateCourseAdminFieldFormData(DelegateCourseDetails details)
        {
            DelegateId = details.DelegateCourseInfo.DelegateId;
            CustomisationId = details.DelegateCourseInfo.CustomisationId;
        }

        protected EditDelegateCourseAdminFieldFormData(EditDelegateCourseAdminFieldFormData formData)
        {
            Answer = formData.Answer;
            DelegateId = formData.DelegateId;
            CustomisationId = formData.CustomisationId;
        }

        [MaxLength(100, ErrorMessage = "Answer must be 100 characters or fewer")]
        public string? Answer { get; set; }

        public int DelegateId { get; set; }
        public int CustomisationId { get; set; }
    }
}
