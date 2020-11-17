namespace DigitalLearningSolutions.Web.Controllers.LearningMenuController
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserOnly)]
    public class LearningMenuController : Controller
    {
        private readonly ILogger<LearningMenuController> logger;
        private readonly IConfiguration config;

        public LearningMenuController(ILogger<LearningMenuController> logger, IConfiguration config)
        {
            this.logger = logger;
            this.config = config;
        }

        [Route("/LearningMenu/{customisationId:int}")]
        public IActionResult Index(int customisationId)
        {
            var model = new InitialMenuViewModel(customisationId);
            return View(model);
        }

        public IActionResult ContentViewer()
        {
            var model = new ContentViewerViewModel(config);
            return View(model);
        }
    }
}
