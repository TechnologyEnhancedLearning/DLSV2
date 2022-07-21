namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyUserHasVerifiedNecessaryEmails : IActionFilter
    {
        private readonly IUserService userService;

        public VerifyUserHasVerifiedNecessaryEmails(
            IUserService userService
        )
        {
            this.userService = userService;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            var userId = controller.User.GetUserIdKnownNotNull();
            var (unverifiedPrimaryEmail, unverifiedCentreEmails) = userService.GetUnverifiedEmailsForUser(userId);

            if (unverifiedPrimaryEmail != null)
            {
                RedirectToVerifyEmailPage(context);
            }

            if (context.RouteData.Values["action"]?.ToString() != "ChooseCentre")
            {
                return;
            }

            var centreId = int.Parse(context.ActionArguments["centreId"].ToString()!);
            var centreEmailIsUnverified = unverifiedCentreEmails.Select(uce => uce.centreId).Contains(centreId);

            if (centreEmailIsUnverified)
            {
                RedirectToVerifyEmailPage(context);
            }
        }

        private static void RedirectToVerifyEmailPage(ActionExecutingContext context)
        {
            context.Result = new RedirectToActionResult(
                "Index",
                "VerifyEmail",
                new { emailVerificationReason = EmailVerificationReason.EmailNotVerified }
            );
        }
    }
}
