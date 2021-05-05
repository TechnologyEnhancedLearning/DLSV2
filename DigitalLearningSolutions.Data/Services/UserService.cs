namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserService
    {
        public (AdminUser?, List<DelegateUser>) GetUsersByUsername(string username);
        public (AdminUser?, List<DelegateUser>) GetUsersByEmailAddress(string emailAddress);
        public (AdminUser?, DelegateUser?) GetUsersById(int? adminId, int? delegateId);
        public List<CentreUserDetails> GetUserCentres(AdminUser adminUser, List<DelegateUser> delegateUsers);
        public bool TryUpdateUserAccountDetails(int? adminId, int? delegateId, string password, string firstName, string surname, string email);
    }

    public class UserService : IUserService
    {
        private readonly IUserDataService userDataService;
        private readonly ILoginService loginService;

        public UserService(IUserDataService userDataService, ILoginService loginService)
        {
            this.userDataService = userDataService;
            this.loginService = loginService;
        }

        public (AdminUser?, List<DelegateUser>) GetUsersByUsername(string username)
        {
            var adminUser = userDataService.GetAdminUserByUsername(username);
            var delegateUsername =
                string.IsNullOrWhiteSpace(adminUser?.EmailAddress) ? username : adminUser.EmailAddress;
            List<DelegateUser> delegateUsers = userDataService.GetDelegateUsersByUsername(delegateUsername);

            return (adminUser, delegateUsers);
        }

        public (AdminUser?, List<DelegateUser>) GetUsersByEmailAddress(string emailAddress)
        {
            var adminUser = userDataService.GetAdminUserByEmailAddress(emailAddress);
            var delegateUsers = userDataService.GetDelegateUsersByEmailAddress(emailAddress);

            return (adminUser, delegateUsers);
        }

        public (AdminUser?, DelegateUser?) GetUsersById(int? userAdminId, int? userDelegateId)
        {
            AdminUser? adminUser = null;

            if (userAdminId != null)
            {
                adminUser = userDataService.GetAdminUserById(userAdminId.Value);
            }

            DelegateUser? delegateUser = null;

            if (userDelegateId != null)
            {
                delegateUser = userDataService.GetDelegateUserById(userDelegateId.Value);
            }

            return (adminUser, delegateUser);
        }

        public List<CentreUserDetails> GetUserCentres(AdminUser? adminUser, List<DelegateUser> delegateUsers)
        {
            var availableCentres = delegateUsers
                .Select(du =>
                    new CentreUserDetails(du.CentreId, du.CentreName, adminUser?.CentreId == du.CentreId, true))
                .ToList();

            if (adminUser != null && availableCentres.All(c => c.CentreId != adminUser.CentreId))
            {
                availableCentres.Add(
                    new CentreUserDetails(adminUser.CentreId, adminUser.CentreName, true));
            }

            return availableCentres.OrderByDescending(ac => ac.IsAdmin).ThenBy(ac => ac.CentreName).ToList();
        }

        public bool TryUpdateUserAccountDetails(int? adminId, int? delegateId, string password, string firstName, string surname, string email)
        {
            var (loggedInAdminUser, loggedInDelegateUser) = GetUsersById(adminId, delegateId);

            var signedInEmail = loggedInAdminUser?.EmailAddress ?? loggedInDelegateUser?.EmailAddress;

            var (adminUser, delegateUsers) = GetUsersByEmailAddress(signedInEmail);

            var (verifiedAdminUser, verifiedDelegateUsers) =
                loginService.VerifyUsers(password, adminUser, delegateUsers);

            if (verifiedAdminUser == null && verifiedDelegateUsers.Count == 0)
            {
                return false;
            }

            if (verifiedAdminUser != null)
            {
                userDataService.UpdateAdminUser(firstName, surname, email, verifiedAdminUser.Id);
            }

            if (verifiedDelegateUsers.Count != 0)
            {
                userDataService.UpdateDelegateUsers(firstName, surname, email, verifiedDelegateUsers.Select(d => d.Id).ToArray());
            }
            
            return true;
        }
    }
}
