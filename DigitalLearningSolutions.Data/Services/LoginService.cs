namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.User;

    public interface ILoginService
    {
        (AdminUser?, DelegateUser?) VerifyUserDetailsAndGetClaims(string username, string password);
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

        public (AdminUser?, DelegateUser?) VerifyUserDetailsAndGetClaims(string username, string password)
        {
            List<AdminUser> adminUsers = userService.GetAdminUsersByUsername(username);
            List<DelegateUser> delegateUsers = userService.GetDelegateUsersByUsername(username);

            if (adminUsers.Count == 0 && delegateUsers.Count == 0)
            {
                throw new UserAccountNotFoundException("No account with that email address or user ID could be found.");
            }

            var verifiedAdminUser = adminUsers.FirstOrDefault(u => cryptoService.VerifyHashedPassword(u.Password, password));
            var verifiedDelegateUser = delegateUsers.FirstOrDefault(u => cryptoService.VerifyHashedPassword(u.Password, password));

            if (verifiedAdminUser == null && verifiedDelegateUser == null)
            {
                throw new IncorrectPasswordLoginException("The password you have entered is incorrect.");
            }

            if (verifiedAdminUser == null && verifiedDelegateUser?.EmailAddress != null)
            {
                verifiedAdminUser ??=
                    userService.GetAdminUsersByUsername(verifiedDelegateUser.EmailAddress)
                        .FirstOrDefault(u => cryptoService.VerifyHashedPassword(u.Password, password));
                if (!verifiedDelegateUser.Approved && verifiedAdminUser == null)
                {
                    throw new DelegateUserNotApprovedException("Only an unapproved delegate account has been found and verified.");
                }
            }

            return (verifiedAdminUser, verifiedDelegateUser);
        }
    }
}
