namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class EditCompletionDateViewModel : EditCompletionDateFormData
    {
        public EditCompletionDateViewModel(
            int progressId,
            DelegateAccessRoute accessedVia,
            DelegateCourseInfo info,
            ReturnPageQuery? returnPageQuery
        ) : base(info, returnPageQuery)
        {
            ProgressId = progressId;
            AccessedVia = accessedVia;
        }

        public EditCompletionDateViewModel(
            EditCompletionDateFormData formData,
            int progressId,
            DelegateAccessRoute accessedVia
        ) : base(formData)
        {
            ProgressId = progressId;
            AccessedVia = accessedVia;
        }

        public int ProgressId { get; set; }

        public DelegateAccessRoute AccessedVia { get; set; }
    }
}
