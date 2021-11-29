namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AddDelegateToGroup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;

    public class AddDelegateToGroupItemsViewModel : AllDelegateItemsViewModel
    {
        public AddDelegateToGroupItemsViewModel(
            IEnumerable<DelegateUserCard> delegateUserCards,
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CustomPrompt> customPrompts,
            int groupId
        )
            : base(delegateUserCards, jobGroups, customPrompts)
        {
            GroupId = groupId;
        }

        public int GroupId { get; set; }
    }
}
