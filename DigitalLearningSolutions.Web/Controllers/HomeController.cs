using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.MiniHub;
    using DigitalLearningSolutions.Web.ViewModels.Home;
    using Microsoft.Extensions.Configuration;

    public class HomeController : Controller
    {
        private readonly List<MiniHubSection> sections = new List<MiniHubSection>(
            new[]
            {
                new MiniHubSection { ControllerName = "Home", ActionName = "Welcome", SectionTitle = "Welcome" },
                new MiniHubSection { ControllerName = "Home", ActionName = "Products", SectionTitle = "Products" },
                new MiniHubSection
                    { ControllerName = "Home", ActionName = "LearningContent", SectionTitle = "Learning Content" },
            });

        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Welcome");
        }

        [HttpGet]
        [Route("Home/Section/Welcome")]
        public IActionResult Welcome()
        {
            return View(GetLandingPageViewModel(0));
        }

        [HttpGet]
        [Route("Home/Section/Products")]
        public IActionResult Products()
        {
            return View(GetLandingPageViewModel(1));
        }

        [HttpGet]
        [Route("Home/Section/LearningContent")]
        public IActionResult LearningContent()
        {
            return View(GetLandingPageViewModel(2));
        }

        private LandingPageViewModel GetLandingPageViewModel(int sectionIndex)
        {
            return new LandingPageViewModel
            {
                MiniHubNavigationModel = new MiniHubNavigationModel
                {
                    Sections = sections,
                    CurrentSectionIndex = sectionIndex,
                },
                UserIsLoggedIn = User.Identity.IsAuthenticated,
                CurrentSiteBaseUrl = configuration[ConfigHelper.CurrentSystemBaseUrlName],
            };
        }
    }
}
