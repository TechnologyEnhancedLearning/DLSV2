namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.ViewModels;

    public interface ILoginService
    {
        LoginResult AttemptLogin(string username, string password);

        IEnumerable<ChooseACentreAccountViewModel> GetChooseACentreAccountViewModels(
            UserEntity? userEntity,
            List<int> idsOfCentresWithUnverifiedEmails
        );

        bool CentreEmailIsVerified(int userId, int centreIdIfLoggingIntoSingleCentre);
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
                return new LoginResult(LoginAttemptResult.InvalidCredentials);
            }

            //if (userEntity.DelegateAccounts.Any(da => da.IsYetToBeClaimed))
            //{
            //    return new LoginResult(LoginAttemptResult.UnclaimedDelegateAccount);
            //}

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
                    ? new LoginResult(LoginAttemptResult.AccountLocked)
                    : new LoginResult(LoginAttemptResult.InvalidCredentials);
            }

            if (userEntity.IsLocked)
            {
                return new LoginResult(LoginAttemptResult.AccountLocked);
            }

            if (!userEntity.UserAccount.Active)
            {
                return new LoginResult(LoginAttemptResult.InactiveAccount);
            }

            userService.ResetFailedLoginCount(userEntity.UserAccount);

            if (userEntity.UserAccount.EmailVerified == null)
            {
                return new LoginResult(
                    LoginAttemptResult.UnverifiedEmail,
                    userEntity
                );
            }

            var centreIdIfLoggingIntoSingleCentre = GetCentreIdIfLoggingUserIntoSingleCentre(userEntity, username);

            if (centreIdIfLoggingIntoSingleCentre == null)
            {
                return new LoginResult(LoginAttemptResult.ChooseACentre, userEntity);
            }

            if (!CentreEmailIsVerified(
                userEntity.UserAccount.Id,
                (int)centreIdIfLoggingIntoSingleCentre
            ))
            {
                return new LoginResult(
                    LoginAttemptResult.UnverifiedEmail,
                    userEntity,
                    centreIdIfLoggingIntoSingleCentre
                );
            }

            return new LoginResult(
                LoginAttemptResult.LogIntoSingleCentre,
                userEntity,
                centreIdIfLoggingIntoSingleCentre
            );

        }

        public IEnumerable<ChooseACentreAccountViewModel> GetChooseACentreAccountViewModels(
            UserEntity? userEntity,
            List<int> idsOfCentresWithUnverifiedEmails
        )
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
                    centreAccountSet.DelegateAccount?.Active ?? false,
                    idsOfCentresWithUnverifiedEmails.Contains(centreAccountSet.CentreId)
                )
            );
        }

        public bool CentreEmailIsVerified(int userId, int centreIdIfLoggingIntoSingleCentre)
        {
            var (_, unverifiedCentreEmails) = userService.GetUnverifiedEmailsForUser(userId);
            return unverifiedCentreEmails.Select(uce => uce.centreId)
                .Contains(centreIdIfLoggingIntoSingleCentre) == false;
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
