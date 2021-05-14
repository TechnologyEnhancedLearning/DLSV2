namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.Attributes;
    using Microsoft.AspNetCore.Mvc;

    public class FindYourCentreController : Controller
    {
        [RedirectDelegateOnlyToLearningPortal]
        public IActionResult Index()
        {
            return View();
        }
    }
}
