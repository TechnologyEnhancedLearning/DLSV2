namespace DigitalLearningSolutions.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("Pricing")]
    public class PricingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
