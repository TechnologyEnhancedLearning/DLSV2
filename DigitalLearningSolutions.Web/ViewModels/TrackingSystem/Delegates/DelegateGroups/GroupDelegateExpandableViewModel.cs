using DigitalLearningSolutions.Data.Models.DelegateGroups;

namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    public class GroupDelegateExpandableViewModel
    {
        public GroupDelegateExpandableViewModel(GroupDelegate groupDelegate)
        {
            GroupDelegateId = groupDelegate.GroupDelegateId;
            Name = (string.IsNullOrEmpty(groupDelegate.FirstName) ? "" : $"{groupDelegate.FirstName} ") + groupDelegate.LastName;
            EmailAddress = groupDelegate.EmailAddress;
            CandidateNumber = groupDelegate.CandidateNumber;
        }

        public int GroupDelegateId { get; set; }

        public string Name { get; set; }
        
        public string? EmailAddress { get; set; }

        public string CandidateNumber { get; set; }
    }
}
