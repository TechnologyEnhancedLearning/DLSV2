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

        public UserAccount UserAccount { get; set; }
        public IEnumerable<AdminAccount> AdminAccounts { get; set; }
        public IEnumerable<DelegateAccount> DelegateAccounts { get; set; }

        public bool IsLocked => UserAccount.FailedLoginCount >= AuthHelper.FailedLoginThreshold && AdminAccounts.Any();

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
