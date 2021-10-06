using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using System.Net.Mime;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.MiniHub;
    using DigitalLearningSolutions.Web.ViewModels.Home;
    using DocumentFormat.OpenXml.Spreadsheet;
    using Microsoft.Extensions.Configuration;

    [RedirectDelegateOnlyToLearningPortal]
    public class HomeController : Controller
    {
        private const string LandingPageMiniHubName = "Digital Learning Solutions";
        private readonly List<MiniHubSection> sections = new List<MiniHubSection>(
            new[]
            {
                new MiniHubSection(sectionTitle: "Welcome", controllerName: "Home", actionName: "Welcome"),
                new MiniHubSection(sectionTitle: "Products", controllerName: "Home", actionName: "Products"),
                new MiniHubSection(
                    sectionTitle: "Learning content",
                    controllerName: "Home",
                    actionName: "LearningContent"),
            });

        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [SetApplicationTypeAndSelectedTab(nameof(ApplicationType.Main), nameof(Tab.Welcome))]
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
                CurrentSiteBaseUrl = configuration[ConfigHelper.CurrentSystemBaseUrlName],
            };
        }
    }
}
