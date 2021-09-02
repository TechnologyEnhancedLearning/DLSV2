namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;

    public interface ILoginService
    {
        UserAccountSet VerifyUsers(
            string password,
            List<AdminUser> unverifiedAdminUsers,
            List<DelegateUser> unverifiedDelegateUsers
        );

        AdminUser? GetVerifiedAdminUserAssociatedWithDelegateUsers(
            List<DelegateUser> delegateUsers,
            string password
        );

        List<DelegateUser> GetVerifiedDelegateUsersAssociatedWithAdminUser(AdminUser adminUser, string password);
    }

    public class LoginService : ILoginService
    {
        private readonly ICryptoService cryptoService;
        private readonly IUserDataService userDataService;

        public LoginService(IUserDataService userDataService, ICryptoService cryptoService)
        {
            this.userDataService = userDataService;
            this.cryptoService = cryptoService;
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

        public AdminUser? GetVerifiedAdminUserAssociatedWithDelegateUsers(
            List<DelegateUser> delegateUsers,
            string password
        )
        {
            var adminUsers = new List<AdminUser?>();

            foreach (var delegateUser in delegateUsers)
            {
                var admin = GetVerifiedAdminUserByEmail(password, delegateUser);
                if (admin != null)
                {
                    adminUsers.Add(admin);
                }
            }

            return adminUsers.Distinct().SingleOrDefault();
        }

        public List<DelegateUser> GetVerifiedDelegateUsersAssociatedWithAdminUser(AdminUser adminUser, string password)
        {
            var delegatesAssociatedWithAdmin = userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!);

            var suitableDelegates = delegatesAssociatedWithAdmin
                .Where(du => du.Active)
                .Where(du => du.Approved)
                .Where(du => du.CentreId == adminUser.CentreId)
                .Where(du => cryptoService.VerifyHashedPassword(du.Password, password));

            return suitableDelegates.ToList();
        }

        private AdminUser? GetVerifiedAdminUserByEmail(string password, DelegateUser delegateUser)
        {
            if (string.IsNullOrWhiteSpace(delegateUser.EmailAddress))
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
