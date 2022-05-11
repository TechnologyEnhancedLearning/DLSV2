namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class EditDelegateCourseAdminFieldFormData
    {
        public EditDelegateCourseAdminFieldFormData() { }

        protected EditDelegateCourseAdminFieldFormData(
            DelegateCourseInfo delegateCourseInfo,
            ReturnPageQuery? returnPageQuery = null
        )
        {
            DelegateId = delegateCourseInfo.DelegateId;
            CustomisationId = delegateCourseInfo.CustomisationId;
            ReturnPageQuery = returnPageQuery;
        }

        protected EditDelegateCourseAdminFieldFormData(EditDelegateCourseAdminFieldFormData formData)
        {
            Answer = formData.Answer;
            DelegateId = formData.DelegateId;
            CustomisationId = formData.CustomisationId;
            ReturnPageQuery = formData.ReturnPageQuery;
        }

        [MaxLength(100, ErrorMessage = "Answer must be 100 characters or fewer")]
        public string? Answer { get; set; }

        public int DelegateId { get; set; }
        public int CustomisationId { get; set; }
        public ReturnPageQuery? ReturnPageQuery { get; set; }
    }
}
