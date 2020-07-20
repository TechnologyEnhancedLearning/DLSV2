namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.ViewModels.Accessibility;
    using Microsoft.AspNetCore.Mvc;

    public class AccessibilityController : Controller
    {
        public IActionResult Index()
        {
            var model = new AccessibilityViewModel();
            return View(model);
        }
    }
}
