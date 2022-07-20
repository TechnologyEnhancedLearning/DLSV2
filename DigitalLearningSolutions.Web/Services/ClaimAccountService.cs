namespace DigitalLearningSolutions.Web.Services
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.ViewModels.Register;

    public interface IClaimAccountService
    {
        ClaimAccountViewModel GetAccountDetailsForCompletingRegistration(
            int userIdForRegistration,
            int centreId,
            string centreName,
            string email,
            int? loggedInUserId = null
        );

        void ConvertTemporaryUserToConfirmedUser(int userId, int centreId, string primaryEmail);

        void LinkAccount(int oldUserId, int newUserId, int centreId);
    }

    public class ClaimAccountService : IClaimAccountService
    {
        private readonly IUserDataService userDataService;
        private readonly IConfigDataService configDataService;

        public ClaimAccountService(IUserDataService userDataService, IConfigDataService configDataService)
        {
            this.userDataService = userDataService;
            this.configDataService = configDataService;
        }

        public ClaimAccountViewModel GetAccountDetailsForCompletingRegistration(
            int userIdForRegistration,
            int centreId,
            string centreName,
            string email,
            int? loggedInUserId = null
        )
        {
            var userMatchingEmail = userDataService.GetUserAccountByPrimaryEmail(email);
            var emailIsTaken = userMatchingEmail != null &&
                               (loggedInUserId == null || loggedInUserId != userMatchingEmail.Id);
            var userAccountToBeClaimed = userDataService.GetUserAccountById(userIdForRegistration);
            var delegateAccountToBeClaimed = userDataService.GetDelegateAccountsByUserId(userIdForRegistration)
                .SingleOrDefault(da => da.CentreId == centreId);
            var supportEmail = configDataService.GetConfigValue(ConfigDataService.SupportEmail);

            return new ClaimAccountViewModel
            {
                UserId = userIdForRegistration,
                CentreId = centreId,
                CentreName = centreName,
                Email = email,
                CandidateNumber = delegateAccountToBeClaimed!.CandidateNumber,
                SupportEmail = supportEmail,
                EmailIsTaken = emailIsTaken,
                EmailIsTakenByActiveUser = emailIsTaken && userMatchingEmail.Active,
                PasswordSet = !string.IsNullOrWhiteSpace(userAccountToBeClaimed?.PasswordHash),
            };
        }

        public void ConvertTemporaryUserToConfirmedUser(int userId, int centreId, string primaryEmail)
        {
            userDataService.SetPrimaryEmailAndActivate(userId, primaryEmail);
            userDataService.SetCentreEmail(userId, centreId, null);
            userDataService.SetRegistrationConfirmationHash(userId, centreId, null);
        }

        public void LinkAccount(int oldUserId, int newUserId, int centreId)
        {
            userDataService.LinkDelegateAccountToNewUser(oldUserId, newUserId, centreId);
            userDataService.LinkUserCentreDetailsToNewUser(oldUserId, newUserId, centreId);
            userDataService.DeleteUser(oldUserId);
        }
    }
}
