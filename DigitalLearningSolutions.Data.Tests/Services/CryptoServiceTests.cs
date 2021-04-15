namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Services;
    using NUnit.Framework;

    public class CryptoServiceTests
    {
        private ICryptoService cryptoService;

        [SetUp]
        public void Setup()
        {
            cryptoService = new CryptoService();
        }

        [Test]
        public void VerifyHashedPassword_Returns_false_if_hashed_password_is_empty()
        {
            // When
            var passwordIsCorrect = cryptoService.VerifyHashedPassword(null, "password");

            // Then
            Assert.IsFalse(passwordIsCorrect);
        }
    }
}
