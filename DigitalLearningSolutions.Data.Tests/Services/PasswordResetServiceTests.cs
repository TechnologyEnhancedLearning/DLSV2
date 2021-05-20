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
        private IPasswordResetDataService passwordResetDataService = null!;
        private IEmailService emailService = null!;
        private ILogger<PasswordResetService> logger = null!;
        private PasswordResetService passwordResetService = null!;
        private IUserService userService = null!;
        private IClockService clockService = null!;
        private ICryptoService cryptoService = null!;
        private IPasswordDataService passwordDataService = null!;

        [SetUp]
        public void SetUp()
        {
            userService = A.Fake<IUserService>();
            logger = A.Fake<ILogger<PasswordResetService>>();
            emailService = A.Fake<IEmailService>();
            clockService = A.Fake<IClockService>();
            passwordResetDataService = A.Fake<IPasswordResetDataService>();
            cryptoService = A.Fake<ICryptoService>();
            passwordDataService = A.Fake<IPasswordDataService>();

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
                clockService,
                cryptoService,
                passwordDataService);
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
            var hashIsValid = await passwordResetService.EmailAndResetPasswordHashAreValidAsync(emailAddress, hash);

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
            var hashIsValid =
                await passwordResetService.EmailAndResetPasswordHashAreValidAsync(emailAddress, resetHash);

            // Then
            hashIsValid.Should().BeTrue();
        }

        [Test]
        public async Task Removes_reset_password_for_exact_users_matching_email()
        {
            // Given
            var users = (
                Builder<AdminUser>.CreateNew().With(u => u.ResetPasswordId = 1).Build(),
                new[] { Builder<DelegateUser>.CreateNew().With(u => u.ResetPasswordId = 4).Build() }.ToList());
            A.CallTo(() => this.userService.GetUsersByEmailAddress("email")).Returns(users);

            // When
            await this.passwordResetService.InvalidateResetPasswordForEmailAsync("email");

            // Then
            A.CallTo(() => this.passwordResetDataService.RemoveResetPasswordAsync(1))
                .MustHaveHappened(1, Times.OrMore);
            A.CallTo(() => this.passwordResetDataService.RemoveResetPasswordAsync(4))
                .MustHaveHappened(1, Times.OrMore);
            A.CallTo(() => this.passwordResetDataService.RemoveResetPasswordAsync(A<int>._))
                .WhenArgumentsMatch(args => args.Get<int>(0) != 1 && args.Get<int>(0) != 4).MustNotHaveHappened();
        }

        [Test]
        public async Task Changing_password_hashes_password_before_saving()
        {
            // Given
            A.CallTo(() => this.cryptoService.GetPasswordHash("new-password1")).Returns("hash-of-password");

            // When
            await this.passwordResetService.ChangePasswordAsync("email", "new-password1");

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
            await this.passwordResetService.ChangePasswordAsync("email", "new-password1");

            // Then
            A.CallTo(() => this.passwordDataService.SetPasswordByEmailAsync(A<string>._, "new-password1"))
                .MustNotHaveHappened();
        }

        private void GivenCurrentTimeIs(DateTime validationTime)
        {
            A.CallTo(() => clockService.UtcNow).Returns(validationTime);
        }
    }
}
