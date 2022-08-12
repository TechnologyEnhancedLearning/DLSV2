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
            CreateEmailVerificationHashesAndSendVerificationEmails_updates_hashIds_and_sends_single_email_for_each_unverified_email()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            const int hashId = 1;
            const string newEmail1 = "new1@email.com";
            const string newEmail2 = "new2@email.com";
            A.CallTo(() => emailVerificationDataService.CreateEmailVerificationHash(A<string>._, A<DateTime>._))
                .Returns(hashId);

            // When
            emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                userAccount,
                new List<PossibleEmailUpdate>
                {
                    new PossibleEmailUpdate
                    {
                        OldEmail = "old1@email.com",
                        NewEmail = newEmail1,
                        NewEmailIsVerified = false,
                    },
                    new PossibleEmailUpdate
                    {
                        OldEmail = "old2@email.com",
                        NewEmail = newEmail2,
                        NewEmailIsVerified = false,
                    },
                    new PossibleEmailUpdate
                    {
                        OldEmail = null,
                        NewEmail = newEmail2,
                        NewEmailIsVerified = false,
                    },
                },
                "example.com"
            );

            // Then
            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForPrimaryEmail(
                    userAccount.Id,
                    newEmail1,
                    hashId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForPrimaryEmail(
                    userAccount.Id,
                    newEmail2,
                    hashId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForCentreEmails(
                    userAccount.Id,
                    newEmail1,
                    hashId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForCentreEmails(
                    userAccount.Id,
                    newEmail2,
                    hashId
                )
            ).MustHaveHappenedOnceExactly();

            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustHaveHappenedTwiceExactly();
        }
    }
}
