namespace DigitalLearningSolutions.Web.Services
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.ViewModels.Register;

    public interface IClaimAccountService
    {
        ClaimAccountViewModel GetAccountDetailsForCompletingRegistration(
            int userId,
            int centreId,
            string centreName,
            string email
        );

        void ConvertTemporaryUserToConfirmedUser(int userId, int centreId, string primaryEmail);
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
            int userId,
            int centreId,
            string centreName,
            string email
        )
        {
            var existingUserOwningEmailIfAny = userDataService.GetUserAccountByPrimaryEmail(email);
            var userAccountToBeClaimed = userDataService.GetUserAccountById(userId);
            var delegateAccountToBeClaimed = userDataService.GetDelegateAccountsByUserId(userId)
                .SingleOrDefault(da => da.CentreId == centreId);
            var supportEmail = configDataService.GetConfigValue(ConfigDataService.SupportEmail);

            return new ClaimAccountViewModel
            {
                UserId = userId,
                CentreId = centreId,
                CentreName = centreName,
                CentreSpecificEmail = email,
                CandidateNumber = delegateAccountToBeClaimed!.CandidateNumber,
                SupportEmail = supportEmail,
                EmailIsTaken = existingUserOwningEmailIfAny != null,
                EmailIsTakenByActiveUser = existingUserOwningEmailIfAny?.Active ?? false,
                PasswordSet = !string.IsNullOrWhiteSpace(userAccountToBeClaimed?.PasswordHash),
            };
        }

        public void ConvertTemporaryUserToConfirmedUser(int userId, int centreId, string primaryEmail)
        {
            userDataService.SetPrimaryEmailAndActivate(userId, primaryEmail);
            userDataService.SetCentreEmail(userId, centreId, null);
            userDataService.SetRegistrationConfirmationHash(userId, centreId, null);
        }
    }
}
