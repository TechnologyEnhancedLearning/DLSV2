namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;

    public interface ILoginService
    {
        LoginResult AttemptLogin(string username, string password);

        IEnumerable<ChooseACentreAccount> GetChooseACentreAccounts(UserEntity? userEntity);
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
                userEntity.UserAccount.FailedLoginCount += 1;
                userService.UpdateFailedLoginCount(userEntity.UserAccount);

                return userEntity.IsLocked
                    ? new LoginResult(LoginAttemptResult.AccountLocked, userEntity)
                    : new LoginResult(LoginAttemptResult.InvalidPassword);
            }

            return userEntity.IsLocked
                ? new LoginResult(LoginAttemptResult.AccountLocked, userEntity)
                : DetermineDestinationForSuccessfulLogin(userEntity, username);
        }

        public IEnumerable<ChooseACentreAccount> GetChooseACentreAccounts(UserEntity? userEntity)
        {
            return userEntity!.CentreAccounts.Values.Where(
                centreAccounts => centreAccounts.AdminAccount?.Active == true || centreAccounts.DelegateAccount != null
            ).Select(
                centreAccounts => new ChooseACentreAccount(
                    centreAccounts.CentreId,
                    centreAccounts.CentreName!,
                    centreAccounts.IsCentreActive,
                    centreAccounts.AdminAccount?.Active == true,
                    centreAccounts.DelegateAccount != null,
                    centreAccounts.DelegateAccount?.Approved ?? false,
                    centreAccounts.DelegateAccount?.Active ?? false
                )
            );
        }

        private LoginResult DetermineDestinationForSuccessfulLogin(UserEntity userEntity, string username)
        {
            userService.ResetFailedLoginCount(userEntity.UserAccount);

            var singleCentreToLogUserInto = GetCentreIdIfLoggingUserIntoSingleCentre(userEntity, username);
            if (singleCentreToLogUserInto == null)
            {
                return new LoginResult(LoginAttemptResult.ChooseACentre, userEntity);
            }

            var accountsToLogInto = userEntity.GetCentreAccounts(singleCentreToLogUserInto.Value);

            if (
                accountsToLogInto == null ||
                accountsToLogInto.IsCentreActive == false ||
                accountsToLogInto.IsActive == false ||
                accountsToLogInto.DelegateAccount?.Approved == false
            )
            {
                return new LoginResult(LoginAttemptResult.ChooseACentre, userEntity);
            }

            return new LoginResult(LoginAttemptResult.LogIntoSingleCentre, userEntity, singleCentreToLogUserInto.Value);
        }

        // If there are no accounts this will also return null, as there is no single centre to log into
        private static int? GetCentreIdIfLoggingUserIntoSingleCentre(UserEntity userEntity, string username)
        {
            // Determine if there is only a single account
            if (userEntity.IsSingleCentreAccount())
            {
                return userEntity.CentreAccounts.Count == 0 ? null as int? : userEntity.CentreAccounts.Keys.First();
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
