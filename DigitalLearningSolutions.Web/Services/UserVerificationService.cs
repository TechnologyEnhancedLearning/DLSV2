namespace DigitalLearningSolutions.Web.Services
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserVerificationService
    {
        UserEntityVerificationResult VerifyUserEntity(
            string password,
            UserEntity userEntity
        );

        bool IsPasswordValid(string? password, int? userId);
    }

    public class UserVerificationService : IUserVerificationService
    {
        private readonly ICryptoService cryptoService;
        private readonly IUserDataService userDataService;

        public UserVerificationService(ICryptoService cryptoService, IUserDataService userDataService)
        {
            this.cryptoService = cryptoService;
            this.userDataService = userDataService;
        }

        public UserEntityVerificationResult VerifyUserEntity(string password, UserEntity userEntity)
        {
            var userAccountPassed = cryptoService.VerifyHashedPassword(userEntity.UserAccount.PasswordHash, password);
            var nullDelegateIds = userEntity.DelegateAccounts
                .Where(d => d.OldPassword == null)
                .Select(d => d.Id).ToList();
            var passedDelegateIds = userEntity.DelegateAccounts
                .Where(d => cryptoService.VerifyHashedPassword(d.OldPassword, password))
                .Select(d => d.Id).ToList();
            var failedDelegateIds = userEntity.DelegateAccounts.Select(d => d.Id)
                .Except(nullDelegateIds.Concat(passedDelegateIds));
            return new UserEntityVerificationResult(
                userAccountPassed,
                nullDelegateIds,
                passedDelegateIds,
                failedDelegateIds
            );
        }

        public bool IsPasswordValid(string? password, int? userId)
        {
            if (string.IsNullOrEmpty(password) || userId == null)
            {
                return false;
            }

            var user = userDataService.GetUserAccountById((int)userId);

            return user != null && cryptoService.VerifyHashedPassword(user.PasswordHash, password);
        }
    }
}
