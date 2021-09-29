﻿namespace DigitalLearningSolutions.Web.Tests.ControllerHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Helpers;
    using FakeItEasy;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static class ControllerContextHelper
    {
        public const int CentreId = 2;
        public const int AdminId = 7;
        public const int DelegateId = 2;
        public const string EmailAddress = "email";
        public const bool IsCentreAdmin = false;
        public const bool IsFrameworkDeveloper = false;
        public const int AdminCategoryId = 0;

        public static T WithDefaultContext<T>(this T controller) where T : Controller
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            return controller;
        }

        public static T WithMockHttpContext<T>(
            this T controller,
            HttpRequest request,
            string? cookieName = null,
            string? cookieValue = null,
            HttpResponse? response = null
        ) where T : Controller
        {
            var httpContext = A.Fake<HttpContext>();

            var cookieCollection = cookieName == null || cookieValue == null
                ? A.Fake<IRequestCookieCollection>()
                : SetUpFakeRequestCookieCollection(cookieName, cookieValue);

            A.CallTo(() => request.Cookies).Returns(cookieCollection);
            A.CallTo(() => httpContext.Request).Returns(request);

            if (response != null)
            {
                A.CallTo(() => httpContext.Response).Returns(response);
            }

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }

        public static IRequestCookieCollection SetUpFakeRequestCookieCollection(
            string cookieName,
            string cookieValue
        )
        {
            var cookieCollection = A.Fake<IRequestCookieCollection>();

            var cookieList = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(cookieName, cookieValue)
                };

            A.CallTo(() => cookieCollection[cookieName]).Returns(cookieValue);
            A.CallTo(() => cookieCollection.GetEnumerator()).Returns(cookieList.GetEnumerator());
            A.CallTo(() => cookieCollection.ContainsKey(cookieName)).Returns(true);

            return cookieCollection;
        }

        public static T WithMockUser<T>(
            this T controller,
            bool isAuthenticated,
            int centreId = CentreId,
            int? adminId = AdminId,
            int? delegateId = DelegateId,
            string? emailAddress = EmailAddress,
            bool isCentreAdmin = IsCentreAdmin,
            bool isFrameworkDeveloper = IsFrameworkDeveloper,
            int adminCategoryId = AdminCategoryId
        ) where T : Controller
        {
            var authenticationType = isAuthenticated ? "mock" : string.Empty;

            controller.HttpContext.User = new ClaimsPrincipal
            (
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(CustomClaimTypes.UserCentreId, centreId.ToString()),
                        new Claim(CustomClaimTypes.UserAdminId, adminId?.ToString() ?? "False"),
                        new Claim(CustomClaimTypes.LearnCandidateId, delegateId?.ToString() ?? "False"),
                        new Claim(ClaimTypes.Email, emailAddress ?? string.Empty),
                        new Claim(CustomClaimTypes.UserCentreAdmin, isCentreAdmin.ToString()),
                        new Claim(CustomClaimTypes.IsFrameworkDeveloper, isFrameworkDeveloper.ToString()),
                        new Claim(CustomClaimTypes.AdminCategoryId, adminCategoryId.ToString())
                    },
                    authenticationType
                )
            );

            return controller;
        }

        public static T WithMockTempData<T>(this T controller) where T : Controller
        {
            controller.TempData = new TempDataDictionary(controller.HttpContext, A.Fake<ITempDataProvider>());
            return controller;
        }

        public static T WithMockServices<T>(this T controller) where T : Controller
        {
            var authService = A.Fake<IAuthenticationService>();
            A.CallTo
            (
                () => authService.SignInAsync(
                    A<HttpContext>._,
                    A<string>._,
                    A<ClaimsPrincipal>._,
                    A<AuthenticationProperties>._
                )
            ).Returns(Task.CompletedTask);
            A.CallTo
            (
                () => authService.SignOutAsync(
                    A<HttpContext>._,
                    A<string>._,
                    A<AuthenticationProperties>._
                )
            ).Returns(Task.CompletedTask);

            var urlHelperFactory = A.Fake<IUrlHelperFactory>();
            var services = A.Fake<IServiceProvider>();
            A.CallTo(() => services.GetService(typeof(IAuthenticationService))).Returns(authService);
            A.CallTo(() => services.GetService(typeof(IUrlHelperFactory))).Returns(urlHelperFactory);

            controller.HttpContext.RequestServices = services;
            return controller;
        }

        public static T WithMockRequest<T>(this T controller, HttpRequest request) where T : Controller
        {
            var httpContext = A.Fake<HttpContext>();
            A.CallTo(() => httpContext.Request).Returns(request);
            
            return controller;
        }
    }
}
