namespace DigitalLearningSolutions.Data.Models.User
{
    using System.Collections.Generic;
    using System.Linq;

    public class UserAccountSet
    {
        public readonly List<AdminUser> AdminAccounts;
        public readonly List<DelegateUser> DelegateAccounts;

        public UserAccountSet() : this(null, null) { }

        public UserAccountSet(List<AdminUser>? adminAccounts, IEnumerable<DelegateUser?>? delegateAccounts)
        {
            AdminAccounts = adminAccounts ?? new List<AdminUser>();
            DelegateAccounts = delegateAccounts?.Where(u => u != null).Select(u => u!).ToList() ??
                               new List<DelegateUser>();
        }

        public bool Any()
        {
            return AdminAccounts.Any() || DelegateAccounts.Any();
        }

        public List<UserReference> GetUserRefs()
        {
            var delegateRefs = DelegateAccounts.Select(user => user.ToUserReference());
            var adminRefs = AdminAccounts.Select(user => user.ToUserReference());
            return delegateRefs.Concat(adminRefs).ToList();
        }

        public void Deconstruct(out List<AdminUser> adminUsers, out List<DelegateUser> delegateUsers)
        {
            adminUsers = AdminAccounts;
            delegateUsers = DelegateAccounts;
        }
    }
}
