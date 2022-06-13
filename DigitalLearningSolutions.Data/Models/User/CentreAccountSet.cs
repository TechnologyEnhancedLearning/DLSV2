namespace DigitalLearningSolutions.Data.Models.User
{
    using System;

    public class CentreAccountSet
    {
        public CentreAccountSet(
            int centreId,
            AdminAccount? adminAccount = null,
            DelegateAccount? delegateAccount = null
        )
        {
            if (adminAccount == null && delegateAccount == null)
            {
                throw new InvalidOperationException(
                    "CentreAccountSet must have at least one of AdminAccount or DelegateAccount"
                );
            }

            CentreId = centreId;
            AdminAccount = adminAccount;
            DelegateAccount = delegateAccount;
        }

        public int CentreId { get; }
        public AdminAccount? AdminAccount { get; }
        public DelegateAccount? DelegateAccount { get; }
        public bool IsCentreActive => AdminAccount?.CentreActive ?? DelegateAccount!.CentreActive;
        public string CentreName => AdminAccount?.CentreName ?? DelegateAccount!.CentreName;
        public bool CanLogIntoAdminAccount => AdminAccount?.Active == true;
        public bool CanLogIntoDelegateAccount => DelegateAccount is { Active: true, Approved: true };
        public bool CanLogInToCentre => IsCentreActive && (CanLogIntoAdminAccount || CanLogIntoDelegateAccount);

        public bool CanLogDirectlyInToCentre => CanLogInToCentre &&
                                                (AdminAccount == null || CanLogIntoAdminAccount) &&
                                                (DelegateAccount == null || CanLogIntoDelegateAccount);
    }
}
