namespace DigitalLearningSolutions.Web.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IPasswordService
    {
        Task ChangePasswordAsync(string email, string newPassword);
        Task ChangePasswordAsync(IEnumerable<UserReference> users, string newPassword);
    }

    public class PasswordService : IPasswordService
    {
        private readonly ICryptoService cryptoService;
        private readonly IPasswordDataService passwordDataService;

        public PasswordService(ICryptoService cryptoService, IPasswordDataService passwordDataService)
        {
            this.cryptoService = cryptoService;
            this.passwordDataService = passwordDataService;
        }

        public async Task ChangePasswordAsync(string email, string newPassword)
        {
            var hashOfPassword = cryptoService.GetPasswordHash(newPassword);
            await passwordDataService.SetPasswordByEmailAsync(email, hashOfPassword);
        }

        public async Task ChangePasswordAsync(IEnumerable<UserReference> users, string newPassword)
        {
            var hashOfPassword = cryptoService.GetPasswordHash(newPassword);
            await passwordDataService.SetPasswordForUsersAsync(users, hashOfPassword);
        }
    }
}
