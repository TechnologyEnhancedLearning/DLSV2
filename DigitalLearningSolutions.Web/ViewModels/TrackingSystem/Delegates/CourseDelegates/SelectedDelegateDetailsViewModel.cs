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
            searchLabel: "Search activity")
        {

            Delegates = result.ItemsToDisplay.Select(
                d => new DelegateSelfAssessmentInfoViewModel(
                    d,
                    DelegateAccessRoute.ActivityDelegates,
                    result.GetReturnPageQuery($"{d.DelegateId}-card")
                )
            );

            Filters = SelfAssessmentDelegateViewModelFilterOptions.GetAllSelfAssessmentDelegatesFilterViewModels();
        }

        public bool Active { get; set; }
        public IEnumerable<DelegateSelfAssessmentInfoViewModel> Delegates { get; set; }
        public override IEnumerable<(string, string)> SortOptions { get; } =
            Enumeration.GetAll<SelfAssessmentDelegatesSortByOption>().Select(o => (o.DisplayText, o.PropertyName));
        public override bool NoDataFound => !Delegates.Any() && NoSearchOrFilter;
    }
}
