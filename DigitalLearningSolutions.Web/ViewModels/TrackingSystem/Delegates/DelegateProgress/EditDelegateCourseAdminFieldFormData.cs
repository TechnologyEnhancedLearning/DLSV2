namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class EditDelegateCourseAdminFieldFormData
    {
        public EditDelegateCourseAdminFieldFormData() { }

        protected EditDelegateCourseAdminFieldFormData(DelegateCourseDetails details, int? returnPage)
        {
            DelegateId = details.DelegateCourseInfo.DelegateId;
            CustomisationId = details.DelegateCourseInfo.CustomisationId;
            ReturnPage = returnPage;
        }

        protected EditDelegateCourseAdminFieldFormData(EditDelegateCourseAdminFieldFormData formData, int? returnPage)
        {
            DelegateId = formData.DelegateId;
            CustomisationId = formData.CustomisationId;
            ReturnPage = returnPage;
        }

        [MaxLength(100, ErrorMessage = "Answer must be 100 characters or fewer")]
        public string? Answer { get; set; }

        public int DelegateId { get; set; }
        public int CustomisationId { get; set; }
        public int? ReturnPage { get; set; }
    }
}
