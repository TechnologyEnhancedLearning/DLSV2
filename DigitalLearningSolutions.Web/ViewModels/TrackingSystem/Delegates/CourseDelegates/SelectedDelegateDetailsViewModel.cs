namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public class SelectedDelegateDetailsViewModel : BaseSearchablePageViewModel<SelfAssessmentDelegate>
    {
        public SelectedDelegateDetailsViewModel(
            SearchSortFilterPaginationResult<SelfAssessmentDelegate> result,
            IEnumerable<FilterModel> availableFilters,
            SelfAssessmentDelegatesData selfAssessmentDelegatesData,
            Dictionary<string, string> routeData
        ) : base(
            result,
            true,
            availableFilters,
            routeData: routeData,
            searchLabel: "Search")
        {

            Delegates = result.ItemsToDisplay.Select(
                d => new DelegateSelfAssessmentInfoViewModel(
                    d,
                    DelegateAccessRoute.ActivityDelegates,
                    result.GetReturnPageQuery($"{d.DelegateId}-card")
                )
            );

            Filters = routeData["selfAssessmentId"]?.ToString() == "1" ?
                SelfAssessmentDelegateViewModelFilterOptions.GetAllSelfAssessmentDelegatesFilterViewModels().Where(x => x.FilterProperty != "SignedOffStatus") :
                SelfAssessmentDelegateViewModelFilterOptions.GetAllSelfAssessmentDelegatesFilterViewModels().Where(x => x.FilterProperty != "SubmittedStatus");

            SortOptions = routeData["selfAssessmentId"]?.ToString() == "1" ?
                Enumeration.GetAll<SelfAssessmentDelegatesSortByOption>().Where(x => x.PropertyName != "SignedOff").Select(o => (o.DisplayText, o.PropertyName)) :
                Enumeration.GetAll<SelfAssessmentDelegatesSortByOption>().Where(x => x.PropertyName != "SubmittedDate").Select(o => (o.DisplayText, o.PropertyName));

        }

        public bool Active { get; set; }
        public IEnumerable<DelegateSelfAssessmentInfoViewModel> Delegates { get; set; }
        public override IEnumerable<(string, string)> SortOptions { get; }
        public override bool NoDataFound => !Delegates.Any() && NoSearchOrFilter;
    }
}
