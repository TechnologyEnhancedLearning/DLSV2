namespace DigitalLearningSolutions.Data.Helpers
{
    using DigitalLearningSolutions.Data.Models.User;

    public static class UserPermissionsHelper
    {
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
