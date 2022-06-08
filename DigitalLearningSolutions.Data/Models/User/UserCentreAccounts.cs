namespace DigitalLearningSolutions.Data.Models.User
{
    public class UserCentreAccounts
    {
        public UserCentreAccounts(
            int centreId,
            AdminAccount? adminAccount = null,
            DelegateAccount? delegateAccount = null
        )
        {
            CentreId = centreId;
            AdminAccount = adminAccount;
            DelegateAccount = delegateAccount;
        }

        public int CentreId { get; }
        public AdminAccount? AdminAccount { get; }
        public DelegateAccount? DelegateAccount { get; }
        public bool IsCentreActive => AdminAccount?.CentreActive ?? DelegateAccount?.CentreActive ?? false;

        public bool IsActive => (AdminAccount != null || DelegateAccount != null) &&
                                AdminAccount?.Active != false &&
                                DelegateAccount?.Active != false;

        public string? CentreName => AdminAccount?.CentreName ?? DelegateAccount?.CentreName;
    }
}
