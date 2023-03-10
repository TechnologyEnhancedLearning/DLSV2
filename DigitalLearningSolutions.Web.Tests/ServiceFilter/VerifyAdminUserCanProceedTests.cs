using DigitalLearningSolutions.Data.DataServices;
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
using System;
using System.Collections.Generic;
using System.Security.Claims;


namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    internal class VerifyAdminUserCanProceedTests
    {
        private readonly ISessionDataService sessionDataService = A.Fake<ISessionDataService>();
        private ActionExecutingContext context = null!;

        [Test]
        public void OnActionExecuting_returns_without_redirect_if_context_controller_not_controller()
        {
            // Given
            context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Object()
            );

            // When
            VerifyAdminUserCanProceed filter = new VerifyAdminUserCanProceed(sessionDataService);
            filter.OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }

        [Test]
        [TestCase("0", true)]
        [TestCase("1234", false)]
        public void OnActionExecuting_returns_without_redirect_if_user_is_not_admin(string userAdminId, bool expectNull)
        {
            // Given
            var homeController = new HomeController(A.Fake<IConfiguration>(), A.Fake<IBrandsService>()).WithDefaultContext().WithMockTempData()
                .WithMockUser(true, 101);
            var httpContext = new DefaultHttpContext();
            Claim[] claims = new Claim[2];
            claims[0] = new Claim("UserAdminID", userAdminId);
            claims[1] = new Claim("UserID", "7");
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
            httpContext.User = new ClaimsPrincipal(claimsIdentity);
            context = new ActionExecutingContext(
                new ActionContext(
                    httpContext,
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
               homeController
            );

            // When
            VerifyAdminUserCanProceed filter = new VerifyAdminUserCanProceed(sessionDataService);
            filter.OnActionExecuting(context);

            // Then
            if (expectNull)
            {
                context.Result.Should().BeNull();
            }
            else
            {
                context.Result.Should().NotBeNull();
                context.Result.Should().Equals(new RedirectToActionResult("Index", "Logout", new { }));
            }
        }

        [Test]
        public void OnActionExecuting_returns_without_redirect_if_request_path_is_logout()
        {
            //Given
            var homeController = new HomeController(A.Fake<IConfiguration>(), A.Fake<IBrandsService>()).WithDefaultContext().WithMockTempData()
               .WithMockUser(true, 101);
            var httpContext = new DefaultHttpContext();
            Claim[] claims = new Claim[2];
            claims[0] = new Claim("UserAdminID", "1234");
            claims[1] = new Claim("UserID", "7");
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
            httpContext.User = new ClaimsPrincipal(claimsIdentity);
            httpContext.Request.Path = "/Logout";
            context = new ActionExecutingContext(
                new ActionContext(
                    httpContext,
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
               homeController
            );

            // When
            VerifyAdminUserCanProceed filter = new VerifyAdminUserCanProceed(sessionDataService);
            filter.OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void OnActionExecuting_returns_without_redirect_if_admin_session_is_active(bool adminSessionActive)
        {
            //Given
            var homeController = new HomeController(A.Fake<IConfiguration>(), A.Fake<IBrandsService>()).WithDefaultContext().WithMockTempData()
               .WithMockUser(true, 101);
            var httpContext = new DefaultHttpContext();
            Claim[] claims = new Claim[3];
            claims[0] = new Claim("UserAdminID", "54321");
            claims[1] = new Claim("UserID", "7");
            claims[2] = new Claim("AdminSessionID", "123456");
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
            httpContext.User = new ClaimsPrincipal(claimsIdentity);
            context = new ActionExecutingContext(
                new ActionContext(
                    httpContext,
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
               homeController
            );

            var adminSession = new Data.Models.AdminSession(123456, 7, DateTime.Now, 0, adminSessionActive);
            A.CallTo(() => sessionDataService.GetAdminSessionById(123456)).Returns(adminSession);

            // When
            VerifyAdminUserCanProceed filter = new VerifyAdminUserCanProceed(sessionDataService);
            filter.OnActionExecuting(context);

            // Then
            if (adminSessionActive)
            {
                context.Result.Should().BeNull();
            }
            else
            {
                Assert.AreEqual(((RedirectToActionResult)context.Result).ActionName, "Index");
                Assert.AreEqual(((RedirectToActionResult)context.Result).ControllerName, "Logout");
            }
        }
    }
}
