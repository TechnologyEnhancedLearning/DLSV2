namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Mvc;

    public class NotificationPreferencesController : Controller
    {
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            var adminId = User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId);
            var delegateId = User.GetCustomClaimAsInt(CustomClaimTypes.LearnCandidateId);

            var model = new NotificationPreferencesViewModel(adminId, delegateId);

            return View(model);
        }
    }
}
