namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;

    public class SelfAssessmentDelegateViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionModel> ActiveStatusOptions = new[]
        {
            SelfAssessmentDelegateAccountStatusFilterOptions.Active,
            SelfAssessmentDelegateAccountStatusFilterOptions.Inactive,
        };

        public static readonly IEnumerable<FilterOptionModel> RemovedStatusOptions = new[]
        {
            SelfAssessmentDelegateRemovedFilterOptions.Removed,
            SelfAssessmentDelegateRemovedFilterOptions.NotRemoved,
        };

        public static readonly IEnumerable<FilterOptionModel> SubmittedStatusOptions = new[]
        {
            SelfAssessmentAssessmentSubmittedFilterOptions.Submitted,
            SelfAssessmentAssessmentSubmittedFilterOptions.NotSubmitted,
        };

        public static readonly IEnumerable<FilterOptionModel> SignedOffStatusOptions = new[]
        {
            SelfAssessmentSignedOffFilterOptions.SignedOff,
            SelfAssessmentSignedOffFilterOptions.NotSignedOff,
        };

        public static List<FilterModel> GetAllSelfAssessmentDelegatesFilterViewModels()
        {
            var filters = new List<FilterModel>
            {
                new FilterModel("ActiveStatus", "Delegate active status", ActiveStatusOptions,"status"),
                new FilterModel("RemovedStatus", "Removed status", RemovedStatusOptions, "status"),
                new FilterModel("SubmittedStatus", "Submitted status", SubmittedStatusOptions,"status"),
                new FilterModel("SignedOffStatus", "Signed off status", SignedOffStatusOptions, "status"),
            };
            return filters;
        }
    }
}
