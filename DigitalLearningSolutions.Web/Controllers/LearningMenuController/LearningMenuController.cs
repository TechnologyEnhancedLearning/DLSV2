namespace DigitalLearningSolutions.Web.Controllers.LearningMenuController
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserOnly)]
    public class LearningMenuController : Controller
    {
        private readonly ILogger<LearningMenuController> logger;
        private readonly IConfiguration config;
        private readonly ICourseContentService courseContentService;

        public LearningMenuController(
            ILogger<LearningMenuController> logger,
            IConfiguration config,
            ICourseContentService courseContentService
        )
        {
            this.logger = logger;
            this.config = config;
            this.courseContentService = courseContentService;
        }

        [Route("/LearningMenu/{customisationId:int}")]
        public IActionResult Index(int customisationId)
        {
            var courseContent = courseContentService.GetCourseContent(customisationId);

            if (courseContent == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var model = new InitialMenuViewModel(courseContent);
            return View(model);
        }

        public IActionResult ContentViewer()
        {
            var model = new ContentViewerViewModel(config);
            return View(model);
        }
    }
}
