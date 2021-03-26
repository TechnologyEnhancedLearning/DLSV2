namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class PasswordResetServiceTests
    {
        private PasswordResetService passwordResetService;
        private IDbConnection connection;
        private IUserService userService;
        private ILogger<PasswordResetService> logger;
        private IConfigService configService;
        private IEmailService emailService;

        [SetUp]
        public void Setup()
        {
            userService = A.Fake<IUserService>();
            logger = A.Fake<ILogger<PasswordResetService>>();
            configService = A.Fake<IConfigService>();
            emailService = A.Fake<IEmailService>();
            connection = ServiceTestHelper.GetDatabaseConnection();

            A.CallTo(() => userService.GetUserByEmailAddress(A<string>._)).Returns(new DelegateUser
            {
                Id = 1,
                FirstName = "Forename",
                Surname = "Surname",
                EmailAddress = "recipient@example.com",
                ResetPasswordId = null
            });

            A.CallTo(() => configService.GetConfigValue(ConfigService.BaseUrl)).Returns("https://example.com");

            passwordResetService = new PasswordResetService(userService, connection, logger, configService, emailService);
        }


        [Test]
        public void Trying_get_null_user_should_throw_an_exception()
        {
            // Given
            A.CallTo(() => userService.GetUserByEmailAddress(A<string>._)).Returns(null);

            // Then
            Assert.Throws<EmailAddressNotFoundException>(() => passwordResetService.SendResetPasswordEmail("recipient@example.com"));
        }

        [Test]
        public void Trying_to_send_reset_password_email_with_null_config_values_should_throw_an_exception()
        {
            // Given
            A.CallTo(() => configService.GetConfigValue(ConfigService.BaseUrl)).Returns(null);

            // Then
            Assert.Throws<ConfigValueMissingException>(() => passwordResetService.SendResetPasswordEmail("recipient@example.com"));
        }

        [Test]
        public void Trying_to_send_password_reset_sends_email()
        {
            // When
            passwordResetService.SendResetPasswordEmail("recipient@example.com");

            // Then
            A.CallTo(() =>
                    emailService.SendEmail(A<Email>._)
                )
                .MustHaveHappened();
        }
    }
}
