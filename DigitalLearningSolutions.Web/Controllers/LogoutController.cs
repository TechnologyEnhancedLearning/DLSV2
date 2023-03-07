namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class LogoutController : Controller
    {
        private readonly ISessionService sessionService;

        public LogoutController(
            ISessionService sessionService
        )
        {
            this.sessionService = sessionService;
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Index()
        {
            await HttpContext.Logout();
            int adminId = 0;
            try
            {
                adminId = User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);
            }
            catch (ArgumentNullException)
            {
                adminId = 0;
            }
            if (adminId > 0)
            {
                var adminSessionId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AdminSessionID");
                if (adminSessionId != null)
                {
                    sessionService.StopAdminSession(adminId, int.Parse(adminSessionId.Value.ToString()));
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
