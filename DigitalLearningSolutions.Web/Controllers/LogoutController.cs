namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Mvc;

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
            int? adminId = User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);
            if (adminId != null)
            {
                try
                {
                    if(TempData.Peek("AdminSession") != null)
                    {
                        int adminSessionId = int.Parse(TempData.Peek("AdminSessionID").ToString());

                        sessionService.StopAdminSession(adminId.Value, adminSessionId);
                    }
                }
                catch(InvalidCastException ex)
                {
                    Serilog.Log.Error(ex.Message);
                }
               
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
