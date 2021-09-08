namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;

    public interface ILoginService
    {
        LoginResult AttemptLogin(string username, string password);
    }

    public class LoginService : ILoginService
    {
        private readonly IUserService userService;
        private readonly IUserVerificationService userVerificationService;

        public LoginService(IUserService userService, IUserVerificationService userVerificationService)
        {
            this.userService = userService;
            this.userVerificationService = userVerificationService;
        }

        public LoginResult AttemptLogin(string username, string password)
        {
            var (unverifiedAdminUsers, unverifiedDelegateUsers) = userService.GetUsersByUsername(username);

            if (NoAccounts(unverifiedAdminUsers, unverifiedDelegateUsers))
            {
                return new LoginResult(LoginAttemptResult.InvalidUsername);
            }

            var (verifiedAdminUsers, verifiedDelegateUsers) = userVerificationService.VerifyUsers(
                password,
                unverifiedAdminUsers,
                unverifiedDelegateUsers
            );

            var verifiedAdminUser = verifiedAdminUsers.SingleOrDefault();

            if (MultipleEmailsUsedAcrossAccounts(verifiedAdminUser, verifiedDelegateUsers))
            {
                throw new LoginWithMultipleEmailsException("Not all accounts have the same email");
            }

            var adminAccountVerificationAttemptedAndFailed = unverifiedAdminUsers.Any() && verifiedAdminUser == null;
            var adminAccountToRecordFailureAgainstIsKnown = unverifiedAdminUsers.Count == 1;

            var adminAccountIsAlreadyLocked = adminAccountToRecordFailureAgainstIsKnown &&
                                              unverifiedAdminUsers.Single().IsLocked;
            var adminAccountHasJustBecomeLocked = adminAccountToRecordFailureAgainstIsKnown &&
                                                  unverifiedAdminUsers.Single().FailedLoginCount == 4 &&
                                                  adminAccountVerificationAttemptedAndFailed;

            var adminAccountIsLocked = adminAccountIsAlreadyLocked || adminAccountHasJustBecomeLocked;
            var delegateAccountVerificationSuccessful = verifiedDelegateUsers.Any();
            var shouldIncreaseFailedLoginCount =
                adminAccountToRecordFailureAgainstIsKnown && adminAccountVerificationAttemptedAndFailed &&
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

            var (verifiedLinkedAdmin, verifiedLinkedDelegates) = GetVerifiedLinkedAccounts(
                password,
                approvedVerifiedDelegates,
                verifiedAdminUser
            );

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

            return availableCentres.Count switch
            {
                0 => new LoginResult(LoginAttemptResult.InactiveCentre),
                1 => new LoginResult(
                    LoginAttemptResult.LogIntoSingleCentre,
                    adminUserToLogIn,
                    delegateUsersToLogIn
                ),
                _ => new LoginResult(
                    LoginAttemptResult.ChooseACentre,
                    adminUserToLogIn,
                    delegateUsersToLogIn,
                    availableCentres
                )
            };
        }

        private (AdminUser? verifiedLinkedAdmin, List<DelegateUser> verifiedLinkedDelegates) GetVerifiedLinkedAccounts(
            string password,
            List<DelegateUser> approvedVerifiedDelegates,
            AdminUser? verifiedAdminUser
        )
        {
            var verifiedLinkedAdmin = verifiedAdminUser ??
                                      userVerificationService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                                          approvedVerifiedDelegates,
                                          password
                                      );
            var verifiedLinkedDelegates =
                userVerificationService.GetVerifiedDelegateUsersAssociatedWithAdminUser(verifiedAdminUser, password);
            return (verifiedLinkedAdmin, verifiedLinkedDelegates);
        }

        private static bool MultipleEmailsUsedAcrossAccounts(AdminUser? adminUser, List<DelegateUser> delegateUsers)
        {
            var emails = delegateUsers.Select(du => du.EmailAddress)
                .ToList();

            if (adminUser != null)
            {
                emails.Add(adminUser.EmailAddress);
            }

            var uniqueEmails = emails.Distinct().ToList();
            return uniqueEmails.Count > 1;
        }

        private static bool NoAccounts(List<AdminUser> adminUsers, List<DelegateUser> delegateUsers)
        {
            return adminUsers.Count == 0 && delegateUsers.Count == 0;
        }
    }
}
