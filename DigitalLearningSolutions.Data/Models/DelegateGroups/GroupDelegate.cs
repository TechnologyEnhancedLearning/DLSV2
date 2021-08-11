namespace DigitalLearningSolutions.Data.Models.DelegateGroups
{
    public class GroupDelegate : BaseSearchableItem
    {
        public int GroupDelegateId { get; set; }

        public int GroupId { get; set; }

        public int DelegateId { get; set; }

        public string? FirstName { get; set; }

        public string LastName { get; set; }

        public string? EmailAddress { get; set; }

        public string CandidateNumber { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? $"{FirstName} {LastName}";
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
