namespace DigitalLearningSolutions.Data.Services
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;

    public interface IPasswordService
    {
        Task ChangePasswordAsync(string email, string newPassword);
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
    }
}
