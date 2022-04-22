namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class EditCompleteByDateViewModel : EditCompleteByDateFormData
    {
        public EditCompleteByDateViewModel(
            int progressId,
            DelegateAccessRoute accessedVia,
            DelegateCourseInfo info,
            ReturnPageQuery returnPageQuery
        ) : base(info)
        {
            ProgressId = progressId;
            AccessedVia = accessedVia;
            ReturnPageQuery = returnPageQuery;
        }

        public EditCompleteByDateViewModel(
            EditCompleteByDateFormData formData,
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
