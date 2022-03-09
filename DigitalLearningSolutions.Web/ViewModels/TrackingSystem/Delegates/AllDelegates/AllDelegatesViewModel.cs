namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllDelegatesViewModel : BaseSearchablePageViewModel<DelegateUserCard>
    {
        public AllDelegatesViewModel(
            SearchSortFilterPaginateResult<DelegateUserCard> result,
            IReadOnlyCollection<CentreRegistrationPrompt> centreRegistrationPrompts,
            IEnumerable<FilterModel> availableFilters
        ) : base(
            result,
            true,
            availableFilters,
            "Search delegates"
        )
        {
            var promptsWithOptions = centreRegistrationPrompts.Where(registrationPrompt => registrationPrompt.Options.Count > 0);
            var returnPage = string.IsNullOrWhiteSpace(SearchString) ? Page : 1;
            Delegates = result.ItemsToDisplay.Select(
                delegateUser =>
                {
                    var delegateRegistrationPrompts = PromptsService.GetDelegateRegistrationPrompts(delegateUser, centreRegistrationPrompts);
                    return new SearchableDelegateViewModel(delegateUser, delegateRegistrationPrompts, promptsWithOptions, returnPage);
                }
            );
        }

        public IEnumerable<SearchableDelegateViewModel> Delegates { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DelegateSortByOptions.Name,
            DelegateSortByOptions.RegistrationDate,
        };

        public override bool NoDataFound => !Delegates.Any() && NoSearchOrFilter;
    }
}
