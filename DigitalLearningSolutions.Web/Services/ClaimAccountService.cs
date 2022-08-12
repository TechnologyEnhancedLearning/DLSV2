namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using DigitalLearningSolutions.Web.ViewModels.Register.ClaimAccount;

    public interface IClaimAccountService
    {
        ClaimAccountViewModel GetAccountDetailsForClaimAccount(
            int userIdForRegistration,
            int centreId,
            string centreName,
            string email,
            int? loggedInUserId = null
        );

        Task ConvertTemporaryUserToConfirmedUser(int userId, int centreId, string primaryEmail, string? password);

        void LinkAccount(int oldUserId, int newUserId, int centreId);
    }

    public class ClaimAccountService : IClaimAccountService
    {
        private readonly IUserDataService userDataService;
        private readonly IConfigDataService configDataService;
        private readonly IPasswordService passwordService;

        public ClaimAccountService(
            IUserDataService userDataService,
            IConfigDataService configDataService,
            IPasswordService passwordService
        )
        {
            this.userDataService = userDataService;
            this.configDataService = configDataService;
            this.passwordService = passwordService;
        }

        public ClaimAccountViewModel GetAccountDetailsForClaimAccount(
            int userIdForRegistration,
            int centreId,
            string centreName,
            string email,
            int? loggedInUserId = null
        )
        {
            var userMatchingEmail = userDataService.GetUserAccountByPrimaryEmail(email);
            var userAccountToBeClaimed = userDataService.GetUserAccountById(userIdForRegistration);
            var delegateAccounts = userDataService.GetDelegateAccountsByUserId(userIdForRegistration).ToList();
            var adminAccounts = userDataService.GetAdminAccountsByUserId(userIdForRegistration).ToList();

            if (
                delegateAccounts.Count != 1 ||
                adminAccounts.Count != 0 ||
                delegateAccounts.Any(da => da.CentreId != centreId)
            )
            {
                throw new Exception(
                    "Expected user account being claimed to only have one delegate account at the correct centre"
                );
            }

            var delegateAccountToBeClaimed = delegateAccounts.First();

            var supportEmail = configDataService.GetConfigValue(ConfigDataService.SupportEmail);

            return new ClaimAccountViewModel
            {
                UserId = userIdForRegistration,
                CentreId = centreId,
                CentreName = centreName,
                Email = email,
                CandidateNumber = delegateAccountToBeClaimed!.CandidateNumber,
                SupportEmail = supportEmail,
                IdOfUserMatchingEmailIfAny = userMatchingEmail?.Id,
                UserMatchingEmailIsActive = userMatchingEmail?.Active == true,
                WasPasswordSetByAdmin = !string.IsNullOrWhiteSpace(userAccountToBeClaimed?.PasswordHash),
            };
        }

        public async Task ConvertTemporaryUserToConfirmedUser(
            int userId,
            int centreId,
            string primaryEmail,
            string? password
        )
        {
            userDataService.SetPrimaryEmailAndActivate(userId, primaryEmail);
            userDataService.SetCentreEmail(userId, centreId, null);
            userDataService.SetRegistrationConfirmationHash(userId, centreId, null);

            if (password != null)
            {
                await passwordService.ChangePasswordAsync(userId, password);
            }
        }

        public void LinkAccount(int currentUserIdForAccount, int newUserIdForAccount, int centreId)
        {
            userDataService.LinkDelegateAccountToNewUser(currentUserIdForAccount, newUserIdForAccount, centreId);
            userDataService.LinkUserCentreDetailsToNewUser(currentUserIdForAccount, newUserIdForAccount, centreId);
            userDataService.DeleteUser(currentUserIdForAccount);
        }
    }
}
