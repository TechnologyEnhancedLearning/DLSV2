namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.Pricing))]
    public class PricingController : Controller
    {
        private readonly IConfiguration configuration;

        public PricingController(
            IConfiguration configuration
        )
        {
            this.configuration = configuration;
        }

        [RedirectDelegateOnlyToLearningPortal]
        public IActionResult Index()
        {
            if (!configuration.IsPricingPageUsed())
            {
                return NotFound();
            }

            return View();
        }
    }
}
