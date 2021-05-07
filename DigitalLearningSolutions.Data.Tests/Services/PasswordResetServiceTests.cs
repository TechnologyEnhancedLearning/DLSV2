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
            emailService = A.Fake<IEmailService>();
            connection = ServiceTestHelper.GetDatabaseConnection();

            A.CallTo(() => userService.GetUsersByEmailAddress(A<string>._)).Returns
            ((
                UserTestHelper.GetDefaultAdminUser(),
                new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() }
            ));

            passwordResetService = new PasswordResetService(userService, connection, logger, emailService);
        }

        [Test]
        public void Trying_get_null_user_should_throw_an_exception()
        {
            // Given
            A.CallTo(() => userService.GetUsersByEmailAddress(A<string>._)).Returns((null, new List<DelegateUser>()));

            // Then
            Assert.Throws<UserAccountNotFoundException>(() => passwordResetService.GenerateAndSendPasswordResetLink("recipient@example.com", "example.com"));
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
