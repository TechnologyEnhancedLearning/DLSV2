namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class EmailVerificationServiceTests
    {
        private IEmailVerificationDataService emailVerificationDataService = null!;
        private IEmailService emailService = null!;
        private IClockUtility clockUtility = null!;
        private EmailVerificationService emailVerificationService = null!;

        [SetUp]
        public void Setup()
        {
            emailVerificationDataService = A.Fake<IEmailVerificationDataService>();
            emailService = A.Fake<IEmailService>();
            clockUtility = A.Fake<IClockUtility>();
            emailVerificationService = new EmailVerificationService(
                emailVerificationDataService,
                emailService,
                clockUtility
            );
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void AccountEmailIsVerifiedForUser_returns_expected_result(bool expectedResult)
        {
            // Given
            const int userId = 2;
            const string email = "test@email.com";
            A.CallTo(() => emailVerificationDataService.AccountEmailIsVerifiedForUser(userId, email))
                .Returns(expectedResult);

            // When
            var result = emailVerificationService.AccountEmailIsVerifiedForUser(userId, email);

            // Then
            result.Should().Be(expectedResult);
        }

        [Test]
        public void
            CreateEmailVerificationHashesAndSendVerificationEmails_does_not_send_emails_if_no_email_requires_verification()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount();

            // When
            emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                userAccount,
                new List<PossibleEmailUpdate>(),
                "example.com"
            );

            // Then
            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForPrimaryEmail(
                    A<int>._,
                    A<string>._,
                    A<int>._
                )
            ).MustNotHaveHappened();

            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForCentreEmails(
                    A<int>._,
                    A<string?>._,
                    A<int>._
                )
            ).MustNotHaveHappened();

            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustNotHaveHappened();
        }

        [Test]
        public void
            CreateEmailVerificationHashesAndSendVerificationEmails_updates_hashId_and_sends_email_when_primary_email_requires_verification()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            const int hashId = 1;
            A.CallTo(() => emailVerificationDataService.CreateEmailVerificationHash(A<string>._, A<DateTime>._))
                .Returns(hashId);

            // When
            emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                userAccount,
                new List<PossibleEmailUpdate>
                {
                    new PossibleEmailUpdate
                    {
                        OldEmail = "old@email.com",
                        NewEmail = "new@email.com",
                        NewEmailIsVerified = false,
                    },
                },
                "example.com"
            );

            // Then
            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForPrimaryEmail(
                    userAccount.Id,
                    "new@email.com",
                    hashId
                )
            ).MustHaveHappenedOnceExactly();

            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void
            CreateEmailVerificationHashesAndSendVerificationEmails_updates_hashIds_and_sends_single_email_when_same_centre_email_requires_verification_at_multiple_centres()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            const int hashId = 1;
            const string email = "centre@email.com";
            A.CallTo(() => emailVerificationDataService.CreateEmailVerificationHash(A<string>._, A<DateTime>._))
                .Returns(hashId);

            // When
            emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                userAccount,
                new List<PossibleEmailUpdate>
                {
                    new PossibleEmailUpdate
                    {
                        OldEmail = "centre1@email.com",
                        NewEmail = email,
                        NewEmailIsVerified = false,
                    },
                    new PossibleEmailUpdate
                    {
                        OldEmail = "centre2@email.com",
                        NewEmail = email,
                        NewEmailIsVerified = false,
                    },
                },
                "example.com"
            );

            // Then
            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForCentreEmails(
                    userAccount.Id,
                    email,
                    hashId
                )
            ).MustHaveHappenedOnceExactly();

            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustHaveHappenedOnceExactly();
        }
    }
}
