namespace DigitalLearningSolutions.Data.Models.User
{
    using System.Collections.Generic;
    using System.Linq;

    public class UserAccountSet
    {
        public readonly AdminUser? AdminAccount;
        public readonly List<DelegateUser> DelegateAccounts;

        public UserAccountSet(AdminUser? adminAccount, IEnumerable<DelegateUser?>? delegateAccounts)
        {
            AdminAccount = adminAccount;
            DelegateAccounts = delegateAccounts?.Where(u => u != null).Select(u => u!).ToList() ??
                               new List<DelegateUser>();
        }

        public bool Any()
        {
            return AdminAccount != null || DelegateAccounts.Any();
        }


        public void Deconstruct(out AdminUser? adminUser, out List<DelegateUser> delegateUsers)
        {
            adminUser = AdminAccount;
            delegateUsers = DelegateAccounts;
        }
    }
}
