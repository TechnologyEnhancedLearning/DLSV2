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
        private readonly ISessionService sessionService;
        private readonly ITutorialContentService tutorialContentService;

        public LearningMenuController(
            ILogger<LearningMenuController> logger,
            IConfiguration config,
            ICourseContentService courseContentService,
            ITutorialContentService tutorialContentService,
            ISessionService sessionService
        )
        {
            this.logger = logger;
            this.config = config;
            this.courseContentService = courseContentService;
            this.tutorialContentService = tutorialContentService;
            this.sessionService = sessionService;
        }

        [Route("/LearningMenu/{customisationId:int}")]
        public IActionResult Index(int customisationId)
        {
            var candidateId = User.GetCandidateId();
            var centreId = User.GetCentreId();
            var courseContent = courseContentService.GetCourseContent(candidateId, customisationId);

            sessionService.StartOrUpdateSession(candidateId, customisationId, HttpContext.Session);

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
            sessionService.StopSession(User.GetCandidateId(), HttpContext.Session);

            return RedirectToAction("Current", "LearningPortal");
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}")]
        public IActionResult Section(int customisationId, int sectionId)
        {
            sessionService.StartOrUpdateSession(User.GetCandidateId(), customisationId, HttpContext.Session);
            return View("Section/Section");
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}/{tutorialId:int}")]
        public IActionResult Tutorial(int customisationId, int sectionId, int tutorialId)
        {
            var candidateId = User.GetCandidateId();
            sessionService.StartOrUpdateSession(candidateId, customisationId, HttpContext.Session);

            var tutorialContent =
                tutorialContentService.GetTutorialContent(candidateId, customisationId, sectionId, tutorialId);

            if (tutorialContent == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }
            return View("Tutorial/Tutorial", new TutorialViewModel(tutorialContent, customisationId, sectionId));
        }

        public IActionResult ContentViewer()
        {
            var model = new ContentViewerViewModel(config);
            return View(model);
        }
    }
}
