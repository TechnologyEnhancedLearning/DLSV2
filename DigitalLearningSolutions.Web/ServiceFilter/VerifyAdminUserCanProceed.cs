using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using DigitalLearningSolutions.Data.DataServices;

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

            if (context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserUserAdmin" && c.Value == "True") == null)
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

                if (controller.TempData.Peek("AdminSessionID") != null)
                {
                    var session = sessionDataService.GetAdminSessionById((int)controller.TempData.Peek("AdminSessionID"));
                    if (session.Active)
                    {
                        return;
                    }
                }
                context.Result = new RedirectToActionResult("Index", "Logout", new { });
            }
        }
    }
}
