namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;

    public class SelectDelegateAllItemsViewModel : AllDelegateItemsViewModel
    {
        public SelectDelegateAllItemsViewModel(
            IEnumerable<DelegateUserCard> delegateUserCards,
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CentreRegistrationPrompt> customPrompts,
            int groupId,
            IEnumerable<(int id, string name)> groups
        )
            : base(delegateUserCards, jobGroups, customPrompts, groups)
        {
            GroupId = groupId;
        }

        public int GroupId { get; set; }
    }
}
