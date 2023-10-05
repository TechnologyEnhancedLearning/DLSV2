namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;

    public static class ContextHelper
    {
        public const int CentreId = 2;
        public const int AdminId = 7;
        public const int DelegateId = 2;
        public const int UserId = 2;
        public const string EmailAddress = "email";
        public const bool IsCentreAdmin = false;
        public const bool IsFrameworkDeveloper = false;
        public const int AdminCategoryId = 0;

        public static ActionExecutingContext GetDefaultActionExecutingContext(Controller controller)
        {
            return new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object?>(),
                controller
            );
        }

        public static ActionExecutingContext WithMockUser(
            this ActionExecutingContext context,
            bool isAuthenticated,
            int? centreId = CentreId,
            int? adminId = AdminId,
            int? delegateId = DelegateId,
            int? userId = UserId,
            string? emailAddress = EmailAddress,
            bool isCentreAdmin = IsCentreAdmin,
            bool isFrameworkDeveloper = IsFrameworkDeveloper,
            int? adminCategoryId = AdminCategoryId
        )
        {
            context.HttpContext.WithMockUser(
                isAuthenticated,
                centreId,
                adminId,
                delegateId,
                userId,
                emailAddress,
                isCentreAdmin,
                isFrameworkDeveloper,
                adminCategoryId
            );

            return context;
        }

        public static HttpContext WithMockUser(
            this HttpContext context,
            bool isAuthenticated,
            int? centreId = CentreId,
            int? adminId = AdminId,
            int? delegateId = DelegateId,
            int? userId = UserId,
            string? emailAddress = EmailAddress,
            bool isCentreAdmin = IsCentreAdmin,
            bool isFrameworkDeveloper = IsFrameworkDeveloper,
            int? adminCategoryId = AdminCategoryId
        )
        {
            var authenticationType = isAuthenticated ? "mock" : string.Empty;

            var claims = new List<Claim>();

            if (centreId != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserCentreId, centreId.ToString()!));
            }

            if (adminId != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserAdminId, adminId.ToString()!));
            }

            if (delegateId != null)
            {
                claims.Add(new Claim(CustomClaimTypes.LearnCandidateId, delegateId.ToString()!));
                claims.Add(new Claim(CustomClaimTypes.LearnUserAuthenticated, "True"));
            }
            else
            {
                claims.Add(new Claim(CustomClaimTypes.LearnUserAuthenticated, "False"));
            }

            if (emailAddress != null)
            {
                claims.Add(new Claim(ClaimTypes.Email, emailAddress));
            }

            claims.Add(new Claim(CustomClaimTypes.UserCentreAdmin, isCentreAdmin.ToString()));
            claims.Add(new Claim(CustomClaimTypes.IsFrameworkDeveloper, isFrameworkDeveloper.ToString()));

            if (adminCategoryId != null)
            {
                claims.Add(new Claim(CustomClaimTypes.AdminCategoryId, adminCategoryId.ToString()!));
            }

            if (userId != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserId, userId.ToString()!));
            }

            context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType));

            return context;
        }
    }
}
