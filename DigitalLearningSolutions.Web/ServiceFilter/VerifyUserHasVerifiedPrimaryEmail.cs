namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyUserHasVerifiedPrimaryEmail : IActionFilter
    {
        private readonly IUserService userService;

        public VerifyUserHasVerifiedPrimaryEmail(
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
            var (unverifiedPrimaryEmail, _) = userService.GetUnverifiedEmailsForUser(userId);

            if (unverifiedPrimaryEmail != null)
            {
                context.Result = controller.RedirectToAction(
                    "Index",
                    "VerifyYourEmail",
                    new { emailVerificationReason = EmailVerificationReason.EmailNotVerified }
                );
            }
        }
    }
}
