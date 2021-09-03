namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;

    public interface ILoginService
    {
        LoginResult AttemptLogin(string username, string password);

        UserAccountSet VerifyUsers(
            string password,
            List<AdminUser> unverifiedAdminUsers,
            List<DelegateUser> unverifiedDelegateUsers
        );

        UserAccountSet GetVerifiedLinkedUsersAccounts(
            int? adminId,
            int? delegateId,
            string password
        );
    }

    public class LoginService : ILoginService
    {
        private readonly ICryptoService cryptoService;
        private readonly IUserDataService userDataService;
        private readonly IUserService userService;

        public LoginService(IUserService userService, IUserDataService userDataService, ICryptoService cryptoService)
        {
            this.userService = userService;
            this.userDataService = userDataService;
            this.cryptoService = cryptoService;
        }

        public LoginResult AttemptLogin(string username, string password)
        {
            var (unverifiedAdminUsers, unverifiedDelegateUsers) = userService.GetUsersByUsername(username);

            if (NoAccounts(unverifiedAdminUsers, unverifiedDelegateUsers))
            {
                return new LoginResult(LoginAttemptResult.InvalidUsername);
            }

            var (verifiedAdminUsers, verifiedDelegateUsers) = VerifyUsers(
                password,
                unverifiedAdminUsers,
                unverifiedDelegateUsers
            );

            var verifiedAdminUser = verifiedAdminUsers.SingleOrDefault();

            if (!EmailIsTheSameOnAllAccounts(verifiedAdminUser, verifiedDelegateUsers))
            {
                throw new Exception("Not all accounts have the same email");
            }

            var adminAccountVerificationAttemptedAndFailed = unverifiedAdminUsers.Any() && verifiedAdminUser == null;
            var doWeKnowWhichAdminAccountToRecordFailureAgainst = unverifiedAdminUsers.Count == 1;

            var adminAccountIsAlreadyLocked = doWeKnowWhichAdminAccountToRecordFailureAgainst &&
                                              unverifiedAdminUsers.Single().IsLocked;
            var adminAccountHasJustBecomeLocked = doWeKnowWhichAdminAccountToRecordFailureAgainst &&
                                                  unverifiedAdminUsers.Single().FailedLoginCount == 4 &&
                                                  adminAccountVerificationAttemptedAndFailed;

            var adminAccountIsLocked = adminAccountIsAlreadyLocked || adminAccountHasJustBecomeLocked;
            var delegateAccountVerificationSuccessful = verifiedDelegateUsers.Any();
            var shouldIncreaseFailedLoginCount =
                doWeKnowWhichAdminAccountToRecordFailureAgainst && adminAccountVerificationAttemptedAndFailed &&
                !delegateAccountVerificationSuccessful;

            if (shouldIncreaseFailedLoginCount)
            {
                var adminUser = unverifiedAdminUsers.Single();
                userService.IncrementFailedLoginCount(adminUser);
            }

            if (adminAccountIsLocked)
            {
                if (delegateAccountVerificationSuccessful)
                {
                    verifiedAdminUser = null;
                }
                else
                {
                    return new LoginResult(LoginAttemptResult.AccountLocked, unverifiedAdminUsers.Single());
                }
            }

            if (verifiedAdminUser == null && !delegateAccountVerificationSuccessful)
            {
                return new LoginResult(LoginAttemptResult.InvalidPassword);
            }

            if (verifiedAdminUser != null)
            {
                userService.ResetFailedLoginCount(verifiedAdminUser);
            }

            var approvedVerifiedDelegates = verifiedDelegateUsers.Where(du => du.Approved).ToList();
            if (verifiedAdminUser == null && !approvedVerifiedDelegates.Any())
            {
                return new LoginResult(LoginAttemptResult.AccountNotApproved);
            }

            var delegateWithEmail = approvedVerifiedDelegates.FirstOrDefault(du => du.EmailAddress != null);

            var verifiedLinkedAdmin = verifiedAdminUser ?? GetVerifiedAdminUserByEmail(password, delegateWithEmail);
            var verifiedLinkedDelegates = GetVerifiedDelegateUsersAssociatedWithAdminUser(verifiedAdminUser, password);

            var adminUserToLoginIfCentreActive = new[] { verifiedAdminUser, verifiedLinkedAdmin }
                .Where(au => au != null).Distinct().SingleOrDefault();
            if (adminUserToLoginIfCentreActive?.IsLocked == true)
            {
                adminUserToLoginIfCentreActive = null;
            }

            var delegateUsersToLogInIfCentreActive =
                approvedVerifiedDelegates.Concat(verifiedLinkedDelegates).Distinct().ToList();

            var (adminUserToLogIn, delegateUsersToLogIn) = userService.GetUsersWithActiveCentres(
                adminUserToLoginIfCentreActive,
                delegateUsersToLogInIfCentreActive
            );
            var availableCentres = userService.GetUserCentres(adminUserToLogIn, delegateUsersToLogIn);

            switch (availableCentres.Count)
            {
                case 0:
                    return new LoginResult(LoginAttemptResult.InactiveCentre);
                case 1:
                    return new LoginResult(
                        LoginAttemptResult.LogIntoSingleCentre,
                        adminUserToLogIn,
                        delegateUsersToLogIn
                    );
                default:
                    return new LoginResult(
                        LoginAttemptResult.ChooseACentre,
                        adminUserToLogIn,
                        delegateUsersToLogIn,
                        availableCentres
                    );
            }
        }

        public UserAccountSet VerifyUsers(
            string password,
            List<AdminUser> unverifiedAdminUsers,
            List<DelegateUser> unverifiedDelegateUsers
        )
        {
            var verifiedAdminUsers =
                unverifiedAdminUsers.Where(au => cryptoService.VerifyHashedPassword(au.Password, password))
                    .ToList();
            var verifiedDelegateUsers =
                unverifiedDelegateUsers.Where(du => cryptoService.VerifyHashedPassword(du.Password, password))
                    .ToList();

            return new UserAccountSet(verifiedAdminUsers, verifiedDelegateUsers);
        }

        public UserAccountSet GetVerifiedLinkedUsersAccounts(
            int? adminId,
            int? delegateId,
            string password
        )
        {
            var (loggedInAdminUser, loggedInDelegateUser) = userService.GetUsersById(adminId, delegateId);

            var signedInEmailIfAny = loggedInAdminUser?.EmailAddress ?? loggedInDelegateUser?.EmailAddress;

            if (string.IsNullOrWhiteSpace(signedInEmailIfAny))
            {
                var loggedInAdminUsers = loggedInAdminUser != null
                    ? new List<AdminUser> { loggedInAdminUser }
                    : new List<AdminUser>();

                var loggedInDelegateUsers = loggedInDelegateUser != null
                    ? new List<DelegateUser> { loggedInDelegateUser }
                    : new List<DelegateUser>();

                return VerifyUsers(password, loggedInAdminUsers, loggedInDelegateUsers);
            }

            var (adminUser, delegateUsers) = userService.GetUsersByEmailAddress(signedInEmailIfAny);

            var adminUsers = adminUser != null
                ? new List<AdminUser> { adminUser }
                : new List<AdminUser>();

            var temp = VerifyUsers(password, adminUsers, delegateUsers);

            return temp;
        }

        public AdminUser? GetVerifiedAdminUserAssociatedWithDelegateUsers(
            List<DelegateUser> delegateUsers,
            string password
        )
        {
            var adminUsers = new List<AdminUser?>();

            foreach (var delegateUser in delegateUsers)
            {
                var admin = GetVerifiedAdminUserByEmail(password, delegateUser);
                if (admin != null)
                {
                    adminUsers.Add(admin);
                }
            }

            return adminUsers.Distinct().SingleOrDefault();
        }

        private bool EmailIsTheSameOnAllAccounts(AdminUser? adminUser, List<DelegateUser> delegateUsers)
        {
            if (adminUser == null && delegateUsers.Count == 0)
            {
                return true;
            }

            var adminEmail = adminUser?.EmailAddress;
            var delegateEmails = delegateUsers.Where(du => du.EmailAddress != null).Select(du => du.EmailAddress)
                .ToList();
            if (adminEmail != null)
            {
                delegateEmails.Add(adminEmail);
            }

            var uniqueEmails = delegateEmails.Distinct().ToList();
            return uniqueEmails.Count == 1;
        }

        private List<DelegateUser> GetVerifiedDelegateUsersAssociatedWithAdminUser(
            AdminUser? adminUser,
            string password
        )
        {
            if (string.IsNullOrEmpty(adminUser?.EmailAddress))
            {
                return new List<DelegateUser>();
            }

            var delegatesAssociatedWithAdmin = userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!);

            var suitableDelegates = delegatesAssociatedWithAdmin
                .Where(du => du.Active)
                .Where(du => du.Approved)
                .Where(du => du.CentreId == adminUser.CentreId)
                .Where(du => cryptoService.VerifyHashedPassword(du.Password, password));

            return suitableDelegates.ToList();
        }

        private bool NoAccounts(List<AdminUser> adminUsers, List<DelegateUser> delegateUsers)
        {
            return adminUsers.Count == 0 && delegateUsers.Count == 0;
        }

        private AdminUser? GetVerifiedAdminUserByEmail(string password, DelegateUser? delegateUser)
        {
            if (string.IsNullOrWhiteSpace(delegateUser?.EmailAddress))
            {
                return null;
            }

            var adminUserAssociatedWithDelegate = userDataService.GetAdminUserByEmailAddress(delegateUser.EmailAddress);

            var isSuitableAdmin = adminUserAssociatedWithDelegate?.CentreId == delegateUser.CentreId &&
                                  cryptoService.VerifyHashedPassword(
                                      adminUserAssociatedWithDelegate.Password,
                                      password
                                  );
            return isSuitableAdmin ? adminUserAssociatedWithDelegate : null;
        }
    }
}
