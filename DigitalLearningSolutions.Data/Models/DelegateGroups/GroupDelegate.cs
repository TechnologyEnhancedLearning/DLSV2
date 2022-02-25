namespace DigitalLearningSolutions.Data.Models.DelegateGroups
{
    using System;
    using DigitalLearningSolutions.Data.Helpers;

    public class GroupDelegate : BaseSearchableItem
    {
        public int GroupDelegateId { get; set; }

        public int GroupId { get; set; }

        public int DelegateId { get; set; }

        public string? FirstName { get; set; }

        public string LastName { get; set; }

        public string? EmailAddress { get; set; }

        public string CandidateNumber { get; set; }

        public DateTime AddedDate { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? NameQueryHelper.GetSortableFullName(FirstName, LastName);
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
