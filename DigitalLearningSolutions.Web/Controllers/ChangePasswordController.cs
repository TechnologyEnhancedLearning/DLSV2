namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Mvc;

    public class ChangePasswordController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ChangePasswordViewModel());
        }
        [HttpPost]
        public IActionResult Index(ChangePasswordViewModel model)
        {
            return View("Success");
        }
    }
}
