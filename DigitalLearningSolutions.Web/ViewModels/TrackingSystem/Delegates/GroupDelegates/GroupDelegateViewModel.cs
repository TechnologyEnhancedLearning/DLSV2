﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;

    public class GroupDelegateViewModel
    {
        public GroupDelegateViewModel(GroupDelegate groupDelegate, ReturnPageQuery returnPageQuery)
        {
            GroupDelegateId = groupDelegate.GroupDelegateId;
            GroupId = groupDelegate.GroupId;
            DelegateId = groupDelegate.DelegateId;
            TitleName = groupDelegate.SearchableName;
            Name = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(groupDelegate.FirstName, groupDelegate.LastName);
            EmailAddress = groupDelegate.CentreEmail ?? groupDelegate.PrimaryEmail;
            CandidateNumber = groupDelegate.CandidateNumber;
            ProfessionalRegistrationNumber = PrnHelper.GetPrnDisplayString(
                groupDelegate.HasBeenPromptedForPrn,
                groupDelegate.ProfessionalRegistrationNumber
            );
            ReturnPageQuery = returnPageQuery;
        }

        public int GroupDelegateId { get; set; }

        public int GroupId { get; set; }

        public int DelegateId { get; set; }

        public string TitleName { get; set; }

        public string Name { get; set; }

        public string? EmailAddress { get; set; }

        public string CandidateNumber { get; set; }

        public string? ProfessionalRegistrationNumber { get; set; }

        public ReturnPageQuery ReturnPageQuery { get; set; }
    }
}
