namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.DbModels;
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
                    new List<AdminUser> { UserTestHelper.GetDefaultAdminUser() },
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
            A.CallTo(() => userService.GetUsersByEmailAddress(A<string>._))
                .Returns((new List<AdminUser>(), new List<DelegateUser>()));

            // Then
            Assert.Throws<UserAccountNotFoundException>(
                () => passwordResetService.GenerateAndSendPasswordResetLink("recipient@example.com", "example.com"));
        }

        [Test]
        public void Trying_to_send_password_reset_sends_email()
        {
            // Given
            var emailAddress = "recipient@example.com";
            var adminUser = Builder<AdminUser>.CreateNew().With(user => user.EmailAddress = emailAddress)
                .Build();

            A.CallTo(() => userService.GetUsersByEmailAddress(A<string>._))
                .Returns((new[] { adminUser }.ToList(), new List<DelegateUser>()));

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
        public async Task Reset_password_hash_is_invalid_if_125_minutes_old()
        {
            // Given
            var createTime = DateTime.UtcNow;

            var resetPasswordModel = Builder<ResetPassword>.CreateNew()
                .With(m => m.Id = 1)
                .With(m => m.PasswordResetDateTime = createTime)
                .With(m => m.ResetPasswordHash = "Old Hash Brown")
                .Build();
            A.CallTo(() => passwordResetDataService.FindAsync(1)).Returns(Task.FromResult(resetPasswordModel));

            var candidate = Builder<DelegateUser>.CreateNew()
                .With(user => user.ResetPasswordId = resetPasswordModel.Id)
                .Build();
            A.CallTo(() => userService.GetUsersByEmailAddress(A<string>._))
                .Returns((new List<AdminUser>(), new[] { candidate }.ToList()));

            GivenCurrentTimeIs(createTime.Add(TimeSpan.FromMinutes(125)));

            // When
            var isValid = await passwordResetService.PasswordResetHashIsValidAsync("email", "New Hash Brown");

            // Then
            isValid.Should().BeFalse();
        }

        [Test]
        public async Task New_reset_password_is_valid_115_minutes_later()
        {
            // Given
            var createTime = DateTime.UtcNow;

            var resetPasswordModel = Builder<ResetPassword>.CreateNew()
                .With(m => m.Id = 1)
                .With(m => m.PasswordResetDateTime = createTime)
                .With(m => m.ResetPasswordHash = "New Hash Brown")
                .Build();
            GivenResetPasswordExists(resetPasswordModel);

            var candidate = Builder<DelegateUser>.CreateNew().With(user => user.ResetPasswordId = resetPasswordModel.Id)
                .Build();
            A.CallTo(() => userService.GetUsersByEmailAddress(A<string>._))
                .Returns((new List<AdminUser>(), new[] { candidate }.ToList()));

            GivenCurrentTimeIs(createTime.Add(TimeSpan.FromMinutes(115)));

            // When
            var isValid = await passwordResetService.PasswordResetHashIsValidAsync("email", "New Hash Brown");

            // Then
            isValid.Should().BeTrue();
        }

        [Test]
        public async Task Reset_password_hash_is_invalid_if_no_user_matching_email()
        {
            // Given
            var createTime = DateTime.UtcNow;

            var resetPasswordModel = Builder<ResetPassword>.CreateNew()
                .With(m => m.Id = 1)
                .With(m => m.PasswordResetDateTime = createTime)
                .With(m => m.ResetPasswordHash = "New Hash Brown")
                .Build();
            GivenResetPasswordExists(resetPasswordModel);

            A.CallTo(() => userService.GetUsersByEmailAddress("Albus.Dumbledore@Hogwarts.com"))
                .Returns((new List<AdminUser>(), new List<DelegateUser>()));

            GivenCurrentTimeIs(createTime + TimeSpan.FromMinutes(2));

            // When
            var isValid = await passwordResetService.PasswordResetHashIsValidAsync(
                "Albus.Dumbledore@Hogwarts.com",
                "New Hash Brown");

            // Then
            isValid.Should().BeFalse();
        }

        [Test]
        public async Task Reset_password_hash_is_invalid_if_hash_does_not_match_db_entity()
        {
            // Given
            const string emailAddress = "Albus.Dumbledore@Hogwarts.com";

            var createTime = DateTime.UtcNow;
            A.CallTo(() => clockService.UtcNow).Returns(createTime + TimeSpan.FromMinutes(2));

            var resetPasswordModel = Builder<ResetPassword>.CreateNew()
                .With(m => m.Id = 1)
                .With(m => m.PasswordResetDateTime = createTime)
                .With(m => m.ResetPasswordHash = "New Hash Brown")
                .Build();
            GivenResetPasswordExists(resetPasswordModel);

            var candidate = Builder<DelegateUser>.CreateNew().With(u => u.ResetPasswordId = resetPasswordModel.Id)
                .Build();
            A.CallTo(() => userService.GetUsersByEmailAddress(emailAddress))
                .Returns((new List<AdminUser>(), new[] { candidate }.ToList()));

            // When
            var isValid = await passwordResetService.PasswordResetHashIsValidAsync(
                emailAddress,
                "Nonexistent Hash");

            // Then
            isValid.Should().BeFalse();
        }

        private void GivenResetPasswordExists(ResetPassword resetPasswordModel)
        {
            A.CallTo(() => passwordResetDataService.FindAsync(resetPasswordModel.Id))
                .Returns(Task.FromResult(resetPasswordModel));
        }

        private void GivenCurrentTimeIs(DateTime validationTime)
        {
            A.CallTo(() => clockService.UtcNow).Returns(validationTime);
        }
    }
}
