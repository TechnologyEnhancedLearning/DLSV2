namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyAdminUserCanAccessDelegateUser : IActionFilter
    {
        private readonly IUserDataService userDataService;

        public VerifyAdminUserCanAccessDelegateUser(IUserDataService userDataService)
        {
            this.userDataService = userDataService;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            var centreId = controller.User.GetCentreId();
            var delegateUserId = int.Parse(context.RouteData.Values["delegateId"].ToString()!);
            var delegateAccount = userDataService.GetDelegateUserById(delegateUserId);

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
