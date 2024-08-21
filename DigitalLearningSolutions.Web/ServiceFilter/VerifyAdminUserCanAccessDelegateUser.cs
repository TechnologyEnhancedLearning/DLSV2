namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyAdminUserCanAccessDelegateUser : IActionFilter
    {
        private readonly IUserService userService;

        public VerifyAdminUserCanAccessDelegateUser(IUserService userService)
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

            var centreId = controller.User.GetCentreIdKnownNotNull();
            var delegateUserId = int.Parse(context.RouteData.Values["delegateId"].ToString()!);
            var delegateAccount = userService.GetDelegateUserById(delegateUserId);

            if (delegateAccount == null)
            {
                context.Result = new NotFoundResult();
            }
            else if (delegateAccount.CentreId != centreId)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "LearningSolutions", new { });
            }
        }
    }
}
