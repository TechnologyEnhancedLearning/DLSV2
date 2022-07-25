namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class VerifyUserHasVerifiedNecessaryEmailsTests
    {
        private const int UserId = 2;
        private const int CentreId = 101;
        private readonly IUserService userService = A.Fake<IUserService>();
        private ActionExecutingContext context = null!;

        [SetUp]
        public void Setup()
        {
            var homeController = new HomeController(A.Fake<IConfiguration>(), A.Fake<IBrandsService>())
                .WithDefaultContext().WithMockTempData()
                .WithMockUser(true, CentreId);
            context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                homeController
            );
        }

        [Test]
        public void Redirects_to_verify_email_page_if_primary_email_is_unverified()
        {
            // Given
            var resultListingPrimaryEmailAsUnverified = ("primary@email.com",
                new List<(int centreId, string centreName, string centreEmail)>());

            A.CallTo(() => userService.GetUnverifiedEmailsForUser(UserId))
                .Returns(resultListingPrimaryEmailAsUnverified);

            // When
            new VerifyUserHasVerifiedNecessaryEmails(userService).OnActionExecuting(context);

            // Then
            ResultShouldBeRedirectToVerifyEmailPage();
        }

        [Test]
        public void Does_not_redirect_if_no_emails_are_unverified()
        {
            // Given
            var resultListingNoEmailsAsUnverified = ((string?)null,
                new List<(int centreId, string centreName, string centreEmail)>());

            A.CallTo(() => userService.GetUnverifiedEmailsForUser(UserId))
                .Returns(resultListingNoEmailsAsUnverified);

            // When
            new VerifyUserHasVerifiedNecessaryEmails(userService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }

        [Test]
        public void
            Does_not_redirect_if_primary_email_is_verified_and_user_has_unverified_centre_email_but_is_not_logging_into_a_centre_directly()
        {
            // Given
            var resultListingCentreEmailAsUnverified = ((string?)null,
                new List<(int centreId, string centreName, string centreEmail)>
                    { (CentreId, "Test Centre", "centre@email.com") });

            A.CallTo(() => userService.GetUnverifiedEmailsForUser(UserId))
                .Returns(resultListingCentreEmailAsUnverified);

            // When
            new VerifyUserHasVerifiedNecessaryEmails(userService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }

        [Test]
        public void
            Redirects_to_verify_email_page_if_centre_email_is_unverified_at_centre_user_has_logged_into_directly()
        {
            // Given
            SetUpContextForLoggingIntoSingleCentre();

            var resultListingCentreEmailAsUnverified = ((string?)null,
                new List<(int centreId, string centreName, string centreEmail)>
                    { (CentreId, "Test Centre", "centre@email.com") });

            A.CallTo(() => userService.GetUnverifiedEmailsForUser(UserId))
                .Returns(resultListingCentreEmailAsUnverified);

            // When
            new VerifyUserHasVerifiedNecessaryEmails(userService).OnActionExecuting(context);

            // Then
            ResultShouldBeRedirectToVerifyEmailPage();
        }

        [Test]
        public void
            Does_not_redirect_because_centre_email_is_unverified_at_centre_other_than_the_one_user_has_logged_into_directly()
        {
            // Given
            SetUpContextForLoggingIntoSingleCentre();

            var resultListingCentreEmailAsUnverifiedAtDifferentCentre = ((string?)null,
                new List<(int centreId, string centreName, string centreEmail)>
                    { (CentreId + 1, "Test Centre", "centre@email.com") });

            A.CallTo(() => userService.GetUnverifiedEmailsForUser(UserId))
                .Returns(resultListingCentreEmailAsUnverifiedAtDifferentCentre);

            // When
            new VerifyUserHasVerifiedNecessaryEmails(userService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }

        private void ResultShouldBeRedirectToVerifyEmailPage()
        {
            context.Result.Should().BeRedirectToActionResult().WithControllerName("VerifyEmail").WithActionName("Index")
                .WithRouteValue("emailVerificationReason", EmailVerificationReason.EmailNotVerified);
        }

        private void SetUpContextForLoggingIntoSingleCentre()
        {
            context.RouteData.Values["action"] = "ChooseCentre";
            context.ActionArguments["centreId"] = CentreId;
        }
    }
}
