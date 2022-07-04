namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Auth;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using NUnit.Framework;

    public class PasswordResetServiceTests
    {
        private IClockService clockService = null!;
        private IEmailService emailService = null!;
        private IPasswordResetDataService passwordResetDataService = null!;
        private PasswordResetService passwordResetService = null!;
        private IPasswordService passwordService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            userService = A.Fake<IUserService>();
            emailService = A.Fake<IEmailService>();
            clockService = A.Fake<IClockService>();
            passwordResetDataService = A.Fake<IPasswordResetDataService>();
            passwordService = A.Fake<IPasswordService>();

            A.CallTo(() => userService.GetUserByEmailAddress(A<string>._))
                .Returns(UserTestHelper.GetDefaultUserAccount());

            passwordResetService = new PasswordResetService(
                userService,
                passwordResetDataService,
                passwordService,
                emailService,
                clockService
            );
        }

        [Test]
        public void Trying_to_get_null_user_should_throw_an_exception()
        {
            // Given
            A.CallTo(() => userService.GetUserByEmailAddress(A<string>._)).Returns(null);

            // Then
            Assert.ThrowsAsync<UserAccountNotFoundException>(
                async () => await passwordResetService.GenerateAndSendPasswordResetLink(
                    "recipient@example.com",
                    "example.com"
                )
            );
        }

        [Test]
        public async Task Trying_to_send_password_reset_sends_email()
        {
            // Given
            var emailAddress = "recipient@example.com";
            var user = new UserAccount
            {
                FirstName = "Test",
                LastName = "User",
                PrimaryEmail = emailAddress,
            };

            A.CallTo(() => userService.GetUserByEmailAddress(emailAddress)).Returns(user);

            // When
            await passwordResetService.GenerateAndSendPasswordResetLink(emailAddress, "example.com");

            // Then
            A.CallTo(
                    () =>
                        emailService.SendEmail(
                            A<Email>.That.Matches(
                                e =>
                                    e.To[0] == emailAddress &&
                                    e.Body.TextBody.Contains("Dear Test User") &&
                                    e.Cc.IsNullOrEmpty() &&
                                    e.Bcc.IsNullOrEmpty() &&
                                    e.Subject == "Digital Learning Solutions Tracking System Password Reset"
                            )
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public async Task Requesting_password_reset_clears_previous_hashes()
        {
            // Given
            var emailAddress = "recipient@example.com";
            var resetPasswordId = 1;
            var user = new UserAccount
            {
                ResetPasswordId = resetPasswordId,
            };

            A.CallTo(() => userService.GetUserByEmailAddress(emailAddress)).Returns(user);

            // When
            await passwordResetService.GenerateAndSendPasswordResetLink(emailAddress, "example.com");

            // Then
            A.CallTo(() => passwordResetDataService.RemoveResetPasswordAsync(resetPasswordId)).MustHaveHappened();
        }

        [Test]
        public async Task Requesting_password_reset_does_not_attempt_to_clear_previous_hashes_when_they_are_null()
        {
            // Given
            var emailAddress = "recipient@example.com";
            var user = new UserAccount
            {
                ResetPasswordId = null,
            };

            A.CallTo(() => userService.GetUserByEmailAddress(emailAddress)).Returns(user);

            // When
            await passwordResetService.GenerateAndSendPasswordResetLink(emailAddress, "example.com");

            // Then
            A.CallTo(() => passwordResetDataService.RemoveResetPasswordAsync(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Reset_password_is_discounted_if_expired()
        {
            // Given
            var passwordResetDateTime = DateTime.UtcNow;
            var emailAddress = "email";
            var hash = "hash";
            var resetPasswordWithUserDetails = new ResetPasswordWithUserDetails
            {
                PasswordResetDateTime = passwordResetDateTime
            };

            A.CallTo(
                    () => passwordResetDataService.FindMatchingResetPasswordEntityWithUserDetailsAsync(
                        emailAddress,
                        hash
                    )
                )
                .Returns(resetPasswordWithUserDetails);

            GivenCurrentTimeIs(passwordResetDateTime + TimeSpan.FromMinutes(125));

            // When
            var hashIsValid =
                await passwordResetService.EmailAndResetPasswordHashAreValidAsync(
                    emailAddress,
                    hash,
                    ResetPasswordHelpers.ResetPasswordHashExpiryTime
                );

            // Then
            hashIsValid.Should().BeFalse();
        }

        [Test]
        public void GenerateAndSendDelegateWelcomeEmail_with_correct_details_sends_email()
        {
            // Given
            const int delegateId = 2;
            const string emailAddress = "recipient@example.com";
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(delegateId, primaryEmail: emailAddress);

            A.CallTo(() => userService.GetDelegateById(delegateId)).Returns(delegateEntity);

            // When
            passwordResetService.GenerateAndSendDelegateWelcomeEmail(delegateId, "example.com");

            // Then
            A.CallTo(
                    () =>
                        emailService.SendEmail(
                            A<Email>.That.Matches(
                                e =>
                                    e.To[0] == emailAddress &&
                                    e.Cc.IsNullOrEmpty() &&
                                    e.Bcc.IsNullOrEmpty() &&
                                    e.Subject == "Welcome to Digital Learning Solutions - Verify your Registration"
                            )
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public void GenerateAndScheduleDelegateWelcomeEmail_schedules_email_with_correct_candidate_number()
        {
            // Given
            var deliveryDate = new DateTime(2200, 1, 1);
            const string addedByProcess = "some process";
            const int delegateId = 2;
            const string emailAddress = "recipient@example.com";
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(delegateId, primaryEmail: emailAddress);

            A.CallTo(() => userService.GetDelegateById(delegateId)).Returns(delegateEntity);

            // When
            passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                delegateId,
                "example.com",
                deliveryDate,
                addedByProcess
            );

            // Then
            A.CallTo(
                    () =>
                        emailService.ScheduleEmail(
                            A<Email>.That.Matches(
                                e =>
                                    e.To[0] == emailAddress &&
                                    e.Cc.IsNullOrEmpty() &&
                                    e.Bcc.IsNullOrEmpty() &&
                                    e.Subject == "Welcome to Digital Learning Solutions - Verify your Registration"
                            ),
                            addedByProcess,
                            deliveryDate
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public void SendWelcomeEmailsToDelegates_schedules_emails_to_delegates()
        {
            // Given
            var deliveryDate = new DateTime(2200, 1, 1);
            var delegateUsers = Builder<DelegateUser>.CreateListOfSize(3)
                .All().With(user => user.EmailAddress = "recipient@example.com")
                .Build();

            // When
            passwordResetService.SendWelcomeEmailsToDelegates(
                delegateUsers.Select(du => du.Id),
                deliveryDate,
                "example.com"
            );

            // Then
            A.CallTo(
                    () =>
                        emailService.ScheduleEmails(
                            A<IEnumerable<Email>>.That.Matches(list => list.Count() == delegateUsers.Count()),
                            "SendWelcomeEmail_Refactor",
                            deliveryDate
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public async Task ResetPasswordAsync_removes_reset_password_changes_password_and_resets_failed_login_count()
        {
            // Given
            var password = "testPass-9";
            var resetPassword = new ResetPasswordWithUserDetails
            {
                Id = 1,
                UserId = 7,
            };

            // When
            await passwordResetService.ResetPasswordAsync(resetPassword, password);

            // Then
            A.CallTo(() => passwordResetDataService.RemoveResetPasswordAsync(resetPassword.Id)).MustHaveHappened();
            A.CallTo(() => passwordService.ChangePasswordAsync(resetPassword.UserId, password)).MustHaveHappened();
            A.CallTo(() => userService.ResetFailedLoginCountByUserId(resetPassword.UserId)).MustHaveHappened();
        }

        [Test]
        public async Task GetValidPasswordResetEntityAsync_returns_password_reset_entity_if_not_expired()
        {
            // Given
            var passwordResetDateTime = DateTime.UtcNow;
            var expiryTime = ResetPasswordHelpers.ResetPasswordHashExpiryTime;
            var emailAddress = "email";
            var hash = "hash";
            var resetPasswordWithUserDetails = new ResetPasswordWithUserDetails
            {
                PasswordResetDateTime = passwordResetDateTime
            };

            GivenCurrentTimeIs(passwordResetDateTime + expiryTime - TimeSpan.FromMinutes(1));

            A.CallTo(
                    () => passwordResetDataService.FindMatchingResetPasswordEntityWithUserDetailsAsync(
                        emailAddress,
                        hash
                    )
                )
                .Returns(resetPasswordWithUserDetails);

            // When
            var result = await passwordResetService.GetValidPasswordResetEntityAsync(emailAddress, hash, expiryTime);

            // Then
            result.Should().Be(resetPasswordWithUserDetails);
        }

        [Test]
        public async Task GetValidPasswordResetEntityAsync_returns_null_if_password_reset_has_expired()
        {
            // Given
            var passwordResetDateTime = DateTime.UtcNow;
            var expiryTime = ResetPasswordHelpers.ResetPasswordHashExpiryTime;
            var emailAddress = "email";
            var hash = "hash";
            var resetPasswordWithUserDetails = new ResetPasswordWithUserDetails
            {
                PasswordResetDateTime = passwordResetDateTime
            };

            GivenCurrentTimeIs(passwordResetDateTime + expiryTime);

            A.CallTo(
                    () => passwordResetDataService.FindMatchingResetPasswordEntityWithUserDetailsAsync(
                        emailAddress,
                        hash
                    )
                )
                .Returns(resetPasswordWithUserDetails);

            // When
            var result = await passwordResetService.GetValidPasswordResetEntityAsync(emailAddress, hash, expiryTime);

            // Then
            result.Should().Be(null);
        }

        [Test]
        public async Task GetValidPasswordResetEntityAsync_returns_null_if_password_reset_is_not_found()
        {
            // Given
            var expiryTime = ResetPasswordHelpers.ResetPasswordHashExpiryTime;
            var emailAddress = "email";
            var hash = "hash";

            A.CallTo(
                    () => passwordResetDataService.FindMatchingResetPasswordEntityWithUserDetailsAsync(
                        emailAddress,
                        hash
                    )
                )
                .Returns(null as ResetPasswordWithUserDetails);

            // When
            var result = await passwordResetService.GetValidPasswordResetEntityAsync(emailAddress, hash, expiryTime);

            // Then
            result.Should().Be(null);
        }

        private void GivenCurrentTimeIs(DateTime validationTime)
        {
            A.CallTo(() => clockService.UtcNow).Returns(validationTime);
        }
    }
}
