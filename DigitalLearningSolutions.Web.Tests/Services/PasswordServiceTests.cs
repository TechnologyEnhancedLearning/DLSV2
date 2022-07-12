namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Services;
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
        public async Task Changing_password_by_email_hashes_password_before_saving()
        {
            // Given
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordAsync("email", "new-password1");

            // Then
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).MustHaveHappened(1, Times.Exactly);
            ThenHasSetPasswordForEmailOnce("email", "hash-of-password");
        }

        [Test]
        public async Task Changing_password_by_email_does_not_save_plain_password()
        {
            // Given
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordAsync("email", "new-password1");

            // Then
            ThenHasNotSetPasswordForAnyUser("new-password1");
        }

        [Test]
        public async Task Changing_password_by_user_refs_hashes_password_before_saving()
        {
            // Given
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");
            var userRefs = new[] { new UserReference(34, UserType.AdminUser) };

            // When
            await passwordService.ChangePasswordAsync(
                userRefs,
                "new-password1"
            );

            // Then
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).MustHaveHappened(1, Times.Exactly);
            ThenHasSetPasswordForUserRefsOnce(userRefs, "hash-of-password");
        }

        [Test]
        public async Task Changing_password_by_user_refs_does_not_save_plain_password()
        {
            // Given
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordAsync(
                new[] { new UserReference(34, UserType.AdminUser) },
                "new-password1"
            );

            // Then
            ThenHasNotSetPasswordForAnyUser("new-password1");
        }

        [Test]
        public async Task Changing_password_by_user_refs_changes_password_for_all_users()
        {
            // Given
            var userRefs = new[]
            {
                new UserReference(1, UserType.AdminUser),
                new UserReference(7, UserType.AdminUser),
                new UserReference(34, UserType.AdminUser),
                new UserReference(355, UserType.DelegateUser),
                new UserReference(654, UserType.DelegateUser),
            };

            A.CallTo(() => cryptoService.GetPasswordHash("new-password")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordAsync(userRefs, "new-password");

            // Then
            ThenHasSetPasswordForUserRefsOnce(userRefs, "hash-of-password");
        }

        private void ThenHasSetPasswordForEmailOnce(string email, string passwordHash)
        {
            A.CallTo(() => passwordDataService.SetPasswordByEmailAsync(email, passwordHash))
                .MustHaveHappened(1, Times.Exactly);
        }

        private void ThenHasSetPasswordForUserRefsOnce(IEnumerable<UserReference> userRefs, string passwordHash)
        {
            A.CallTo(() => passwordDataService.SetPasswordForUsersAsync(userRefs, passwordHash))
                .MustHaveHappened(1, Times.Exactly);
        }

        private void ThenHasNotSetPasswordForAnyUser(string passwordHash)
        {
            A.CallTo(() => passwordDataService.SetPasswordByEmailAsync(A<string>._, passwordHash))
                .MustNotHaveHappened();
        }
    }
}
