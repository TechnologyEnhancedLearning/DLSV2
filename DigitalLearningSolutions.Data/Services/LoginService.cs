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

        IEnumerable<ChooseACentreAccountViewModel> GetChooseACentreAccountViewModels(UserEntity? userEntity);
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

        public IEnumerable<ChooseACentreAccountViewModel> GetChooseACentreAccountViewModels(UserEntity? userEntity)
        {
            return userEntity!.CentreAccountSetsByCentreId.Values.Where(
                centreAccountSet => centreAccountSet.AdminAccount?.Active == true ||
                                    centreAccountSet.DelegateAccount != null
            ).Select(
                centreAccountSet => new ChooseACentreAccountViewModel(
                    centreAccountSet.CentreId,
                    centreAccountSet.CentreName,
                    centreAccountSet.IsCentreActive,
                    centreAccountSet.AdminAccount?.Active == true,
                    centreAccountSet.DelegateAccount != null,
                    centreAccountSet.DelegateAccount?.Approved ?? false,
                    centreAccountSet.DelegateAccount?.Active ?? false
                )
            );
        }

        private LoginResult DetermineDestinationForSuccessfulLogin(UserEntity userEntity, string username)
        {
            userService.ResetFailedLoginCount(userEntity.UserAccount);

            var singleCentreToLogUserInto = GetCentreIdIfLoggingUserIntoSingleCentre(userEntity, username);

            return singleCentreToLogUserInto == null
                ? new LoginResult(LoginAttemptResult.ChooseACentre, userEntity)
                : new LoginResult(LoginAttemptResult.LogIntoSingleCentre, userEntity, singleCentreToLogUserInto);
        }

        // If there are no accounts this will also return null, as there is no single centre to log into
        private static int? GetCentreIdIfLoggingUserIntoSingleCentre(UserEntity userEntity, string username)
        {
            // Determine if there is only a single account
            if (userEntity.IsSingleCentreAccount)
            {
                var accountsToLogInto = userEntity.CentreAccountSetsByCentreId.Values.Single();

                return accountsToLogInto.CanLogInToCentre ? accountsToLogInto.CentreId : null as int?;
            }

            // Determine if we are logging in via candidate number.
            var delegateAccountToLogIntoIfCandidateNumberUsed = userEntity.DelegateAccounts.SingleOrDefault(
                da =>
                    string.Equals(da.CandidateNumber, username, StringComparison.CurrentCultureIgnoreCase)
            );

            if (delegateAccountToLogIntoIfCandidateNumberUsed == null)
            {
                return null;
            }

            var canLogIntoToAccount = userEntity
                .CentreAccountSetsByCentreId[delegateAccountToLogIntoIfCandidateNumberUsed.CentreId].CanLogInToCentre;

            return canLogIntoToAccount ? delegateAccountToLogIntoIfCandidateNumberUsed.CentreId : null as int?;
        }
    }
}
