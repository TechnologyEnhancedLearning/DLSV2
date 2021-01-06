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
        private readonly ISectionContentService sectionContentService;
        private readonly ITutorialContentService tutorialContentService;
        private readonly IDiagnosticAssessmentService diagnosticAssessmentService;
        private readonly IPostLearningAssessmentService postLearningAssessmentService;
        private readonly ICourseCompletionService courseCompletionService;

        public LearningMenuController(
            ILogger<LearningMenuController> logger,
            IConfiguration config,
            ICourseContentService courseContentService,
            ISectionContentService sectionContentService,
            ITutorialContentService tutorialContentService,
            IDiagnosticAssessmentService diagnosticAssessmentService,
            IPostLearningAssessmentService postLearningAssessmentService,
            ISessionService sessionService,
            ICourseCompletionService courseCompletionService
        )
        {
            this.logger = logger;
            this.config = config;
            this.courseContentService = courseContentService;
            this.tutorialContentService = tutorialContentService;
            this.sessionService = sessionService;
            this.sectionContentService = sectionContentService;
            this.diagnosticAssessmentService = diagnosticAssessmentService;
            this.postLearningAssessmentService = postLearningAssessmentService;
            this.courseCompletionService = courseCompletionService;
        }

        [Route("/LearningMenu/{customisationId:int}")]
        public IActionResult Index(int customisationId)
        {
            var candidateId = User.GetCandidateId();
            var centreId = User.GetCentreId();
            var courseContent = courseContentService.GetCourseContent(candidateId, customisationId);

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
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateSession(candidateId, customisationId, HttpContext.Session);
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
            var candidateId = User.GetCandidateId();
            var centreId = User.GetCentreId();
            var sectionContent = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            if (sectionContent == null || centreId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as section/centre id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId?.ToString() ?? "null"}, section id: {sectionId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId.Value);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId}, section id: {sectionId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            var model = new SectionContentViewModel(config, sectionContent, customisationId, sectionId);
            return View("Section/Section", model);
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}/Diagnostic")]
        public IActionResult Diagnostic(int customisationId, int sectionId)
        {
            var candidateId = User.GetCandidateId();
            var centreId = User.GetCentreId();
            var diagnosticAssessment =
                diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            if (diagnosticAssessment == null || centreId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as section/centre id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId?.ToString() ?? "null"}, section id: {sectionId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId.Value);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            var model = new DiagnosticAssessmentViewModel(diagnosticAssessment, customisationId, sectionId);
            return View("Diagnostic/Diagnostic", model);
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}/PostLearning")]
        public IActionResult PostLearning(int customisationId, int sectionId)
        {
            var candidateId = User.GetCandidateId();
            var centreId = User.GetCentreId();
            var postLearningAssessment =
                postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            if (postLearningAssessment == null || centreId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as section/centre id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId?.ToString() ?? "null"}, section id: {sectionId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId.Value);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            var model = new PostLearningAssessmentViewModel(postLearningAssessment, customisationId, sectionId);
            return View("PostLearning/PostLearning", model);
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}/{tutorialId:int}")]
        public IActionResult Tutorial(int customisationId, int sectionId, int tutorialId)
        {
            var candidateId = User.GetCandidateId();
            var centreId = User.GetCentreId();

            var tutorialInformation =
                tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            if (tutorialInformation == null || centreId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as customisation/section/tutorial id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId?.ToString() ?? "null"}, section id: {sectionId} tutorial id: {tutorialId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId.Value);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            var viewModel = new TutorialViewModel(config, tutorialInformation, customisationId, sectionId);
            return View("Tutorial/Tutorial", viewModel);
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}/{tutorialId:int}/Tutorial")]
        public IActionResult ContentViewer(int customisationId, int sectionId, int tutorialId)
        {
            var candidateId = User.GetCandidateId();
            var centreId = User.GetCentreId();

            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

            if (tutorialContent?.TutorialPath == null || centreId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as customisation/section/tutorial id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId?.ToString() ?? "null"}, section id: {sectionId} tutorial id: {tutorialId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId.Value);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            var model = new ContentViewerViewModel(
                config,
                tutorialContent,
                customisationId,
                centreId.Value,
                sectionId,
                tutorialId,
                candidateId,
                progressId.Value
            );
            return View("Tutorial/ContentViewer", model);
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}/{tutorialId:int}/Video")]
        public IActionResult TutorialVideo(int customisationId, int sectionId, int tutorialId)
        {
            var candidateId = User.GetCandidateId();
            var centreId = User.GetCentreId();

            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            if (tutorialVideo == null || centreId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as customisation/section/tutorial id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId?.ToString() ?? "null"}, section id: {sectionId} tutorial id: {tutorialId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId.Value);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            var model = new TutorialVideoViewModel(
                config,
                tutorialVideo,
                customisationId,
                sectionId,
                tutorialId
            );
            return View("Tutorial/TutorialVideo", model);
        }

        [Route("/LearningMenu/{customisationId:int}/CompletionSummary")]
        public IActionResult CompletionSummary(int customisationId)
        {
            var candidateId = User.GetCandidateId();
            var centreId = User.GetCentreId();

            var courseCompletion = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

            if (courseCompletion == null || centreId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as customisation id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId?.ToString() ?? "null"}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId.Value);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            var model = new CourseCompletionViewModel(courseCompletion);
            return View("Completion/Completion", model);
        }
    }
}
