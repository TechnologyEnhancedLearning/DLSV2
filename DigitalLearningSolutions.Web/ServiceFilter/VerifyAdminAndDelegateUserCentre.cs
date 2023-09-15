namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyAdminAndDelegateUserCentre : IActionFilter
    {
        private readonly IUserDataService userDataService;

        public VerifyAdminAndDelegateUserCentre(IUserDataService userDataService)
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

            var centreId = controller.User.GetCentreIdKnownNotNull();
            var delegateUserId = int.Parse(context.RouteData.Values["delegateId"].ToString()!);
            var delegateAccount = userDataService.GetDelegateUserById(delegateUserId);

            if (delegateAccount == null)
            {
                context.Result = new RedirectToActionResult("StatusCode", "LearningSolutions", new { code = 410 });

            }
            else if (delegateAccount != null && delegateAccount.CentreId != centreId)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "LearningSolutions", new { });
            }
        }
    }
}
