namespace DigitalLearningSolutions.Data.Models.User
{
    public class CentreAccountSet
    {
        public CentreAccountSet(
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

        public string? CentreName => AdminAccount?.CentreName ?? DelegateAccount?.CentreName;
        public AdminAccount? ActiveAdminAccount => AdminAccount?.Active == true ? AdminAccount : null;

        public DelegateAccount? ActiveApprovedDelegateAccount =>
            DelegateAccount is { Active: true, Approved: true } ? DelegateAccount : null;

        // True if there is either an active admin account, or an active approved delegate account
        public bool CanLogInToCentre =>
            IsCentreActive && (ActiveAdminAccount != null || ActiveApprovedDelegateAccount != null);

        // Only true if neither admin nor delegate account is inactive, and the delegate account is not unapproved
        public bool CanLogDirectlyInToCentre => IsCentreActive && AdminAccount?.Active != false &&
                                                DelegateAccount?.Active != false && DelegateAccount?.Approved != false;
    }
}
