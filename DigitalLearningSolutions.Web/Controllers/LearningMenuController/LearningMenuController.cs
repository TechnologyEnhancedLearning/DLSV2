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
            var courseContent = courseContentService.GetCourseContent(User.GetCandidateId(), customisationId);
            var centreId = User.GetCentreId();

            if (courseContent == null || centreId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as course/centre id was not found. " +
                    $"Candidate id: {User.GetCandidateId()}, customisation id: {customisationId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(User.GetCandidateId(), customisationId, centreId.Value);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 500 as no progress id was returned. " +
                    $"Candidate id: {User.GetCandidateId()}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 500 });
            }

            courseContentService.UpdateProgress(progressId.Value);

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
