namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserService
    {
        (List<AdminUser>, List<DelegateUser>) GetUsersByUsername(string username);
        (AdminUser? adminUser, List<DelegateUser> delegateUsers) GetUsersByEmailAddress(string emailAddress);
        (AdminUser? adminUser, DelegateUser? delegateUser) GetUsersById(int? adminId, int? delegateId);

        (AdminUser?, List<DelegateUser>) GetUsersWithActiveCentres(
            AdminUser? adminUser,
            List<DelegateUser> delegateUsers
        );

        List<CentreUserDetails> GetUserCentres(AdminUser? adminUser, List<DelegateUser> delegateUsers);

        void UpdateUserAccountDetails(
            AccountDetailsData accountDetailsData,
            CentreAnswersData? centreAnswersData = null
        );

        bool NewEmailAddressIsValid(string emailAddress, int? adminUserId, int? delegateUserId, int centreId);

        UserAccountSet GetVerifiedLinkedUsersAccounts(int? adminId, int? delegateId, string password);

        bool IsPasswordValid(int? adminId, int? delegateId, string password);

        bool IsDelegateEmailValidForCentre(string email, int centreId);

        void ResetFailedLoginCount(AdminUser adminUser);

        void IncrementFailedLoginCount(AdminUser adminUser);
    }

    public class UserService : IUserService
    {
        private readonly ILoginService loginService;
        private readonly IUserDataService userDataService;

        public UserService(IUserDataService userDataService, ILoginService loginService)
        {
            this.userDataService = userDataService;
            this.loginService = loginService;
        }

        public (List<AdminUser>, List<DelegateUser>) GetUsersByUsername(string username)
        {
            var adminUsers = userDataService.GetAdminUsersByUsername(username);
            List<DelegateUser> delegateUsers = userDataService.GetDelegateUsersByUsername(username);

            return (adminUsers, delegateUsers);
        }

        public (AdminUser? adminUser, List<DelegateUser> delegateUsers) GetUsersByEmailAddress(string emailAddress)
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

        public (AdminUser?, List<DelegateUser>) GetUsersWithActiveCentres(
            AdminUser? adminUser,
            List<DelegateUser> delegateUsers
        )
        {
            var adminUserWithActiveCentre = adminUser?.CentreActive == true ? adminUser : null;
            var delegateUsersWithActiveCentres = delegateUsers.Where(du => du.CentreActive).ToList();
            return (adminUserWithActiveCentre, delegateUsersWithActiveCentres);
        }

        public List<CentreUserDetails> GetUserCentres(AdminUser? adminUser, List<DelegateUser> delegateUsers)
        {
            var availableCentres = delegateUsers
                .Select(
                    du =>
                        new CentreUserDetails(du.CentreId, du.CentreName, adminUser?.CentreId == du.CentreId, true)
                )
                .ToList();

            if (adminUser != null && availableCentres.All(c => c.CentreId != adminUser.CentreId))
            {
                availableCentres.Add(new CentreUserDetails(adminUser.CentreId, adminUser.CentreName, true));
            }

            return availableCentres.OrderByDescending(ac => ac.IsAdmin).ThenBy(ac => ac.CentreName).ToList();
        }

        public void UpdateUserAccountDetails(
            AccountDetailsData accountDetailsData,
            CentreAnswersData? centreAnswersData = null
        )
        {
            var (verifiedAdminUsers, verifiedDelegateUsers) =
                GetVerifiedLinkedUsersAccounts(
                    accountDetailsData.AdminId,
                    accountDetailsData.DelegateId,
                    accountDetailsData.Password
                );

            if (verifiedAdminUsers.Count != 0)
            {
                userDataService.UpdateAdminUser(
                    accountDetailsData.FirstName,
                    accountDetailsData.Surname,
                    accountDetailsData.Email,
                    accountDetailsData.ProfileImage,
                    verifiedAdminUsers.Single().Id
                );
            }

            if (verifiedDelegateUsers.Count != 0)
            {
                var delegateIds = verifiedDelegateUsers.Select(d => d.Id).ToArray();
                userDataService.UpdateDelegateUsers(
                    accountDetailsData.FirstName,
                    accountDetailsData.Surname,
                    accountDetailsData.Email,
                    accountDetailsData.ProfileImage,
                    delegateIds
                );

                if (verifiedDelegateUsers.Any(u => u.Id == accountDetailsData.DelegateId) && centreAnswersData != null)
                {
                    userDataService.UpdateDelegateUserCentrePrompts(
                        accountDetailsData.DelegateId!.Value,
                        centreAnswersData.JobGroupId,
                        centreAnswersData.Answer1,
                        centreAnswersData.Answer2,
                        centreAnswersData.Answer3,
                        centreAnswersData.Answer4,
                        centreAnswersData.Answer5,
                        centreAnswersData.Answer6
                    );
                }
            }
        }

        public bool NewEmailAddressIsValid(string emailAddress, int? adminUserId, int? delegateUserId, int centreId)
        {
            var (adminUser, delegateUser) = GetUsersById(adminUserId, delegateUserId);
            if (!UserEmailHasChanged(adminUser, emailAddress) && !UserEmailHasChanged(delegateUser, emailAddress))
            {
                return true;
            }

            var (adminUsersWithNewEmail, delegateUsersWithNewEmail) = GetUsersByEmailAddress(emailAddress);

            return adminUsersWithNewEmail == null && delegateUsersWithNewEmail.Count(u => u.CentreId == centreId) == 0;
        }

        public UserAccountSet GetVerifiedLinkedUsersAccounts(
            int? adminId,
            int? delegateId,
            string password
        )
        {
            var (loggedInAdminUser, loggedInDelegateUser) = GetUsersById(adminId, delegateId);

            var signedInEmailIfAny = loggedInAdminUser?.EmailAddress ?? loggedInDelegateUser?.EmailAddress;

            if (string.IsNullOrWhiteSpace(signedInEmailIfAny))
            {
                var loggedInAdminUsers = loggedInAdminUser != null
                    ? new List<AdminUser> { loggedInAdminUser }
                    : new List<AdminUser>();

                var loggedInDelegateUsers = loggedInDelegateUser != null
                    ? new List<DelegateUser> { loggedInDelegateUser }
                    : new List<DelegateUser>();

                return loginService.VerifyUsers(password, loggedInAdminUsers, loggedInDelegateUsers);
            }

            var (adminUser, delegateUsers) = GetUsersByEmailAddress(signedInEmailIfAny);

            var adminUsers = adminUser != null
                ? new List<AdminUser> { adminUser }
                : new List<AdminUser>();

            var temp = loginService.VerifyUsers(password, adminUsers, delegateUsers);

            return temp;
        }

        public bool IsPasswordValid(int? adminId, int? delegateId, string password)
        {
            var verifiedLinkedUsersAccounts = GetVerifiedLinkedUsersAccounts(adminId, delegateId, password);

            return verifiedLinkedUsersAccounts.Any();
        }

        public bool IsDelegateEmailValidForCentre(string email, int centreId)
        {
            var duplicateUsers = userDataService.GetDelegateUsersByEmailAddress(email)
                .Where(u => u.CentreId == centreId);

            return !duplicateUsers.Any();
        }

        public void ResetFailedLoginCount(AdminUser adminUser)
        {
            if (adminUser.FailedLoginCount != 0)
            {
                userDataService.UpdateAdminUserFailedLoginCount(adminUser.Id, 0);
            }
        }

        public void IncrementFailedLoginCount(AdminUser adminUser)
        {
            userDataService.UpdateAdminUserFailedLoginCount(adminUser.Id, adminUser.FailedLoginCount + 1);
        }

        private static bool UserEmailHasChanged(User? user, string emailAddress)
        {
            return user != null && user.EmailAddress != emailAddress;
        }
    }
}
