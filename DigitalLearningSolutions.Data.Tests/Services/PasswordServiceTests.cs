namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
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
        public async Task Changing_password_hashes_password_before_saving()
        {
            // Given
            A.CallTo(() => this.cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");

            // When
            await this.passwordService.ChangePasswordAsync("email", "new-password1");

            // Then
            A.CallTo(() => this.cryptoService.GetPasswordHash("new-password1")).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => this.passwordDataService.SetPasswordByEmailAsync(A<string>._, "hash-of-password"))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public async Task Changing_password_does_not_save_plain_password()
        {
            // Given
            A.CallTo(() => this.cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");

            // When
            await this.passwordService.ChangePasswordAsync("email", "new-password1");

            // Then
            A.CallTo(() => this.passwordDataService.SetPasswordByEmailAsync(A<string>._, "new-password1"))
                .MustNotHaveHappened();
        }
    }
}
