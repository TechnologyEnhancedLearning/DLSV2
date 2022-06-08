namespace DigitalLearningSolutions.Data.Models.User
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;

    public class UserEntity
    {
        public UserEntity(
            UserAccount userAccount,
            IEnumerable<AdminAccount> adminAccounts,
            IEnumerable<DelegateAccount> delegateAccounts
        )
        {
            UserAccount = userAccount;
            AdminAccounts = adminAccounts;
            DelegateAccounts = delegateAccounts;
        }

        public UserAccount UserAccount { get; }
        public IEnumerable<AdminAccount> AdminAccounts { get; }
        public IEnumerable<DelegateAccount> DelegateAccounts { get; }

        private bool AllAdminAccountsInactive => AdminAccounts.All(a => !a.Active);

        public bool IsLocked => UserAccount.FailedLoginCount >= AuthHelper.FailedLoginThreshold &&
                                AdminAccounts.Any() && !AllAdminAccountsInactive;

        public IDictionary<int, UserCentreAccounts> CentreAccounts
        {
            get
            {
                var centreAccounts = DelegateAccounts.Select(
                    delegateAccount => new UserCentreAccounts(
                        delegateAccount.CentreId,
                        AdminAccounts.FirstOrDefault(adminAccount => adminAccount.CentreId == delegateAccount.CentreId),
                        delegateAccount
                    )
                ).ToList();

                var adminOnlyAccounts = AdminAccounts.Where(
                    aa => centreAccounts.All(account => account.CentreId != aa.CentreId)
                );

                centreAccounts.AddRange(
                    adminOnlyAccounts.Select(account => new UserCentreAccounts(account.CentreId, account))
                );

                return centreAccounts.ToDictionary(accounts => accounts.CentreId);
            }
        }

        public UserCentreAccounts? GetCentreAccounts(int centreId)
        {
            CentreAccounts.TryGetValue(centreId, out var centreAccounts);
            return centreAccounts;
        }

        public bool IsSingleCentreAccount()
        {
            if (AdminAccounts.Count() > 1 || DelegateAccounts.Count() > 1)
            {
                return false;
            }

            var adminCentreId = AdminAccounts.SingleOrDefault()?.CentreId;
            var delegateCentreId = DelegateAccounts.SingleOrDefault()?.CentreId;

            return adminCentreId == null || delegateCentreId == null || adminCentreId == delegateCentreId;
        }
    }
}
