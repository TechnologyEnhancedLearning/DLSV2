namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;

    public interface ILoginService
    {
        public (AdminUser?, List<DelegateUser>) VerifyUsers(
            string password, AdminUser? unverifiedAdminUser, List<DelegateUser> unverifiedDelegateUsers);

        public AdminUser? GetVerifiedAdminUserAssociatedWithDelegateUser(DelegateUser approvedDelegateUser,
            string password);
    }

    public class LoginService : ILoginService
    {
        private readonly ICryptoService cryptoService;
        private readonly IUserService userService;

        public LoginService(IUserService userService, ICryptoService cryptoService)
        {
            this.userService = userService;
            this.cryptoService = cryptoService;
        }

        public (AdminUser?, List<DelegateUser>) VerifyUsers(string password, AdminUser? unverifiedAdminUser,
            List<DelegateUser> unverifiedDelegateUsers)
        {
            var verifiedAdminUser =
                cryptoService.VerifyHashedPassword(unverifiedAdminUser?.Password, password)
                    ? unverifiedAdminUser
                    : null;
            var verifiedDelegateUsers =
                unverifiedDelegateUsers.Where(du => cryptoService.VerifyHashedPassword(du.Password, password))
                    .ToList();

            return (verifiedAdminUser, verifiedDelegateUsers);
        }

        public AdminUser? GetVerifiedAdminUserAssociatedWithDelegateUser(DelegateUser approvedDelegateUser,
            string password)
        {
            if (string.IsNullOrWhiteSpace(approvedDelegateUser.EmailAddress))
            {
                return null;
            }

            var adminUserAssociatedWithDelegate =
                userService.GetAdminUserByUsername(approvedDelegateUser.EmailAddress);
            return cryptoService.VerifyHashedPassword(adminUserAssociatedWithDelegate?.Password, password)
                ? adminUserAssociatedWithDelegate
                : null;
        }
    }
}
