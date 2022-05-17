namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
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
            var userEntity = userService.GetUserByUsername(username);

            if (userEntity == null)
            {
                return new LoginResult(LoginAttemptResult.InvalidUsername);
            }

            if (!userEntity.UserAccount.Active)
            {
                return new LoginResult(LoginAttemptResult.InactiveAccount);
            }

            var verificationResult = userVerificationService.VerifyUserEntity(password, userEntity);

            if (verificationResult.PasswordMatchesAtLeastOneAccountPassword &&
                !verificationResult.PasswordMatchesAllAccountPasswords)
            {
                return new LoginResult(LoginAttemptResult.AccountsHaveMismatchedPasswords);
            }

            if (!verificationResult.PasswordMatchesAtLeastOneAccountPassword)
            {
                userService.UpdateFailedLoginCount(userEntity.UserAccount);

                return new LoginResult(LoginAttemptResult.InvalidPassword);
            }
            
            if (!userEntity.AdminAccountsLocked)
            {
                userService.ResetFailedLoginCount(userEntity.UserAccount);
            }

            var singleCentreToLogUserInto = GetCentreIdIfLoggingUserIntoSingleCentre(userEntity, username);
            if (singleCentreToLogUserInto == null)
            {
                return new LoginResult(LoginAttemptResult.ChooseACentre, userEntity);
            }

            var adminAccountToLogInto =
                userEntity.AdminAccounts.SingleOrDefault(aa => aa.CentreId == singleCentreToLogUserInto.Value);
            var delegateAccountToLogInto =
                userEntity.DelegateAccounts.SingleOrDefault(da => da.CentreId == singleCentreToLogUserInto.Value);

            var centreIsActive = adminAccountToLogInto?.CentreActive ?? delegateAccountToLogInto?.CentreActive ?? false;
            var accountAtCentreIsActive = adminAccountToLogInto?.Active ?? delegateAccountToLogInto?.Active ?? false;
            var adminAccountIsAtSingleCentreAndIsLocked =
                userEntity.AdminAccountsLocked && adminAccountToLogInto != null;
            if (!centreIsActive || !accountAtCentreIsActive || delegateAccountToLogInto is { Approved: false } ||
                adminAccountIsAtSingleCentreAndIsLocked)
            {
                return new LoginResult(LoginAttemptResult.ChooseACentre, userEntity);
            }

            return new LoginResult(LoginAttemptResult.LogIntoSingleCentre, userEntity, singleCentreToLogUserInto.Value);
        }

        private static int? GetCentreIdIfLoggingUserIntoSingleCentre(UserEntity userEntity, string username)
        {
            // Determine if there is only a single account
            if (userEntity.IsSingleCentreAccount())
            {
                var adminCentreId = userEntity.AdminAccounts.SingleOrDefault()?.CentreId;
                var delegateCentreId = userEntity.DelegateAccounts.SingleOrDefault()?.CentreId;
                return adminCentreId ?? delegateCentreId;
            }

            // Determine if we are logging in via candidate number.
            var delegateAccountToLogIntoIfCandidateNumberUsed = userEntity.DelegateAccounts.SingleOrDefault(
                da =>
                    string.Equals(da.CandidateNumber, username, StringComparison.CurrentCultureIgnoreCase)
            );

            return delegateAccountToLogIntoIfCandidateNumberUsed?.CentreId;
        }
    }
}
