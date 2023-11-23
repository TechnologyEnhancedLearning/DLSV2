namespace DigitalLearningSolutions.Web.Tests.Services
{
    using DigitalLearningSolutions.Web.Services;
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

        [Test]
        public void VerifyHashedPassword_Returns_true_for_hash_from_GetPasswordHash()
        {
            // When
            var password = "Password!1";
            var passwordHash = cryptoService.GetPasswordHash(password);
            var passwordIsCorrect = cryptoService.VerifyHashedPassword(passwordHash, password);

            // Then
            Assert.IsTrue(passwordIsCorrect);
        }
    }
}
