namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserService
    {
        public (AdminUser?, List<DelegateUser>) GetUsersByUsername(string username);
        public (List<AdminUser>, List<DelegateUser>) GetUsersByEmailAddress(string emailAddress);
        public (AdminUser?, DelegateUser?) GetUsersById(string? adminId, string? delegateId);
    }

    public class UserService : IUserService
    {
        private readonly IUserDataService userDataService;

        public UserService(IUserDataService userDataService)
        {
            this.userDataService = userDataService;
        }

        public (AdminUser?, List<DelegateUser>) GetUsersByUsername(string username)
        {
            var adminUser = userDataService.GetAdminUserByUsername(username);
            var delegateUsername =
                string.IsNullOrWhiteSpace(adminUser?.EmailAddress) ? username : adminUser.EmailAddress;
            List<DelegateUser> delegateUsers = userDataService.GetDelegateUsersByUsername(delegateUsername);

            return (adminUser, delegateUsers);
        }

        public (List<AdminUser>, List<DelegateUser>) GetUsersByEmailAddress(string emailAddress)
        {
            List<AdminUser> adminUsers = userDataService.GetAdminUsersByEmailAddress(emailAddress);
            List<DelegateUser> delegateUsers = userDataService.GetDelegateUsersByEmailAddress(emailAddress);

            return (adminUsers, delegateUsers);
        }

        public (AdminUser?, DelegateUser?) GetUsersById(string? userAdminId, string? userDelegateId)
        {
            AdminUser? adminUser = null;

            if (int.TryParse(userAdminId, out int adminId))
            {
                adminUser = userDataService.GetAdminUserById(adminId);
            }

            DelegateUser? delegateUser = null;

            if (int.TryParse(userDelegateId, out var delegateId))
            {
                delegateUser = userDataService.GetDelegateUserById(delegateId);
            }

            return (adminUser, delegateUser);
        }
    }
}
