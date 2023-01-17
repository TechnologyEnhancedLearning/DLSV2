namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public abstract class User : BaseSearchableItem
    {
        public int Id { get; set; }

        public int CentreId { get; set; }

        public string CentreName { get; set; }

        public bool Active { get; set; }

        public bool Approved { get; set; }

        public bool CentreActive { get; set; }

        public string? RegistrationConfirmationHash { get; set; }

        public string? FirstName { get; set; }

        public string LastName { get; set; }

        public string? EmailAddress { get; set; }

        public string? Password { get; set; }

        public int? ResetPasswordId { get; set; }

        public byte[]? ProfileImage { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? NameQueryHelper.GetSortableFullName(FirstName, LastName);
            set => SearchableNameOverrideForFuzzySharp = value;
        }

        public string FullName => FirstName == null ? LastName : $"{FirstName} {LastName}";

        public abstract UserReference ToUserReference();
    }
}
