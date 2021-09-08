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
            List<AdminUser> unverifiedAdminUsers,
            List<DelegateUser> unverifiedDelegateUsers
        );

        List<DelegateUser> GetVerifiedDelegateUsersAssociatedWithAdminUser(AdminUser? adminUser, string password);

        /// <summary>
        /// Gets a single verified admin associated with a set of delegate users.
        /// This method should only be called with a set of delegate users with the same email address.
        /// </summary>
        AdminUser? GetVerifiedAdminUserAssociatedWithDelegateUsers(List<DelegateUser> delegateUsers, string password);
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
            List<AdminUser> unverifiedAdminUsers,
            List<DelegateUser> unverifiedDelegateUsers
        )
        {
            var verifiedAdminUsers =
                unverifiedAdminUsers.Where(au => cryptoService.VerifyHashedPassword(au.Password, password))
                    .ToList();
            var verifiedDelegateUsers =
                unverifiedDelegateUsers.Where(du => cryptoService.VerifyHashedPassword(du.Password, password))
                    .ToList();

            return new UserAccountSet(verifiedAdminUsers, verifiedDelegateUsers);
        }

        public List<DelegateUser> GetVerifiedDelegateUsersAssociatedWithAdminUser(
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

        public AdminUser? GetVerifiedAdminUserAssociatedWithDelegateUsers(
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
                                  delegateUsers.Any(du => du.CentreId == adminUserAssociatedWithDelegates.CentreId) &&
                                  cryptoService.VerifyHashedPassword(
                                      adminUserAssociatedWithDelegates.Password,
                                      password
                                  );
            return isSuitableAdmin ? adminUserAssociatedWithDelegates : null;
        }
    }
}
