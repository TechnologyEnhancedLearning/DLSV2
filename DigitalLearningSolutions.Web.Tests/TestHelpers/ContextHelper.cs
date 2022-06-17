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
                new Dictionary<string, object>(),
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

            context.User = new ClaimsPrincipal
            (
                new ClaimsIdentity(
                    new[]
                    {
                        centreId == null ? null : new Claim(CustomClaimTypes.UserCentreId, centreId.ToString()),
                        adminId == null ? null : new Claim(CustomClaimTypes.UserAdminId, adminId.ToString()),
                        delegateId == null ? null : new Claim(CustomClaimTypes.LearnCandidateId, delegateId.ToString()),
                        new Claim(CustomClaimTypes.LearnUserAuthenticated, delegateId != null ? "True" : "False"),
                        emailAddress == null ? null : new Claim(ClaimTypes.Email, emailAddress),
                        new Claim(CustomClaimTypes.UserCentreAdmin, isCentreAdmin.ToString()),
                        new Claim(CustomClaimTypes.IsFrameworkDeveloper, isFrameworkDeveloper.ToString()),
                        new Claim(CustomClaimTypes.AdminCategoryId, adminCategoryId?.ToString()),
                        userId == null ? null : new Claim(CustomClaimTypes.UserId, userId.ToString()),
                    },
                    authenticationType
                )
            );

            return context;
        }
    }
}
