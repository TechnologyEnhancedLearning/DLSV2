namespace DigitalLearningSolutions.Web.Controllers.LearningMenuController
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserOnly)]
    public class LearningMenuController : Controller
    {
        private readonly ILogger<LearningMenuController> logger;
        private readonly IConfiguration config;
        private readonly ICourseContentService courseContentService;
        private readonly ISessionService sessionService;

        public LearningMenuController(
            ILogger<LearningMenuController> logger,
            IConfiguration config,
            ICourseContentService courseContentService,
            ISessionService sessionService
        )
        {
            this.logger = logger;
            this.config = config;
            this.courseContentService = courseContentService;
            this.sessionService = sessionService;
        }

        [Route("/LearningMenu/{customisationId:int}")]
        public IActionResult Index(int customisationId)
        {
            var candidateId = User.GetCandidateId();
            var centreId = User.GetCentreId();
            var courseContent = courseContentService.GetCourseContent(candidateId, customisationId);

            StartOrUpdateSession(candidateId, customisationId);

            if (courseContent == null || centreId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as course/centre id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId?.ToString() ?? "null"}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId.Value);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 500 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 500 });
            }

            courseContentService.UpdateProgress(progressId.Value);

            var model = new InitialMenuViewModel(courseContent);
            return View(model);
        }

        [Route("/LearningMenu/Close")]
        public IActionResult Close()
        {
            sessionService.StopSession(User.GetCandidateId());
            HttpContext.Session.Clear();

            return RedirectToAction("Current", "LearningPortal");
        }

        public IActionResult ContentViewer()
        {
            var model = new ContentViewerViewModel(config);
            return View(model);
        }

        private void StartOrUpdateSession(int candidateId, int customisationId)
        {
            var currentSessionId = HttpContext.Session.GetInt32($"SessionID-{customisationId}");
            if (currentSessionId != null)
            {
                sessionService.UpdateSessionDuration(currentSessionId.Value);
            }
            else
            {
                // Clear all session variables
                HttpContext.Session.Clear();

                // Make and keep track of a new session starting at this request
                var newSessionId = sessionService.StartOrRestartSession(candidateId, customisationId);
                HttpContext.Session.SetInt32($"SessionID-{customisationId}", newSessionId);
            }
        }
    }
}
