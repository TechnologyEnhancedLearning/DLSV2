namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AddDelegateToGroup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;

    public class AddDelegateToGroupViewModel : AllDelegatesViewModel
    {
        public AddDelegateToGroupViewModel(
            IEnumerable<DelegateUserCard> delegateUserCards,
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CustomPrompt> customPrompts,
            int page,
            int groupId,
            string groupName,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? filterBy
        ) : base(delegateUserCards, jobGroups, customPrompts, page, searchString, sortBy, sortDirection, filterBy)
        {
            GroupId = groupId;
            GroupName = groupName;
        }

        public string? GroupName { get; set; }
        public int GroupId { get; set; }
    }
}
