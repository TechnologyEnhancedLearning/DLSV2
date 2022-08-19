namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class VerifyYourEmailControllerTests
    {
        private const int UserId = 3;
        private IConfiguration config = null!;
        private VerifyYourEmailController controller = null!;
        private IEmailVerificationService emailVerificationService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            userService = A.Fake<IUserService>();
            emailVerificationService = A.Fake<IEmailVerificationService>();
            config = A.Fake<IConfiguration>();

            controller = new VerifyYourEmailController(userService, emailVerificationService, config)
                .WithDefaultContext().WithMockUser(true, 101, 1, 2, UserId);
        }

        [Test]
        public void ResendVerificationEmails_sends_emails_to_all_unverified_emails_and_returns_view()
        {
            // Given
            const string primaryEmail = "primary@email.com";
            const string centreEmail1 = "centre1@email.com";
            const string centreEmail2 = "centre2@email.com";

            var resultListingPrimaryAndCentreEmailsAsUnverified = (primaryEmail,
                new List<(int centreId, string centreName, string centreEmail)>
                    { (1, "Centre name 1", "centre1@email.com"), (2, "Centre name 2", "centre2@email.com") });
            var defaultUserEntity = UserTestHelper.GetDefaultUserEntity(UserId);

            A.CallTo(() => userService.GetUserById(UserId)).Returns(defaultUserEntity);
            A.CallTo(() => userService.GetUnverifiedEmailsForUser(UserId))
                .Returns(resultListingPrimaryAndCentreEmailsAsUnverified);
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    A<UserAccount>._,
                    A<List<string>>._,
                    A<string>._
                )
            ).DoesNothing();

            // When
            var result = controller.ResendVerificationEmails();

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index").WithRouteValue(
                "emailVerificationReason",
                EmailVerificationReason.EmailNotVerified
            );
            A.CallTo(() => userService.GetUserById(UserId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.GetUnverifiedEmailsForUser(UserId)).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    defaultUserEntity.UserAccount,
                    A<List<string>>.That.Matches(
                        list => ListTestHelper.ListOfStringsMatch(
                            list,
                            new List<string> { primaryEmail, centreEmail1, centreEmail2 }
                        )
                    ),
                    A<string>._
                )
            ).MustHaveHappenedOnceExactly();
        }
    }
}
