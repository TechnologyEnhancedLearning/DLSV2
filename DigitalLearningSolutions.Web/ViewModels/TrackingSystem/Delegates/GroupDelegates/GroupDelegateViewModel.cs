namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;

    public class GroupDelegateViewModel
    {
        public GroupDelegateViewModel(GroupDelegate groupDelegate)
        {
            GroupDelegateId = groupDelegate.GroupDelegateId;
            GroupId = groupDelegate.GroupId;
            DelegateId = groupDelegate.DelegateId;
            TitleName = DisplayStringHelper.GetNameWithEmailForDisplay(groupDelegate.SearchableName, groupDelegate.EmailAddress);
            Name = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(groupDelegate.FirstName, groupDelegate.LastName);
            EmailAddress = groupDelegate.EmailAddress;
            CandidateNumber = groupDelegate.CandidateNumber;
            ProfessionalRegistrationNumber = groupDelegate.ProfessionalRegistrationNumber;
        }

        public int GroupDelegateId { get; set; }

        public int GroupId { get; set; }

        public int DelegateId { get; set; }

        public string TitleName { get; set; }

        public string Name { get; set; }

        public string? EmailAddress { get; set; }

        public string CandidateNumber { get; set; }

        public string? ProfessionalRegistrationNumber { get; set; }
    }
}
