namespace DigitalLearningSolutions.Data.Models.DelegateGroups
{
    public class GroupDelegateAdmin
    {
        public int AdminId { get; set; }

        public int GroupId { get; set; }

        public string? Forename { get; set; }

        public string? Surname { get; set; }

        public string? FullName { get; set; }

        public bool Active { get; set; }
    }
}
