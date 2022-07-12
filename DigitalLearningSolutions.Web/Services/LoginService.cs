namespace DigitalLearningSolutions.Web.Services
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
            var (unverifiedAdminUser, unverifiedDelegateUsers) = userService.GetUsersByUsername(username);

            if (NoAccounts(unverifiedAdminUser, unverifiedDelegateUsers))
            {
                return new LoginResult(LoginAttemptResult.InvalidCredentials);
            }

            var (verifiedAdminUser, verifiedDelegateUsers) = userVerificationService.VerifyUsers(
                password,
                unverifiedAdminUser,
                unverifiedDelegateUsers
            );

            if (MultipleEmailsUsedAcrossAccounts(verifiedAdminUser, verifiedDelegateUsers))
            {
                throw new LoginWithMultipleEmailsException("Not all accounts have the same email");
            }

            var adminAccountVerificationAttemptedAndFailed = unverifiedAdminUser != null && verifiedAdminUser == null;
            var delegateAccountVerificationSuccessful = verifiedDelegateUsers.Any();
            var shouldIncreaseFailedLoginCount =
                adminAccountVerificationAttemptedAndFailed &&
                !delegateAccountVerificationSuccessful;

            var userEmail = delegateAccountVerificationSuccessful ? verifiedDelegateUsers[0].EmailAddress : null;
            var adminAccountAssociatedWithDelegateAccount =
                userEmail == null ? null : userService.GetAdminUserByEmailAddress(userEmail);

            var adminAccountIsAlreadyLocked = unverifiedAdminUser?.IsLocked == true ||
                                              adminAccountAssociatedWithDelegateAccount?.IsLocked == true;
            var adminAccountHasJustBecomeLocked = unverifiedAdminUser?.FailedLoginCount == 4 &&
                                                  shouldIncreaseFailedLoginCount;

            var adminAccountIsLocked = adminAccountIsAlreadyLocked || adminAccountHasJustBecomeLocked;

            if (shouldIncreaseFailedLoginCount)
            {
                userService.IncrementFailedLoginCount(unverifiedAdminUser!);
                unverifiedAdminUser!.FailedLoginCount += 1;
            }

            if (adminAccountIsLocked)
            {
                return new LoginResult(LoginAttemptResult.AccountLocked);
            }

            if (verifiedAdminUser == null && !delegateAccountVerificationSuccessful)
            {
                return new LoginResult(LoginAttemptResult.InvalidCredentials);
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

            var adminUserToLoginIfCentreActive = verifiedLinkedAdmin;
            if (adminUserToLoginIfCentreActive?.IsLocked == true)
            {
                adminUserToLoginIfCentreActive = null;
            }

            var delegateUsersToLogInIfCentreActive =
                approvedVerifiedDelegates.Concat(verifiedLinkedDelegates)
                    .GroupBy(du => du.Id)
                    .Select(g => g.First())
                    .ToList();

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
            var verifiedAssociatedAdmin =
                userVerificationService.GetActiveApprovedVerifiedAdminUserAssociatedWithDelegateUsers(
                    approvedVerifiedDelegates,
                    password
                );

            // If we find a new linked admin we must be logging in by CandidateNumber or AliasID.
            // In this case, we are trying to log directly into a centre so we discard an admin at a different centre.
            if (approvedVerifiedDelegates.All(du => du.CentreId != verifiedAssociatedAdmin?.CentreId))
            {
                verifiedAssociatedAdmin = null;
            }

            var verifiedLinkedAdmin = verifiedAdminUser ?? verifiedAssociatedAdmin;

            var verifiedLinkedDelegates =
                userVerificationService.GetActiveApprovedVerifiedDelegateUsersAssociatedWithAdminUser(
                    verifiedAdminUser,
                    password
                );
            return (verifiedLinkedAdmin, verifiedLinkedDelegates);
        }

        private static bool MultipleEmailsUsedAcrossAccounts(AdminUser? adminUser, List<DelegateUser> delegateUsers)
        {
            var emails = delegateUsers.Select(du => du.EmailAddress?.ToLowerInvariant())
                .ToList();

            if (adminUser != null)
            {
                emails.Add(adminUser.EmailAddress?.ToLowerInvariant());
            }

            var uniqueEmails = emails.Distinct().ToList();
            return uniqueEmails.Count > 1;
        }

        private static bool NoAccounts(AdminUser? adminUser, List<DelegateUser> delegateUsers)
        {
            return adminUser == null && delegateUsers.Count == 0;
        }
    }
}
