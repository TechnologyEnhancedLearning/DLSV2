namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
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
        public void AccountEmailRequiresVerification_Returns_Expected_Result(bool expectedResult)
        {
            // Given
            const int userId = 2;
            const string email = "test@email.com";
            A.CallTo(() => emailVerificationDataService.AccountEmailRequiresVerification(userId, email))
                .Returns(expectedResult);

            // When
            var result = emailVerificationService.AccountEmailRequiresVerification(userId, email);

            // Then
            result.Should().Be(expectedResult);
        }

        [Test]
        public void SendVerificationEmails_Does_Not_Send_Emails_If_No_Email_Requires_Verification()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount();

            // When
            emailVerificationService.SendVerificationEmails(userAccount, new List<(string, int?)>(), "example.com");

            // Then
            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForPrimaryEmail(A<int>._, A<int>._)
            ).MustNotHaveHappened();

            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForCentreEmail(
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();

            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustNotHaveHappened();
        }

        [Test]
        public void SendVerificationEmails_Updates_HashId_And_Sends_Email_When_Primary_Email_Requires_Verification()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            const int hashId = 1;
            A.CallTo(() => emailVerificationDataService.CreateEmailVerificationHash(A<string>._, A<DateTime>._))
                .Returns(hashId);

            // When
            emailVerificationService.SendVerificationEmails(
                userAccount,
                new List<(string, int?)> { ("primary@email.com", null) },
                "example.com"
            );

            // Then
            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForPrimaryEmail(userAccount.Id, hashId)
            ).MustHaveHappenedOnceExactly();

            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForCentreEmail(
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();

            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void
            SendVerificationEmails_Updates_HashIds_And_Sends_Single_Email_When_Same_Centre_Email_Requires_Verification_At_Multiple_Centres()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            const int hashId = 1;
            const string email = "centre@email.com";
            A.CallTo(() => emailVerificationDataService.CreateEmailVerificationHash(A<string>._, A<DateTime>._))
                .Returns(hashId);

            // When
            emailVerificationService.SendVerificationEmails(
                userAccount,
                new List<(string, int?)> { (email, 1), (email, 2) },
                "example.com"
            );

            // Then
            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForPrimaryEmail(userAccount.Id, hashId)
            ).MustNotHaveHappened();

            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForCentreEmail(
                    userAccount.Id,
                    1,
                    hashId
                )
            ).MustHaveHappenedOnceExactly();

            A.CallTo(
                () => emailVerificationDataService.UpdateEmailVerificationHashIdForCentreEmail(
                    userAccount.Id,
                    2,
                    hashId
                )
            ).MustHaveHappenedOnceExactly();

            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustHaveHappenedOnceExactly();
        }
    }
}
