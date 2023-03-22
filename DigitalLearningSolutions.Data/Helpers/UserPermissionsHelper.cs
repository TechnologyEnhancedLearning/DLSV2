namespace DigitalLearningSolutions.Data.Helpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.User;

    public static class UserPermissionsHelper
    {
        public static bool LoggedInAdminCanDeactivateUser(
            AdminAccount adminAccount,
            AdminAccount loggedInAdminAccount
        )
        {
            if (loggedInAdminAccount.IsSuperAdmin)
            {
                return adminAccount.Id != loggedInAdminAccount.Id;
            }

            if (loggedInAdminAccount.IsCentreManager)
            {
                return !adminAccount.IsSuperAdmin
                       && adminAccount.Id != loggedInAdminAccount.Id;
            }

            return false;
        }

        [Obsolete("Use the method that takes parameters of type AdminAccount")]
        public static bool LoggedInAdminCanDeactivateUser(AdminUser adminUser, AdminUser loggedInAdminUser)
        {
            if (loggedInAdminUser.IsUserAdmin)
            {
                return adminUser.Id != loggedInAdminUser.Id;
            }

            if (loggedInAdminUser.IsCentreManager)
            {
                return !adminUser.IsUserAdmin
                       && !adminUser.IsCentreManager
                       && adminUser.Id != loggedInAdminUser.Id;
            }

            return false;
        }
    }
}
