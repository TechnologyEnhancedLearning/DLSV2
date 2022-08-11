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
        public void AccountEmailIsVerifiedForUser_Returns_Expected_Result(bool expectedResult)
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
            CreateEmailVerificationHashesAndSendVerificationEmails_Does_Not_Send_Emails_If_No_Email_Requires_Verification()
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
            CreateEmailVerificationHashesAndSendVerificationEmails_Updates_HashId_And_Sends_Email_When_Primary_Email_Requires_Verification()
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
            CreateEmailVerificationHashesAndSendVerificationEmails_Updates_HashIds_And_Sends_Single_Email_When_Same_Centre_Email_Requires_Verification_At_Multiple_Centres()
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
