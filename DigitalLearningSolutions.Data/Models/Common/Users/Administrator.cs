using DigitalLearningSolutions.Data.Helpers;
using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

namespace DigitalLearningSolutions.Data.Models.Common.Users
{
    public class Administrator : BaseSearchableItem
    {
        public int AdminID { get; set; }
        public int AdminUserID { get; set; }
        public int CentreID { get; set; }
        public string? Email { get; set; }
        public string? Forename { get; set; }
        public string? Surname { get; set; }
        public bool Active { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsNominatedSupervisor { get; set; }
        public bool IsFrameworkDeveloper { get; set; }
        public byte[]? ProfileImage { get; set; }
        public string? CentreName { get; set; }
        public string SupervisorRoleName =>
            IsSupervisor ? "Supervisor" :
            IsNominatedSupervisor ? "Nominated supervisor" :
            string.Empty;
        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? $"{Forename} {Surname} {Email}";
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
