﻿namespace DigitalLearningSolutions.Data.Services
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IPasswordService
    {
        Task ChangePasswordAsync(UserReference user, string newPassword);
        Task ChangePasswordAsync(string email, string newPassword);

        Task ChangePasswordForLinkedUserAccounts(
            AdminUser? admin,
            DelegateUser? candidate,
            string newPassword
        );
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

        public async Task ChangePasswordForLinkedUserAccounts(
            AdminUser? admin,
            DelegateUser? candidate,
            string newPassword
        )
        {
            var emailIfAny = admin?.EmailAddress ?? candidate?.EmailAddress;

            if (!string.IsNullOrWhiteSpace(emailIfAny))
            {
                await ChangePasswordAsync(emailIfAny, newPassword);
            }
            else
            {
                if (admin != null)
                {
                    await ChangePasswordAsync(new UserReference(admin.Id, UserType.AdminUser), newPassword);
                }

                if (candidate != null)
                {
                    await ChangePasswordAsync(new UserReference(candidate.Id, UserType.DelegateUser), newPassword);
                }
            }
        }
    }
}
