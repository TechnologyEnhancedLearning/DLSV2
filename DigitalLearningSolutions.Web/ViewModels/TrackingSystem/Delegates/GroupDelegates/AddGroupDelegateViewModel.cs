namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;
    using PromptHelper = DigitalLearningSolutions.Web.Helpers.PromptHelper;

    public class AddGroupDelegateViewModel : BaseSearchablePageViewModel
    {
        public AddGroupDelegateViewModel(
            List<DelegateUserCard> delegateUserCards,
            List<(int id, string name)> jobGroups,
            List<CentreRegistrationPrompt> customPrompts,
            int page,
            int groupId,
            string groupName,
            string? searchString,
            string? filterBy
        ) : base(
            searchString,
            page,
            true,
            DefaultSortByOptions.Name.PropertyName,
            GenericSortingHelper.Ascending,
            filterBy
        )
        {
            var searchedItems = GenericSearchHelper.SearchItems(delegateUserCards.AsQueryable(), SearchString);
            var paginatedItems = SortFilterAndPaginate(searchedItems);

            var promptsWithOptions = customPrompts.Where(customPrompt => customPrompt.Options.Count > 0);
            Delegates = paginatedItems.Select(
                delegateUser =>
                {
                    var customFields = PromptHelper.GetDelegateRegistrationPrompts(delegateUser, customPrompts);
                    return new SearchableDelegateViewModel(delegateUser, customFields, promptsWithOptions, page);
                }
            );

            Filters = GroupDelegatesViewModelFilterOptions.GetAddGroupDelegateFilterViewModels(
                jobGroups,
                promptsWithOptions
            );

            GroupId = groupId;
            GroupName = groupName;
        }

        public string GroupName { get; set; }
        public int GroupId { get; set; }

        public IEnumerable<SearchableDelegateViewModel> Delegates { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; }

        public override bool NoDataFound => !Delegates.Any() && NoSearchOrFilter;
    }
}
