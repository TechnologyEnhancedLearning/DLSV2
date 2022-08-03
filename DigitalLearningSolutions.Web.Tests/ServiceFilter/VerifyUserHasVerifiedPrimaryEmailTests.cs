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

    public class VerifyUserHasVerifiedPrimaryEmailTests
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
            new VerifyUserHasVerifiedPrimaryEmail(userService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeRedirectToActionResult().WithControllerName("VerifyEmail")
                .WithActionName("VerifyYourEmail")
                .WithRouteValue("emailVerificationReason", EmailVerificationReason.EmailNotVerified);
        }

        [Test]
        public void Does_not_redirect_if_primary_email_is_verified()
        {
            // Given
            var resultListingPrimaryEmailAsVerified = ((string?)null,
                new List<(int centreId, string centreName, string centreEmail)>());

            A.CallTo(() => userService.GetUnverifiedEmailsForUser(UserId))
                .Returns(resultListingPrimaryEmailAsVerified);

            // When
            new VerifyUserHasVerifiedPrimaryEmail(userService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }
    }
}
