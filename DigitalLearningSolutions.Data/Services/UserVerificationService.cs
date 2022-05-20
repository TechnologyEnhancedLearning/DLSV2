namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserVerificationService
    {
        UserAccountSet VerifyUsers(
            string password,
            AdminUser? unverifiedAdminUser,
            List<DelegateUser> unverifiedDelegateUsers
        );

        UserEntityVerificationResult VerifyUserEntity(
            string password,
            UserEntity userEntity
        );

        List<DelegateUser> GetActiveApprovedVerifiedDelegateUsersAssociatedWithAdminUser(AdminUser? adminUser, string password);

        /// <summary>
        /// Gets a single verified admin associated with a set of delegate users.
        /// This method should only be called with a set of delegate users with the same email address.
        /// </summary>
        AdminUser? GetActiveApprovedVerifiedAdminUserAssociatedWithDelegateUsers(List<DelegateUser> delegateUsers, string password);
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

        public UserAccountSet VerifyUsers(
            string password,
            AdminUser? unverifiedAdminUser,
            List<DelegateUser> unverifiedDelegateUsers
        )
        {
            var verifiedAdminUser =
                cryptoService.VerifyHashedPassword(unverifiedAdminUser?.Password, password)
                    ? unverifiedAdminUser
                    : null;
            var verifiedDelegateUsers =
                unverifiedDelegateUsers.Where(du => cryptoService.VerifyHashedPassword(du.Password, password))
                    .ToList();

            return new UserAccountSet(verifiedAdminUser, verifiedDelegateUsers);
        }

        public UserEntityVerificationResult VerifyUserEntity(string password, UserEntity userEntity)
        {
            var userAccountPassed = cryptoService.VerifyHashedPassword(userEntity.UserAccount.PasswordHash, password);
            var passedDelegateIds = userEntity.DelegateAccounts.Where(
                d => d.OldPassword == null || cryptoService.VerifyHashedPassword(d.OldPassword, password)
            ).Select(d => d.Id);
            var failedDelegateIds = userEntity.DelegateAccounts.Select(d => d.Id)
                .Where(id => !passedDelegateIds.Contains(id));
            return new UserEntityVerificationResult(userAccountPassed, passedDelegateIds, failedDelegateIds);
        }

        public List<DelegateUser> GetActiveApprovedVerifiedDelegateUsersAssociatedWithAdminUser(
            AdminUser? adminUser,
            string password
        )
        {
            if (string.IsNullOrEmpty(adminUser?.EmailAddress))
            {
                return new List<DelegateUser>();
            }

            var delegatesAssociatedWithAdmin = userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!);

            var suitableDelegates = delegatesAssociatedWithAdmin
                .Where(du => du.Active && du.Approved && cryptoService.VerifyHashedPassword(du.Password, password));

            return suitableDelegates.ToList();
        }

        public AdminUser? GetActiveApprovedVerifiedAdminUserAssociatedWithDelegateUsers(
            List<DelegateUser> delegateUsers,
            string password
        )
        {
            var delegateEmail = delegateUsers.FirstOrDefault(du => du.EmailAddress != null)?.EmailAddress;

            if (string.IsNullOrWhiteSpace(delegateEmail))
            {
                return null;
            }

            var adminUserAssociatedWithDelegates = userDataService.GetAdminUserByEmailAddress(delegateEmail);

            var isSuitableAdmin = adminUserAssociatedWithDelegates != null &&
                                  adminUserAssociatedWithDelegates.Active &&
                                  adminUserAssociatedWithDelegates.Approved &&
                                  cryptoService.VerifyHashedPassword(
                                      adminUserAssociatedWithDelegates.Password,
                                      password
                                  );
            return isSuitableAdmin ? adminUserAssociatedWithDelegates : null;
        }
    }
}
