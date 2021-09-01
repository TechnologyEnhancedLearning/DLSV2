namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;

    public interface ILoginService
    {
        public UserAccountSet VerifyUsers(
            string password,
            AdminUser? unverifiedAdminUser,
            List<DelegateUser> unverifiedDelegateUsers
        );

        public AdminUser? GetVerifiedAdminUserAssociatedWithDelegateUsers(
            List<DelegateUser> delegateUsers,
            string password
        );
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

        public AdminUser? GetVerifiedAdminUserAssociatedWithDelegateUsers(List<DelegateUser> delegateUsers, string password)
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

        private AdminUser? GetVerifiedAdminUserByEmail(string password, DelegateUser delegateUser)
        {
            if (string.IsNullOrWhiteSpace(delegateUser.EmailAddress))
            {
                return null;
            }

            var adminUserAssociatedWithDelegate = userDataService.GetAdminUserByUsername(delegateUser.EmailAddress);

            var isSuitableAdmin = adminUserAssociatedWithDelegate?.CentreId == delegateUser.CentreId &&
                                  cryptoService.VerifyHashedPassword(
                                      adminUserAssociatedWithDelegate.Password,
                                      password
                                  );
            return isSuitableAdmin ? adminUserAssociatedWithDelegate : null;
        }
    }
}
