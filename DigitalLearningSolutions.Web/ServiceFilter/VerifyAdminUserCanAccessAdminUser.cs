namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyAdminUserCanAccessAdminUser : IActionFilter
    {
        private readonly IUserDataService userDataService;

        public VerifyAdminUserCanAccessAdminUser(IUserDataService userDataService)
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
            var adminUserId = int.Parse(context.RouteData.Values["adminId"].ToString()!);
            var adminAccount = userDataService.GetAdminUserById(adminUserId);

            if (adminAccount == null)
            {
                context.Result = new NotFoundResult();
            }
            else if (adminAccount.CentreId != centreId)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "LearningSolutions", new { });
            }
        }
    }
}
