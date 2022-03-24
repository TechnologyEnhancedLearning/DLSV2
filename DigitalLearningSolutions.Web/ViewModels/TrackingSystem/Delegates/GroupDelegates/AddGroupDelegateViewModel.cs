namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;

    public class AddGroupDelegateViewModel : BaseSearchablePageViewModel<DelegateUserCard>
    {
        public AddGroupDelegateViewModel(
            SearchSortFilterPaginationResult<DelegateUserCard> result,
            IEnumerable<FilterModel> availableFilters,
            List<CentreRegistrationPrompt> customPrompts,
            int groupId,
            string groupName
        ) : base(
            result,
            true,
            availableFilters
        )
        {
            GroupId = groupId;
            GroupName = groupName;
            var promptsWithOptions = customPrompts.Where(customPrompt => customPrompt.Options.Count > 0);
            Delegates = result.ItemsToDisplay.Select(
                delegateUser =>
                {
                    var customFields = PromptsService.GetDelegateRegistrationPrompts(delegateUser, customPrompts);
                    return new SearchableDelegateViewModel(delegateUser, customFields, promptsWithOptions, Page);
                }
            );
        }

        public string GroupName { get; set; }
        public int GroupId { get; set; }

        public IEnumerable<SearchableDelegateViewModel> Delegates { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; }

        public override bool NoDataFound => !Delegates.Any() && NoSearchOrFilter;
    }
}
