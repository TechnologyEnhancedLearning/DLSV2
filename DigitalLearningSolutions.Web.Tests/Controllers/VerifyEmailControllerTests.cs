namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Common;
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
            A.CallTo(() => userService.GetEmailVerificationDetails(A<string>._, A<string>._)).MustNotHaveHappened();

            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_with_non_matching_email_and_code_returns_NotFound()
        {
            // Given
            A.CallTo(() => userService.GetEmailVerificationDetails(Email, Code)).Returns(null);

            // When
            var result = controller.Index(Email, Code);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_with_verified_email_returns_page_without_updating_records()
        {
            // Given
            var emailVerificationDetails = new EmailVerificationDetails
            {
                UserId = 1,
                Email = Email,
                EmailVerificationHash = Code,
                EmailVerified = new DateTime(2022, 1, 1),
                CentreIdIfEmailIsForUnapprovedDelegate = null,
            };

            A.CallTo(() => userService.GetEmailVerificationDetails(Email, Code)).Returns(emailVerificationDetails);

            // When
            var result = controller.Index(Email, Code);

            // Then
            A.CallTo(() => userService.SetEmailVerified(A<int>._, A<string>._, A<DateTime>._)).MustNotHaveHappened();

            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void Index_with_expired_verification_returns_link_expired_page_without_updating_records()
        {
            // Given
            var now = new DateTime(2022, 1, 1);
            var threeDaysAgo = now.AddDays(-3);

            var emailVerificationDetails = new EmailVerificationDetails
            {
                UserId = 1,
                Email = Email,
                EmailVerificationHash = Code,
                EmailVerified = null,
                EmailVerificationHashCreatedDate = threeDaysAgo,
                CentreIdIfEmailIsForUnapprovedDelegate = null,
            };

            A.CallTo(() => clockUtility.UtcNow).Returns(now);
            A.CallTo(() => userService.GetEmailVerificationDetails(Email, Code)).Returns(emailVerificationDetails);

            // When
            var result = controller.Index(Email, Code);

            // Then
            A.CallTo(() => userService.SetEmailVerified(A<int>._, A<string>._, A<DateTime>._)).MustNotHaveHappened();

            result.Should().BeViewResult().WithViewName("VerificationLinkExpired");
        }

        [Test]
        public void Index_with_unverified_email_matching_code_updates_records_and_returns_page()
        {
            // Given
            const int userId = 1;
            var now = new DateTime(2022, 1, 1);

            var emailVerificationDetails = new EmailVerificationDetails
            {
                UserId = userId,
                Email = Email,
                EmailVerificationHash = Code,
                EmailVerified = null,
                EmailVerificationHashCreatedDate = now,
                CentreIdIfEmailIsForUnapprovedDelegate = null,
            };

            A.CallTo(() => clockUtility.UtcNow).Returns(now);
            A.CallTo(() => userService.GetEmailVerificationDetails(Email, Code)).Returns(emailVerificationDetails);
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

            var emailVerificationDetails = new EmailVerificationDetails
            {
                UserId = userId,
                Email = Email,
                EmailVerificationHash = Code,
                EmailVerified = null,
                EmailVerificationHashCreatedDate = now,
                CentreIdIfEmailIsForUnapprovedDelegate = centreIdForUnapprovedDelegate,
            };

            A.CallTo(() => clockUtility.UtcNow).Returns(now);
            A.CallTo(() => userService.GetEmailVerificationDetails(Email, Code)).Returns(emailVerificationDetails);
            A.CallTo(() => userService.SetEmailVerified(userId, Email, now)).DoesNothing();

            // When
            var result = controller.Index(Email, Code);

            // Then
            result.Should().BeViewResult().ModelAs<int>().IsSameOrEqualTo(centreIdForUnapprovedDelegate);
        }
    }
}
