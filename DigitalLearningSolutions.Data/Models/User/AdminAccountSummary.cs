namespace DigitalLearningSolutions.Data.Models.User
{
    /// <summary>
    /// A lightweight view of an active admin account, used when an email address
    /// matches admin accounts at more than one centre and the user needs to choose one.
    /// </summary>
    public class AdminAccountSummary
    {
        public int AdminId { get; set; }
        public string Email { get; set; } = string.Empty;
        public int CentreId { get; set; }
        public string? CentreName { get; set; }

        public string DisplayName => string.IsNullOrWhiteSpace(CentreName)
            ? Email
            : $"{Email} ({CentreName})";
    }
}
