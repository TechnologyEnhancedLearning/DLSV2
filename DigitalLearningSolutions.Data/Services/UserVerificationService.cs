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

        AdminUser? GetVerifiedAdminUserAssociatedWithDelegateUser(DelegateUser? delegateUser, string password);
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
                .Where(du => du.Active)
                .Where(du => du.Approved)
                .Where(du => du.CentreId == adminUser.CentreId)
                .Where(du => cryptoService.VerifyHashedPassword(du.Password, password));

            return suitableDelegates.ToList();
        }

        public AdminUser? GetVerifiedAdminUserAssociatedWithDelegateUser(DelegateUser? delegateUser, string password)
        {
            if (string.IsNullOrWhiteSpace(delegateUser?.EmailAddress))
            {
                return null;
            }

            var adminUserAssociatedWithDelegate = userDataService.GetAdminUserByEmailAddress(delegateUser.EmailAddress);

            var isSuitableAdmin = adminUserAssociatedWithDelegate?.CentreId == delegateUser.CentreId &&
                                  cryptoService.VerifyHashedPassword(
                                      adminUserAssociatedWithDelegate.Password,
                                      password
                                  );
            return isSuitableAdmin ? adminUserAssociatedWithDelegate : null;
        }
    }
}
