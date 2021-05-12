namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.Attributes;
    using Microsoft.AspNetCore.Mvc;
    
    public class PricingController : Controller
    {
        [DelegateOnlyInaccessible]
        public IActionResult Index()
        {
            return View();
        }
    }
}
