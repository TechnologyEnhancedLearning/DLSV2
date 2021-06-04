﻿namespace DigitalLearningSolutions.Data.Services
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IPasswordService
    {
        Task ChangePasswordAsync(UserReference user, string newPassword);
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

        public async Task ChangePasswordAsync(UserReference user, string newPassword)
        {
            var hashOfPassword = cryptoService.GetPasswordHash(newPassword);
            await passwordDataService.SetPasswordByUserReferenceAsync(user, hashOfPassword);
        }

        public async Task ChangePasswordAsync(string email, string newPassword)
        {
            var hashOfPassword = cryptoService.GetPasswordHash(newPassword);
            await passwordDataService.SetPasswordByEmailAsync(email, hashOfPassword);
        }
    }
}
