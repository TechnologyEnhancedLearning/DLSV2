namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using NUnit.Framework;

    public class PasswordServiceTests
    {
        private ICryptoService cryptoService = null!;
        private IPasswordDataService passwordDataService = null!;
        private PasswordService passwordService = null!;

        [SetUp]
        public void SetUp()
        {
            cryptoService = A.Fake<ICryptoService>();
            passwordDataService = A.Fake<IPasswordDataService>();
            passwordService = new PasswordService(cryptoService, passwordDataService);
        }

        [Test]
        public async Task Changing_password_by_user_id_hashes_password_before_saving()
        {
            // Given
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordAsync(1, "new-password1");

            // Then
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).MustHaveHappened(1, Times.Exactly);
            ThenHasSetPasswordForEmailOnce(1, "hash-of-password");
        }

        [Test]
        public async Task Changing_password_by_user_id_does_not_save_plain_password()
        {
            // Given
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordAsync(1, "new-password1");

            // Then
            ThenHasNotSetPasswordForAnyUser("new-password1");
        }

        [Test]
        public async Task Changing_password_by_user_id_sets_old_passwords_to_null()
        {
            // Given
            A.CallTo(() => cryptoService.GetPasswordHash("new-password")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordAsync(1, "new-password");

            // Then
            ThenHasSetOldPasswordToNullOnce(1);
        }

        private void ThenHasSetPasswordForEmailOnce(int userId, string passwordHash)
        {
            A.CallTo(() => passwordDataService.SetPasswordByUserIdAsync(userId, passwordHash))
                .MustHaveHappened(1, Times.Exactly);
        }

        private void ThenHasSetOldPasswordToNullOnce(int userId)
        {
            A.CallTo(() => passwordDataService.SetOldPasswordsToNullByUserIdAsync(userId))
                .MustHaveHappened(1, Times.Exactly);
        }

        private void ThenHasNotSetPasswordForAnyUser(string passwordHash)
        {
            A.CallTo(() => passwordDataService.SetPasswordByUserIdAsync(A<int>._, passwordHash))
                .MustNotHaveHappened();
        }
    }
}
