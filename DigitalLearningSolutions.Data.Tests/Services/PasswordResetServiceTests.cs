namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Auth;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class PasswordResetServiceTests
    {
        private IPasswordResetDataService passwordResetDataService;
        private IEmailService emailService;
        private ILogger<PasswordResetService> logger;
        private PasswordResetService passwordResetService;
        private IUserService userService;
        private IClockService clockService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            userService = A.Fake<IUserService>();
            logger = A.Fake<ILogger<PasswordResetService>>();
            emailService = A.Fake<IEmailService>();
            clockService = A.Fake<IClockService>();
            passwordResetDataService = A.Fake<IPasswordResetDataService>();

            A.CallTo(() => userService.GetUsersByEmailAddress(A<string>._)).Returns
            (
                (
                    UserTestHelper.GetDefaultAdminUser(),
                    new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() }
                ));

            passwordResetService = new PasswordResetService(
                userService,
                passwordResetDataService,
                logger,
                emailService,
                clockService);
        }

        [Test]
        public void Trying_get_null_user_should_throw_an_exception()
        {
            // Given
            A.CallTo(() => userService.GetUsersByEmailAddress(A<string>._)).Returns((null, new List<DelegateUser>()));

            // Then
            Assert.Throws<UserAccountNotFoundException>(
                () => passwordResetService.GenerateAndSendPasswordResetLink("recipient@example.com", "example.com"));
        }

        [Test]
        public void Trying_to_send_password_reset_sends_email()
        {
            // Given
            var emailAddress = "recipient@example.com";
            var adminUser = Builder<AdminUser>.CreateNew()
                .With(user => user.EmailAddress = emailAddress)
                .Build();

            A.CallTo(() => userService.GetUsersByEmailAddress(emailAddress))
                .Returns((adminUser, new List<DelegateUser>()));

            // When
            passwordResetService.GenerateAndSendPasswordResetLink(emailAddress, "example.com");

            // Then
            A.CallTo(
                    () =>
                        emailService.SendEmail(
                            A<Email>.That.Matches(
                                e =>
                                    e.To[0] == emailAddress &&
                                    e.Cc.IsNullOrEmpty() &&
                                    e.Bcc.IsNullOrEmpty() &&
                                    e.Subject == "Digital Learning Solutions Tracking System Password Reset"))
                )
                .MustHaveHappened();
            ;
        }

        [Test]
        public async Task Reset_password_is_discounted_if_expired()
        {
            // Given
            var createTime = DateTime.UtcNow;
            var emailAddress = "email";
            var hash = "hash";

            var resetPasswordWithUserDetails = Builder<ResetPasswordWithUserDetails>.CreateNew()
                .With(rp => rp.PasswordResetDateTime = createTime)
                .Build();
            A.CallTo(
                    () => passwordResetDataService.FindMatchingResetPasswordEntitiesWithUserDetailsAsync(
                        emailAddress,
                        hash))
                .Returns(Task.FromResult(new[] { resetPasswordWithUserDetails }.ToList()));

            GivenCurrentTimeIs(createTime + TimeSpan.FromMinutes(125));

            // When
            var hashIsValid = await passwordResetService.EmailAndResetPasswordHashAreValid(emailAddress, hash);

            // Then
            hashIsValid.Should().BeFalse();
        }

        [Test]
        public async Task User_references_are_correctly_calculated()
        {
            // Given
            var createTime = DateTime.UtcNow;
            var resetPasswords = Builder<ResetPasswordWithUserDetails>.CreateListOfSize(3)
                .All().With(rp => rp.PasswordResetDateTime = createTime)
                .TheFirst(2).With(rp => rp.UserType = UserType.DelegateUser)
                .TheRest().With(rp => rp.UserType = UserType.AdminUser)
                .TheFirst(1).With(rp => rp.UserId = 7)
                .TheNext(1).With(rp => rp.UserId = 2)
                .TheNext(1).With(rp => rp.UserId = 4)
                .Build().ToList();
            var emailAddress = "email";
            var resetHash = "hash";

            A.CallTo(
                    () => passwordResetDataService.FindMatchingResetPasswordEntitiesWithUserDetailsAsync(
                        emailAddress,
                        resetHash))
                .Returns(Task.FromResult(resetPasswords));

            GivenCurrentTimeIs(createTime + TimeSpan.FromMinutes(2));

            // When
            var hashIsValid = await passwordResetService.EmailAndResetPasswordHashAreValid(emailAddress, resetHash);

            // Then
            hashIsValid.Should().BeTrue();
        }

        private void GivenCurrentTimeIs(DateTime validationTime)
        {
            A.CallTo(() => clockService.UtcNow).Returns(validationTime);
        }
    }
}
