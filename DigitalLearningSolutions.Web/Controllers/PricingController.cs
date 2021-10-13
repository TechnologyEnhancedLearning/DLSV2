namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc;

    [SetApplicationType(nameof(ApplicationType.Main))]
    [SetSelectedTab(nameof(NavMenuTab.Pricing))]
    public class PricingController : Controller
    {
        [RedirectDelegateOnlyToLearningPortal]
        public IActionResult Index()
        {
            return View();
        }
    }
}
