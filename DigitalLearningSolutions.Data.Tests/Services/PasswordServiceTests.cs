namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
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
        public async Task Changing_password_by_user_ref_hashes_password_before_saving()
        {
            // Given
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordAsync(new UserReference(34, UserType.AdminUser), "new-password1");

            // Then
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).MustHaveHappened(1, Times.Exactly);
            ThenHasSetPasswordForUserOnce(new UserReference(34, UserType.AdminUser), "hash-of-password");
        }

        [Test]
        public async Task Changing_password_by_user_ref_does_not_save_plain_password()
        {
            // Given
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordAsync(new UserReference(34, UserType.AdminUser), "new-password1");

            // Then
            ThenHasNotSetPasswordForAnyUser("new-password1");
        }

        [Test]
        public async Task Changing_password_for_linked_accounts_uses_email_if_exists()
        {
            // Given
            var admin = Builder<AdminUser>.CreateNew().With(u => u.EmailAddress = "email").Build();
            var delegateUser = Builder<DelegateUser>.CreateNew().With(u => u.EmailAddress = "email").Build();
            A.CallTo(() => cryptoService.GetPasswordHash("new-password")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordForLinkedUserAccounts(admin, delegateUser, "new-password");

            // Then
            ThenHasSetPasswordForEmailOnce("email", "hash-of-password");
            ThenHasNotSetPasswordByUserRef();
        }

        [Test]
        public async Task Changing_password_for_linked_accounts_uses_admin_email_if_no_delegate_email()
        {
            // Given
            var admin = Builder<AdminUser>.CreateNew().With(u => u.EmailAddress = "email").Build();
            A.CallTo(() => cryptoService.GetPasswordHash("new-password")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordForLinkedUserAccounts(admin, null, "new-password");

            // Then
            ThenHasSetPasswordForEmailOnce("email", "hash-of-password");
            ThenHasNotSetPasswordByUserRef();
        }

        [Test]
        public async Task Changing_password_for_linked_accounts_uses_delegate_email_if_no_admin_email()
        {
            // Given
            var delegateUser = Builder<DelegateUser>.CreateNew().With(u => u.EmailAddress = "email").Build();
            A.CallTo(() => cryptoService.GetPasswordHash("new-password")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordForLinkedUserAccounts(null, delegateUser, "new-password");

            // Then
            ThenHasSetPasswordForEmailOnce("email", "hash-of-password");
            ThenHasNotSetPasswordByUserRef();
        }

        [Test]
        public async Task Changing_password_for_linked_accounts_uses_user_ids_if_no_email()
        {
            // Given
            var admin = Builder<AdminUser>.CreateNew().With(u => u.EmailAddress = null).With(u => u.Id = 34).Build();
            var delegateUser = Builder<DelegateUser>.CreateNew().With(u => u.EmailAddress = null).With(u => u.Id = 309)
                .Build();
            A.CallTo(() => cryptoService.GetPasswordHash("new-password")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordForLinkedUserAccounts(admin, delegateUser, "new-password");

            // Then
            ThenHasNotSetPasswordByEmail();
            ThenHasSetPasswordForUserOnce(new UserReference(34, UserType.AdminUser), "hash-of-password");
            ThenHasSetPasswordForUserOnce(new UserReference(309, UserType.DelegateUser), "hash-of-password");
        }

        private void ThenHasSetPasswordForEmailOnce(string email, string passwordHash)
        {
            A.CallTo(() => passwordDataService.SetPasswordByEmailAsync(email, passwordHash))
                .MustHaveHappened(1, Times.Exactly);
        }

        private void ThenHasSetPasswordForUserOnce(UserReference userRef, string passwordHash)
        {
            A.CallTo(
                    () => passwordDataService.SetPasswordByUserReferenceAsync(
                        userRef,
                        passwordHash
                    )
                )
                .MustHaveHappened(1, Times.Exactly);
        }

        private void ThenHasNotSetPasswordByEmail()
        {
            A.CallTo(() => passwordDataService.SetPasswordByEmailAsync(A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        private void ThenHasNotSetPasswordByUserRef()
        {
            A.CallTo(() => passwordDataService.SetPasswordByUserReferenceAsync(A<UserReference>._, A<string>._))
                .MustNotHaveHappened();
        }

        private void ThenHasNotSetPasswordForAnyUser(string passwordHash)
        {
            A.CallTo(() => passwordDataService.SetPasswordByEmailAsync(A<string>._, passwordHash))
                .MustNotHaveHappened();
            A.CallTo(() => passwordDataService.SetPasswordByUserReferenceAsync(A<UserReference>._, passwordHash))
                .MustNotHaveHappened();
        }
    }
}
