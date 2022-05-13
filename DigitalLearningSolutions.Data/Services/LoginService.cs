namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
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
            // TODO: Add DEPRECATED TAG to old user models that are still used elsewhere?
            var userEntity = userService.GetUserByUsername(username);

            if (userEntity == null)
            {
                return new LoginResult(LoginAttemptResult.InvalidUsername);
            }

            var verificationResult = userVerificationService.VerifyUserEntity(password, userEntity);
            var singleCentreToLogUserInto = GetCentreIdIfLoggingUserIntoSingleCentre(userEntity, username);

            if (!verificationResult.PasswordMatchesAtLeastOneAccountPassword)
            {
                if (userEntity.AdminAccounts.Any())
                {
                    userEntity.UserAccount.FailedLoginCount += 1;
                    userService.UpdateFailedLoginCount(userEntity.UserAccount);
                }

                // TODO: if we have multiple accounts, and at least one delegate only account exists at a centre without any admins, we might show them the Choose a centre page instead?
                return userEntity.IsLocked ? new LoginResult(LoginAttemptResult.AccountLocked, userEntity) : new LoginResult(LoginAttemptResult.InvalidPassword);
            }

            if (verificationResult.PasswordMatchesAtLeastOneAccountPassword && !verificationResult.PasswordMatchesAllAccountPasswords)
            {
                return new LoginResult(LoginAttemptResult.AccountsHaveMismatchedPasswords);
            }

            if (userEntity.IsLocked)
            {
                return new LoginResult(LoginAttemptResult.AccountLocked, userEntity);
            }

            // TODO: do we always want to do this? Need to make sure we aren't unlocking admin accounts when logging in via single
            userService.ResetFailedLoginCount(userEntity.UserAccount);

            if (singleCentreToLogUserInto == null)
            {
                var availableCentres = userService.GetUserCentres(userEntity);
                return new LoginResult(LoginAttemptResult.ChooseACentre, userEntity, availableCentres);
            }

            // TODO: Need to change this so that we can get redirected into ChooseACentre if other accounts are available.
            var userEntityWithSingleAccount = LoginHelper.FilterUserEntityForLoggingIntoSingleCentre(userEntity, singleCentreToLogUserInto.Value);

            var adminAccountToLogInto = userEntityWithSingleAccount.AdminAccounts.SingleOrDefault();
            var delegateAccountToLogInto = userEntityWithSingleAccount.DelegateAccounts.SingleOrDefault();

            // TODO These are set to false if all are null. In reality, these should never be null as there should always be at least one
            var centreIsActive = adminAccountToLogInto?.CentreActive ?? delegateAccountToLogInto?.CentreActive ?? false;
            if (!centreIsActive)
            {
                return new LoginResult(LoginAttemptResult.InactiveCentre);
            }

            var showInactiveAccountPage = FilterSingleCentreUserEntityIfAccountsAreInactive(ref userEntityWithSingleAccount);
            if (showInactiveAccountPage)
            {
                return new LoginResult(LoginAttemptResult.InactiveAccount);
            }

            // Refresh delegateAccountToLogInto as it may have been removed if inactive
            delegateAccountToLogInto = userEntityWithSingleAccount.DelegateAccounts.SingleOrDefault();
            // TODO: do we want to prevent log in of admin account if not approved?
            if (delegateAccountToLogInto is { Approved: false })
            {
                return new LoginResult(LoginAttemptResult.AccountNotApproved);
            }

            return new LoginResult(LoginAttemptResult.LogIntoSingleCentre, userEntityWithSingleAccount);
        }

        private int? GetCentreIdIfLoggingUserIntoSingleCentre(UserEntity userEntity, string username)
        {
            // Determine if there is only a single account
            var userHasOneAdminAccountAndDelegateAccountOrFewer =
                userEntity.AdminAccounts.Count() <= 1 && userEntity.DelegateAccounts.Count() <= 1;
            if (userHasOneAdminAccountAndDelegateAccountOrFewer)
            {
                var adminCentreId = userEntity.AdminAccounts.SingleOrDefault()?.CentreId;
                var delegateCentreId = userEntity.DelegateAccounts.SingleOrDefault()?.CentreId;
                if (adminCentreId == delegateCentreId)
                {
                    return adminCentreId ?? delegateCentreId;
                }
            }

            // Determine if we are logging in via candidate number.
            var delegateAccountToLogIntoIfCandidateNumberUsed = userEntity.DelegateAccounts.SingleOrDefault(da =>
                string.Equals(da.CandidateNumber, username, StringComparison.CurrentCultureIgnoreCase));
            if (delegateAccountToLogIntoIfCandidateNumberUsed == null)
            {
                return null;
            }

            return delegateAccountToLogIntoIfCandidateNumberUsed.CentreId;
        }

        /// <summary>
        /// Determines which accounts are inactive, and whether we should take the user to the
        /// InactiveAccount page. Will remove the AdminAccount from the UserEntity if it is inactive
        /// and the delegate account is not.
        /// </summary>
        /// <returns>True if the user should be shown the InactiveAccount page. False if we should log the user in</returns>
        private bool FilterSingleCentreUserEntityIfAccountsAreInactive(ref UserEntity userEntity)
        {
            var adminAccountIsActive = userEntity.AdminAccounts.SingleOrDefault()?.Active;
            var delegateAccountIsActive = userEntity.DelegateAccounts.SingleOrDefault()?.Active;

            if (adminAccountIsActive == null)
            {
                return !delegateAccountIsActive!.Value;
            }

            if (delegateAccountIsActive == null)
            {
                return !adminAccountIsActive!.Value;
            }

            // TODO: If one account is inactive, and the other active, we log the user in on the active account only.
            if (!adminAccountIsActive.Value && delegateAccountIsActive.Value)
            {
                userEntity.AdminAccounts = new List<AdminAccount>();
                return false;
            }

            if (adminAccountIsActive.Value && !delegateAccountIsActive.Value)
            {
                userEntity.DelegateAccounts = new List<DelegateAccount>();
                return false;
            }

            return !(adminAccountIsActive.Value && delegateAccountIsActive.Value);
        }
    }
}
