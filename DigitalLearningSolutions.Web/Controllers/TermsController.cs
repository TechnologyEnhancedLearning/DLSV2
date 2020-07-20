namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.ViewModels.Terms;
    using Microsoft.AspNetCore.Mvc;

    public class TermsController : Controller
    {
        public IActionResult Index()
        {
            var model = new TermsViewModel();
            return View(model);
        }
    }
}
