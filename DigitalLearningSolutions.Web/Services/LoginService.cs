namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Data.ViewModels;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authentication;

    public interface ILoginService
    {
        LoginResult AttemptLogin(string username, string password);

        LoginResult AttemptLoginUserEntity(
            UserEntity userEntity,
            string username);

        IEnumerable<ChooseACentreAccountViewModel> GetChooseACentreAccountViewModels(
            UserEntity? userEntity,
            List<int> idsOfCentresWithUnverifiedEmails
        );

        bool CentreEmailIsVerified(int userId, int centreIdIfLoggingIntoSingleCentre);

        void UpdateLastAccessedForUsersTable(int Id);

        Task<string> HandleLoginResult(
            LoginResult loginResult,
            TicketReceivedContext context,
            string returnUrl,
            ISessionService sessionService,
            IUserService userService,
            string appRootPath);
    }

    public class LoginService : ILoginService
    {
        private readonly IUserService userService;
        private readonly IUserVerificationService userVerificationService;
        private readonly ILoginDataService loginDataService;

        public LoginService(IUserService userService, IUserVerificationService userVerificationService, ILoginDataService loginDataService)
        {
            this.userService = userService;
            this.userVerificationService = userVerificationService;
            this.loginDataService = loginDataService;
        }

        public void UpdateLastAccessedForUsersTable(int Id)
        {
           loginDataService.UpdateLastAccessedForUsersTable(Id);
        }

        public LoginResult AttemptLogin(string username, string password)
        {
            var userEntity = userService.GetUserByUsername(username);

            if (userEntity == null)
            {
                return new LoginResult(LoginAttemptResult.InvalidCredentials);
            }

            if (userEntity.DelegateAccounts.Any(da => da.IsYetToBeClaimed))
            {
                return new LoginResult(LoginAttemptResult.UnclaimedDelegateAccount);
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

                return userEntity.UserAccount.FailedLoginCount >= AuthHelper.FailedLoginThreshold
                    ? new LoginResult(LoginAttemptResult.AccountLocked)
                    : new LoginResult(LoginAttemptResult.InvalidCredentials);
            }

            return this.AttemptLoginUserEntity(
                userEntity,
                username);
        }

        public LoginResult AttemptLoginUserEntity(
            UserEntity userEntity,
            string username)
        {
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

            var centreIdIfLoggingIntoSingleCentre = GetCentreIdIfLoggingUserIntoSingleCentre(
                userEntity,
                username);

            if (centreIdIfLoggingIntoSingleCentre == null)
            {
                return new LoginResult(
                    LoginAttemptResult.ChooseACentre,
                    userEntity);
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

        public async Task<string> HandleLoginResult(
            LoginResult loginResult,
            TicketReceivedContext context,
            string returnUrl,
            ISessionService sessionService,
            IUserService userService,
            string appRootPath)
        {
            switch (loginResult.LoginAttemptResult)
            {
                case LoginAttemptResult.AccountLocked:
                    return appRootPath + "/login/AccountLocked";
                case LoginAttemptResult.InactiveAccount:
                    return appRootPath + "/login/AccountInactive";
                case LoginAttemptResult.UnverifiedEmail:
                    await LoginHelper.CentrelessLogInAsync(
                        context,
                        loginResult.UserEntity!.UserAccount,
                        false);
                    return appRootPath + "/VerifyYourEmail/" + EmailVerificationReason.EmailNotVerified;
                case LoginAttemptResult.LogIntoSingleCentre:
                    var singleCentreClaims = LoginClaimsHelper.GetClaimsForSignIntoCentre(
                           loginResult.UserEntity,
                           loginResult.CentreToLogInto!.Value);
                    var singleCentreClaimsIdentity = (ClaimsIdentity)context.Principal.Identity;
                    singleCentreClaimsIdentity.AddClaims(singleCentreClaims);
                    return await LoginHelper.LogIntoCentreAsync(
                        loginResult.UserEntity,
                        false,
                        returnUrl,
                        loginResult.CentreToLogInto!.Value,
                        context,
                        sessionService,
                        userService);
                case LoginAttemptResult.ChooseACentre:
                    var idsOfCentresWithUnverifiedEmails = userService.GetUnverifiedEmailsForUser(
                            loginResult
                            .UserEntity!
                            .UserAccount
                            .Id)
                        .centreEmails
                        .Select(uce => uce.centreId)
                        .ToList();
                    var activeCentres = loginResult.UserEntity!.CentreAccountSetsByCentreId.Values.Where(
                                            centreAccountSet => (centreAccountSet.AdminAccount?.Active == true ||
                                            centreAccountSet.DelegateAccount != null) &&
                                            centreAccountSet.IsCentreActive == true &&
                                            centreAccountSet.DelegateAccount?.Active == true &&
                                            centreAccountSet.DelegateAccount?.Approved == true &&
                                            !idsOfCentresWithUnverifiedEmails.Contains(centreAccountSet.CentreId)).ToList();

                    if (activeCentres.Count() == 1)
                    {
                        var chooseCentreClaims = LoginClaimsHelper.GetClaimsForSignIntoCentre(
                            loginResult.UserEntity,
                            activeCentres[0].CentreId);
                        var chooseCentreClaimsIdentity = (ClaimsIdentity)context.Principal.Identity;
                        chooseCentreClaimsIdentity.AddClaims(chooseCentreClaims);

                        return await LoginHelper.LogIntoCentreAsync(
                        loginResult.UserEntity,
                        false,
                        returnUrl,
                        activeCentres[0].CentreId,
                        context,
                        sessionService,
                        userService);
                    }

                    await LoginHelper.CentrelessLogInAsync(
                        context,
                        loginResult.UserEntity!.UserAccount,
                        false);
                    return appRootPath + "/Login/ChooseACentre";
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
