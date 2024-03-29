﻿namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.VerifyEmail;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class VerifyEmailControllerTests
    {
        private const string Email = "email@email.com";
        private const string Code = "code";
        private IClockUtility clockUtility = null!;
        private VerifyEmailController controller = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            userService = A.Fake<IUserService>();
            clockUtility = A.Fake<IClockUtility>();

            controller = new VerifyEmailController(userService, clockUtility)
                .WithDefaultContext();
        }

        [Test]
        [TestCase(null, null)]
        [TestCase(null, "")]
        [TestCase("", null)]
        [TestCase("", "")]
        [TestCase("email", null)]
        [TestCase("email", "")]
        [TestCase(null, "code")]
        [TestCase("", "code")]
        public void Index_with_null_or_empty_email_or_code_returns_NotFound_without_calling_services(
            string? email,
            string? code
        )
        {
            // When
            var result = controller.Index(email, code);

            // Then
            A.CallTo(
                    () => userService.GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails(A<string>._, A<string>._)
                )
                .MustNotHaveHappened();

            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_with_non_matching_email_and_code_returns_ErrorView()
        {
            // Given
            A.CallTo(() => userService.GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails(Email, Code)).Returns(
                null
            );

            // When
            var result = controller.Index(Email, Code);

            // Then
            result.Should().BeViewResult().WithViewName("VerificationLinkError");
        }

        [Test]
        public void Index_with_unverified_email_matching_code_updates_records_and_returns_page()
        {
            // Given
            const int userId = 1;
            var now = new DateTime(2022, 1, 1);

            var verificationData = new EmailVerificationTransactionData(Email, now, null, userId);

            A.CallTo(() => clockUtility.UtcNow).Returns(now);
            A.CallTo(() => userService.GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails(Email, Code))
                .Returns(verificationData);
            A.CallTo(() => userService.SetEmailVerified(userId, Email, now)).DoesNothing();

            // When
            var result = controller.Index(Email, Code);

            // Then
            A.CallTo(() => userService.SetEmailVerified(userId, Email, now)).MustHaveHappenedOnceExactly();

            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void Index_with_unverified_email_for_unapproved_delegate_passes_centre_id_to_the_view()
        {
            // Given
            const int userId = 1;
            const int centreIdForUnapprovedDelegate = 2;
            var now = new DateTime(2022, 1, 1);

            var verificationData = new EmailVerificationTransactionData(
                Email,
                now,
                centreIdForUnapprovedDelegate,
                userId
            );

            A.CallTo(() => clockUtility.UtcNow).Returns(now);
            A.CallTo(() => userService.GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails(Email, Code))
                .Returns(verificationData);
            A.CallTo(() => userService.SetEmailVerified(userId, Email, now)).DoesNothing();

            // When
            var result = controller.Index(Email, Code);

            // Then
            result.Should().BeViewResult().ModelAs<EmailVerifiedViewModel>().CentreIdIfEmailIsForUnapprovedDelegate
                .Should().Be(centreIdForUnapprovedDelegate);
        }
    }
}
