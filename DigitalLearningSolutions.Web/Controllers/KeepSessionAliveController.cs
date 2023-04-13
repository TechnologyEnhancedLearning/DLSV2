using DigitalLearningSolutions.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningSolutions.Web.Controllers
{
    public class KeepSessionAliveController : Controller
    {
      public JsonResult Ping()
      {
        return Json("Success");
      }
    }
}
