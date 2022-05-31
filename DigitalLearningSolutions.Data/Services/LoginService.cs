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
            var adminAccounts = userEntity!.AdminAccounts.Where(aa => aa.Active);

            var chooseACentreAccounts = userEntity.DelegateAccounts.Select(
                account =>
                {
                    var isAdmin = adminAccounts.Any(aa => aa.CentreId == account.CentreId);

                    return new ChooseACentreAccount(
                        account.CentreId,
                        account.CentreName,
                        account.CentreActive,
                        isAdmin,
                        true,
                        account.Approved,
                        account.Active
                    );
                }
            ).ToList();

            var adminOnlyAccounts = adminAccounts.Where(
                aa => chooseACentreAccounts.All(account => account.CentreId != aa.CentreId)
            );

            chooseACentreAccounts.AddRange(
                adminOnlyAccounts.Select(
                    account => new ChooseACentreAccount(
                        account.CentreId,
                        account.CentreName,
                        account.CentreActive,
                        true
                    )
                )
            );

            return chooseACentreAccounts;
        }

        private LoginResult DetermineDestinationForSuccessfulLogin(UserEntity userEntity, string username)
        {
            userService.ResetFailedLoginCount(userEntity.UserAccount);

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
            var accountAtCentreIsActive = (adminAccountToLogInto?.Active == null || adminAccountToLogInto.Active) &&
                                          (delegateAccountToLogInto?.Active == null || delegateAccountToLogInto.Active);

            if (!centreIsActive || !accountAtCentreIsActive || delegateAccountToLogInto is { Approved: false })
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
