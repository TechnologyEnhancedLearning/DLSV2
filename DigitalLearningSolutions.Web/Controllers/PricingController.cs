namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc;

    [SetApplicationTypeAndSelectedTab(nameof(ApplicationType.Main), nameof(Tab.Pricing))]
    public class PricingController : Controller
    {
        [RedirectDelegateOnlyToLearningPortal]
        public IActionResult Index()
        {
            return View();
        }
    }
}
