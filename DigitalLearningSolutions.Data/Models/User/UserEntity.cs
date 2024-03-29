﻿namespace DigitalLearningSolutions.Data.Models.User
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

        public IDictionary<int, CentreAccountSet> CentreAccountSetsByCentreId
        {
            get
            {
                var centreAccountSet = DelegateAccounts.Select(
                    delegateAccount => new CentreAccountSet(
                        delegateAccount.CentreId,
                        AdminAccounts.SingleOrDefault(
                            adminAccount => adminAccount.CentreId == delegateAccount.CentreId
                        ),
                        delegateAccount
                    )
                ).ToList();

                var adminOnlyAccounts = AdminAccounts.Where(
                    aa => centreAccountSet.All(account => account.CentreId != aa.CentreId)
                );

                centreAccountSet.AddRange(
                    adminOnlyAccounts.Select(account => new CentreAccountSet(account.CentreId, account))
                );

                return centreAccountSet.ToDictionary(accounts => accounts.CentreId);
            }
        }

        public CentreAccountSet? GetCentreAccountSet(int? centreId)
        {
            if (centreId == null)
            {
                return null;
            }

            CentreAccountSetsByCentreId.TryGetValue(centreId.Value, out var centreAccountSet);
            return centreAccountSet;
        }

        public bool IsSingleCentreAccount => CentreAccountSetsByCentreId.Count == 1;
    }
}
