namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;
    using System;

    public class EditCompleteByDateViewModel : EditCompleteByDateFormData
    {
        public EditCompleteByDateViewModel(
            string name,
            DateTime? completeByDate,
            ReturnPageQuery returnPageQuery,
            DelegateAccessRoute accessedVia,
            int? delegateUserId = null,
            int? selfAssessmentId = null,
            string delegateName=null
        )
        {
            Name = name;
            Day = completeByDate?.Day;
            Month = completeByDate?.Month;
            Year = completeByDate?.Year;
            DelegateUserId = delegateUserId;
            SelfAssessmentId = selfAssessmentId;
            DelegateName = delegateName;
            AccessedVia = accessedVia;
            ReturnPageQuery = returnPageQuery;
        }

        public EditCompleteByDateViewModel(
            EditCompleteByDateFormData formData,
            int delegateUserId,
            int? selfAssessmentId,
            DelegateAccessRoute accessedVia
        ) : base(formData)
        {
            DelegateUserId = delegateUserId;
            SelfAssessmentId = selfAssessmentId;
            AccessedVia = accessedVia;
        }

        public DelegateAccessRoute AccessedVia { get; set; }
    }
}
