namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.MiniHub;
    using DigitalLearningSolutions.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [RedirectDelegateOnlyToLearningPortal]
    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.Welcome))]
    public class HomeController : Controller
    {
        private const string LandingPageMiniHubName = "Digital Learning Solutions";

        private readonly IConfiguration configuration;

        private readonly List<MiniHubSection> sections = new List<MiniHubSection>(
            new[]
            {
                new MiniHubSection("Welcome", "Home", "Welcome"),
                new MiniHubSection("Products", "Home", "Products"),
                new MiniHubSection(
                    "Learning content",
                    "Home",
                    "LearningContent"
                ),
            }
        );

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Welcome");
        }

        [HttpGet]
        public IActionResult Welcome()
        {
            return View(GetLandingPageViewModel(0));
        }

        [HttpGet]
        public IActionResult Products()
        {
            return View(GetLandingPageViewModel(1));
        }

        [HttpGet]
        public IActionResult LearningContent()
        {
            return View(GetLandingPageViewModel(2));
        }

        private LandingPageViewModel GetLandingPageViewModel(int sectionIndex)
        {
            return new LandingPageViewModel
            {
                MiniHubNavigationModel = new MiniHubNavigationModel(LandingPageMiniHubName, sections, sectionIndex),
                UserIsLoggedIn = User.Identity.IsAuthenticated,
                CurrentSiteBaseUrl = configuration.GetCurrentSystemBaseUrl(),
            };
        }
    }
}
