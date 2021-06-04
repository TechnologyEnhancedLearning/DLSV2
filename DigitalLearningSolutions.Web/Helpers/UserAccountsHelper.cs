namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;

    public static class UserAccountsHelper
    {
        public static bool Any(this (AdminUser?, List<DelegateUser>) users)
        {
            var (adminUser, delegateUsers) = users;
            return adminUser != null || delegateUsers.Any();
        }
    }
}
