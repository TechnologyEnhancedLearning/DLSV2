namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
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
        public async Task Changing_password_by_email_hashes_password_before_saving()
        {
            // Given
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordAsync("email", "new-password1");

            // Then
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordDataService.SetPasswordByEmailAsync(A<string>._, "hash-of-password"))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public async Task Changing_password_by_email_does_not_save_plain_password()
        {
            // Given
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordAsync("email", "new-password1");

            // Then
            A.CallTo(() => passwordDataService.SetPasswordByEmailAsync(A<string>._, "new-password1"))
                .MustNotHaveHappened();
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
            A.CallTo(
                    () => passwordDataService.SetPasswordByUserReferenceAsync(
                        new UserReference(34, UserType.AdminUser),
                        "hash-of-password"
                    )
                )
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public async Task Changing_password_by_user_ref_does_not_save_plain_password()
        {
            // Given
            A.CallTo(() => cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");

            // When
            await passwordService.ChangePasswordAsync(new UserReference(34, UserType.AdminUser), "new-password1");

            // Then
            A.CallTo(
                    () => passwordDataService.SetPasswordByUserReferenceAsync(
                        new UserReference(34, UserType.AdminUser),
                        "new-password1"
                    )
                )
                .MustNotHaveHappened();
        }
    }
}
