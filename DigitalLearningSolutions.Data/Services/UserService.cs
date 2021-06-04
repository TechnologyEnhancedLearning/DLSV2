namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserService
    {
        public (AdminUser? adminUser, List<DelegateUser> delegateUsers) GetUsersByUsername(string username);
        public (AdminUser? adminUser, List<DelegateUser> delegateUsers) GetUsersByEmailAddress(string emailAddress);
        public (AdminUser? adminUser, DelegateUser? delegateUser) GetUsersById(int? adminId, int? delegateId);

        public (AdminUser?, List<DelegateUser>) GetUsersWithActiveCentres(
            AdminUser? adminUser,
            List<DelegateUser> delegateUsers
        );

        public List<CentreUserDetails> GetUserCentres(AdminUser? adminUser, List<DelegateUser> delegateUsers);

        public bool TryUpdateUserAccountDetails(
            AccountDetailsData accountDetailsData,
            CentreAnswersData? centreAnswersData = null
        );

        public bool NewEmailAddressIsValid(string emailAddress, int? adminUserId, int? delegateUserId, int centreId);

        (AdminUser?, List<DelegateUser>) GetVerifiedLinkedUsersAccounts(int? adminId, int? delegateId, string password);
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

        public bool TryUpdateUserAccountDetails(
            AccountDetailsData accountDetailsData,
            CentreAnswersData? centreAnswersData = null
        )
        {
            var (verifiedAdminUser, verifiedDelegateUsers) =
                GetVerifiedLinkedUsersAccounts(
                    accountDetailsData.AdminId,
                    accountDetailsData.DelegateId,
                    accountDetailsData.Password
                );

            if (verifiedAdminUser == null && verifiedDelegateUsers.Count == 0)
            {
                return false;
            }

            if (verifiedAdminUser != null)
            {
                userDataService.UpdateAdminUser(
                    accountDetailsData.FirstName,
                    accountDetailsData.Surname,
                    accountDetailsData.Email,
                    accountDetailsData.ProfileImage,
                    verifiedAdminUser.Id
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

            return true;
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

        private static bool UserEmailHasChanged(User? user, string emailAddress)
        {
            return user != null && user.EmailAddress != emailAddress;
        }

        public (AdminUser?, List<DelegateUser>) GetVerifiedLinkedUsersAccounts(
            int? adminId,
            int? delegateId,
            string password
        )
        {
            var (loggedInAdminUser, loggedInDelegateUser) = GetUsersById(adminId, delegateId);

            var signedInEmailIfAny = loggedInAdminUser?.EmailAddress ?? loggedInDelegateUser?.EmailAddress;

            if (string.IsNullOrWhiteSpace(signedInEmailIfAny))
            {
                return loginService.VerifyUsers(
                    password,
                    loggedInAdminUser,
                    loggedInDelegateUser != null
                        ? new List<DelegateUser> { loggedInDelegateUser }
                        : new List<DelegateUser>()
                );
            }

            var (adminUser, delegateUsers) = GetUsersByEmailAddress(signedInEmailIfAny);

            return loginService.VerifyUsers(password, adminUser, delegateUsers);
        }
    }
}
