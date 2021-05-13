namespace DigitalLearningSolutions.Web.Attributes
{
    using System;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DelegateOnlyInaccessibleAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity.IsAuthenticated
                && (user.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated) ?? false)
                && !user.GetAdminId().HasValue)
            {
                context.Result = new RedirectToActionResult("Current", "LearningPortal", new {});
            }
        }
    }
}
