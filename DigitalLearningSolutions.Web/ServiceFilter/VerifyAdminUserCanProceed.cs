using DigitalLearningSolutions.Data.DataServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace DigitalLearningSolutions.Web.ServiceFilter
{
    public class VerifyAdminUserCanProceed : ActionFilterAttribute
    {
        private readonly ISessionDataService sessionDataService;

        public VerifyAdminUserCanProceed(ISessionDataService sessionDataService)
        {
            this.sessionDataService = sessionDataService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            // If user does not have an AdminID then they must be a delegate user, so no need to check for concurrency.
            var adminId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserAdminID");
            if (adminId == null || int.Parse(adminId.Value) == 0)
            {
                return;
            }

            var adminUserId = int.Parse(context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserID").Value);

            if (context.HttpContext.Request.Path.Value == "/Logout")
            {
                return;
            }

            if (adminUserId > 0)
            {
                var adminSessionId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AdminSessionID");
                if (adminSessionId != null)
                {
                    var session = sessionDataService.GetAdminSessionById(int.Parse(adminSessionId.Value));
                    if (session != null && session.Active)
                    {
                        return;
                    }
                }
                context.Result = new RedirectToActionResult("Index", "Logout", new { });
            }
        }
    }
}
