namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Data;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class PasswordResetServiceTests
    {
        private IConfigService configService;
        private IDbConnection connection;
        private IEmailService emailService;
        private ILogger<PasswordResetService> logger;
        private PasswordResetService passwordResetService;
        private IUserService userService;

        [SetUp]
        public void Setup()
        {
            userService = A.Fake<IUserService>();
            logger = A.Fake<ILogger<PasswordResetService>>();
            configService = A.Fake<IConfigService>();
            emailService = A.Fake<IEmailService>();
            connection = ServiceTestHelper.GetDatabaseConnection();

            A.CallTo(() => userService.GetUsersByEmailAddress(A<string>._)).Returns
            ((
                new List<AdminUser> { UserTestHelper.GetDefaultAdminUser() },
                new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() }
            ));

            passwordResetService = new PasswordResetService(userService, connection, logger, configService, emailService);
        }

        [Test]
        public void Trying_get_null_user_should_throw_an_exception()
        {
            // Given
            A.CallTo(() => userService.GetUsersByEmailAddress(A<string>._)).Returns((new List<AdminUser>(), new List<DelegateUser>()));

            // Then
            Assert.Throws<EmailAddressNotFoundException>(() => passwordResetService.GenerateAndSendPasswordResetLink("recipient@example.com", "example.com"));
        }

        [Test]
        public void Trying_to_send_password_reset_sends_email()
        {
            // When
            passwordResetService.GenerateAndSendPasswordResetLink("recipient@example.com", "example.com");

            // Then
            A.CallTo(() =>
                    emailService.SendEmail(A<Email>.That.Matches(e =>
                        e.To[0] == "recipient@example.com" &&
                        e.Cc.IsNullOrEmpty() &&
                        e.Bcc.IsNullOrEmpty() &&
                        e.Subject == "Digital Learning Solutions Tracking System Password Reset"))
                )
                .MustHaveHappened();
        }
    }
}
