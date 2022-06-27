namespace DigitalLearningSolutions.Data.Models.DelegateGroups
{
    using System;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class GroupDelegate : BaseSearchableItem
    {
        public int GroupDelegateId { get; set; }

        public int GroupId { get; set; }

        public int DelegateId { get; set; }

        public string? FirstName { get; set; }

        public string LastName { get; set; }

        public string PrimaryEmail { get; set; }

        public string CandidateNumber { get; set; }

        public DateTime AddedDate { get; set; }

        public bool HasBeenPromptedForPrn { get; set; }

        public string? ProfessionalRegistrationNumber { get; set; }

        public string? CentreEmail { get; set; }

        public DateTime? CentreEmailVerified { get; set; }

        public string EmailForCentreNotifications => CentreEmailHelper.GetEmailForCentreNotifications(
            PrimaryEmail,
            CentreEmail,
            CentreEmailVerified
        );

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? NameQueryHelper.GetSortableFullName(FirstName, LastName);
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
