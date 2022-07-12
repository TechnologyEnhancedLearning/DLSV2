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
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Auth;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Services;
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
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            userService = A.Fake<IUserService>();
            emailService = A.Fake<IEmailService>();
            clockService = A.Fake<IClockService>();
            passwordResetDataService = A.Fake<IPasswordResetDataService>();

            A.CallTo(() => userService.GetUsersByEmailAddress(A<string>._)).Returns
            (
                (
                    UserTestHelper.GetDefaultAdminUser(),
                    new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() }
                )
            );

            passwordResetService = new PasswordResetService(
                userService,
                passwordResetDataService,
                emailService,
                clockService
            );
        }

        [Test]
        public void Trying_get_null_user_should_throw_an_exception()
        {
            // Given
            A.CallTo(() => userService.GetUsersByEmailAddress(A<string>._)).Returns((null, new List<DelegateUser>()));

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
            var adminUser = Builder<AdminUser>.CreateNew()
                .With(user => user.EmailAddress = emailAddress)
                .Build();

            A.CallTo(() => userService.GetUsersByEmailAddress(emailAddress))
                .Returns((adminUser, new List<DelegateUser>()));

            // When
            await passwordResetService.GenerateAndSendPasswordResetLink(emailAddress, "example.com");

            // Then
            A.CallTo(
                    () =>
                        emailService.SendEmail(
                            A<Email>.That.Matches(
                                e =>
                                    e.To[0] == emailAddress &&
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
            var adminUser = Builder<AdminUser>.CreateNew()
                .With(user => user.ResetPasswordId = resetPasswordId)
                .Build();

            A.CallTo(() => userService.GetUsersByEmailAddress(emailAddress))
                .Returns((adminUser, new List<DelegateUser>()));

            // When
            await passwordResetService.GenerateAndSendPasswordResetLink(emailAddress, "example.com");

            // Then
            A.CallTo(
                    () =>
                        passwordResetDataService.RemoveResetPasswordAsync(resetPasswordId)
                )
                .MustHaveHappened();
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
                        hash
                    )
                )
                .Returns(Task.FromResult(new[] { resetPasswordWithUserDetails }.ToList()));

            GivenCurrentTimeIs(createTime + TimeSpan.FromMinutes(125));

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
                        resetHash
                    )
                )
                .Returns(Task.FromResult(resetPasswords));

            GivenCurrentTimeIs(createTime + TimeSpan.FromMinutes(2));

            // When
            var hashIsValid =
                await passwordResetService.EmailAndResetPasswordHashAreValidAsync(
                    emailAddress,
                    resetHash,
                    ResetPasswordHelpers.ResetPasswordHashExpiryTime
                );

            // Then
            hashIsValid.Should().BeTrue();
        }

        [Test]
        public async Task InvalidateResetPasswordForEmailAsync_removes_reset_password_for_exact_users_matching_email()
        {
            // Given
            var users = (
                Builder<AdminUser>.CreateNew().With(u => u.ResetPasswordId = 1).Build(),
                new[] { Builder<DelegateUser>.CreateNew().With(u => u.ResetPasswordId = 4).Build() }.ToList());
            A.CallTo(() => userService.GetUsersByEmailAddress("email")).Returns(users);

            // When
            await passwordResetService.InvalidateResetPasswordForEmailAsync("email");

            // Then
            A.CallTo(() => passwordResetDataService.RemoveResetPasswordAsync(1))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordResetDataService.RemoveResetPasswordAsync(4))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordResetDataService.RemoveResetPasswordAsync(A<int>._))
                .WhenArgumentsMatch(args => args.Get<int>(0) != 1 && args.Get<int>(0) != 4).MustNotHaveHappened();
        }

        [Test]
        public void GenerateAndSendDelegateWelcomeEmail_with_null_user_should_throw_an_exception()
        {
            // Given
            A.CallTo(() => userService.GetUsersByEmailAddress(A<string>._)).Returns((null, new List<DelegateUser>()));

            // Then
            Assert.Throws<UserAccountNotFoundException>(
                () => passwordResetService.GenerateAndSendDelegateWelcomeEmail(
                    "recipient@example.com",
                    "A1",
                    "example.com"
                )
            );
        }

        [Test]
        public void GenerateAndSendDelegateWelcomeEmail_with_incorrect_candidate_number_should_throw_an_exception()
        {
            // Given
            const string emailAddress = "recipient@example.com";
            const string candidateNumber = "A1";
            var delegateUser = Builder<DelegateUser>.CreateNew()
                .With(user => user.EmailAddress = emailAddress)
                .With(user => user.CandidateNumber = candidateNumber)
                .Build();
            A.CallTo(() => userService.GetDelegateUsersByEmailAddress(emailAddress))
                .Returns(new List<DelegateUser> { delegateUser });

            // Then
            Assert.Throws<UserAccountNotFoundException>(
                () => passwordResetService.GenerateAndSendDelegateWelcomeEmail(
                    "recipient@example.com",
                    "IncorrectCandidateNumber",
                    "example.com"
                )
            );
        }

        [Test]
        public void GenerateAndSendDelegateWelcomeEmail_with_correct_details_sends_email()
        {
            // Given
            const string emailAddress = "recipient@example.com";
            const string candidateNumber = "A1";
            var delegateUser = Builder<DelegateUser>.CreateNew()
                .With(user => user.EmailAddress = emailAddress)
                .With(user => user.CandidateNumber = candidateNumber)
                .Build();

            A.CallTo(() => userService.GetDelegateUsersByEmailAddress(emailAddress))
                .Returns(new List<DelegateUser> { delegateUser });

            // When
            passwordResetService.GenerateAndSendDelegateWelcomeEmail(emailAddress, candidateNumber, "example.com");

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
            const string emailAddress = "recipient@example.com";
            const string addedByProcess = "some process";
            const string candidateNumber = "A1";
            var delegateUser = Builder<DelegateUser>.CreateNew()
                .With(user => user.EmailAddress = emailAddress)
                .With(user => user.CandidateNumber = candidateNumber)
                .Build();

            A.CallTo(() => userService.GetDelegateUsersByEmailAddress(emailAddress))
                .Returns(new List<DelegateUser> { delegateUser });

            // When
            passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                emailAddress,
                candidateNumber,
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
        public void GenerateAndScheduleDelegateWelcomeEmail_throws_exception_with_incorrect_candidate_number()
        {
            // Given
            var deliveryDate = new DateTime(2200, 1, 1);
            const string emailAddress = "recipient@example.com";
            const string addedByProcess = "some process";
            const string candidateNumber = "A1";
            var delegateUser = Builder<DelegateUser>.CreateNew()
                .With(user => user.EmailAddress = emailAddress)
                .With(user => user.CandidateNumber = candidateNumber)
                .Build();

            A.CallTo(() => userService.GetDelegateUsersByEmailAddress(emailAddress))
                .Returns(new List<DelegateUser> { delegateUser });

            // Then
            Assert.Throws<UserAccountNotFoundException>(
                () =>
                    passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                        emailAddress,
                        "IncorrectCandidateNumber",
                        "example.com",
                        deliveryDate,
                        addedByProcess
                    )
            );
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
                delegateUsers,
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

        private void GivenCurrentTimeIs(DateTime validationTime)
        {
            A.CallTo(() => clockService.UtcNow).Returns(validationTime);
        }
    }
}
