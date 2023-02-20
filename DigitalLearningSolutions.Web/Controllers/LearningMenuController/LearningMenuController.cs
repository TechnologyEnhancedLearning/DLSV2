namespace DigitalLearningSolutions.Web.Controllers.LearningMenuController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserDelegateOnly)]
    public class LearningMenuController : Controller
    {
        private const int MinimumTutorialAverageTimeToIncreaseAuthExpiry = 45;
        private readonly ILogger<LearningMenuController> logger;
        private readonly IConfiguration config;
        private readonly IConfigDataService configDataService;
        private readonly ICourseContentService courseContentService;
        private readonly ISessionService sessionService;
        private readonly ISectionContentDataService sectionContentDataService;
        private readonly ITutorialContentDataService tutorialContentDataService;
        private readonly IDiagnosticAssessmentService diagnosticAssessmentService;
        private readonly IPostLearningAssessmentService postLearningAssessmentService;
        private readonly ICourseCompletionService courseCompletionService;
        private readonly ICourseDataService courseDataService;
        private readonly IClockUtility clockUtility;

        public LearningMenuController(
            ILogger<LearningMenuController> logger,
            IConfiguration config,
            IConfigDataService configDataService,
            ICourseContentService courseContentService,
            ISectionContentDataService sectionContentDataService,
            ITutorialContentDataService tutorialContentDataService,
            IDiagnosticAssessmentService diagnosticAssessmentService,
            IPostLearningAssessmentService postLearningAssessmentService,
            ISessionService sessionService,
            ICourseCompletionService courseCompletionService,
            ICourseDataService courseDataService,
            IClockUtility clockUtility
        )
        {
            this.logger = logger;
            this.config = config;
            this.configDataService = configDataService;
            this.courseContentService = courseContentService;
            this.tutorialContentDataService = tutorialContentDataService;
            this.sessionService = sessionService;
            this.sectionContentDataService = sectionContentDataService;
            this.diagnosticAssessmentService = diagnosticAssessmentService;
            this.postLearningAssessmentService = postLearningAssessmentService;
            this.courseCompletionService = courseCompletionService;
            this.clockUtility = clockUtility;
            this.courseDataService = courseDataService;
        }

        [Route("/LearningMenu/{customisationId:int}")]
        public IActionResult Index(int customisationId)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            if (config.GetValue<string>("LegacyLearningMenu") != "")
            {
                if ((config.GetValue<bool>("LegacyLearningMenu") && !configDataService.GetCentreBetaTesting(centreId))|(!config.GetValue<bool>("LegacyLearningMenu") && configDataService.GetCentreBetaTesting(centreId)))
                {
                    string baseUrl = config.GetValue<string>("CurrentSystemBaseUrl");
                    string url = $"{baseUrl}/tracking/learn?customisationid={customisationId}&lp=1";
                    return Redirect(url);
                }
            }
            var candidateId = User.GetCandidateIdKnownNotNull();
            var courseContent = courseContentService.GetCourseContent(candidateId, customisationId);
            if (courseContent == null)
            {
                logger.LogError(
                    "Redirecting to 404 as course/centre id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId.ToString() ?? "null"}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }
            if (!String.IsNullOrEmpty(courseContent.Password) && !courseContent.PasswordSubmitted)
            {
                return RedirectToAction("CoursePassword", "LearningMenu", new { customisationId });
            }
            if (courseContent.Sections.Count == 1)
            {
                var sectionId = courseContent.Sections.First().Id;
                return RedirectToAction("Section", "LearningMenu", new { customisationId, sectionId });
            }
            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);
            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }
            sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            SetTempData(candidateId, customisationId);

            var model = new InitialMenuViewModel(courseContent);
            return View(model);
        }
        [Route("LearningMenu/{customisationId:int}/Password")]
        public IActionResult CoursePassword(int customisationId, bool error = false)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var candidateId = User.GetCandidateIdKnownNotNull();
            var courseContent = courseContentService.GetCourseContent(candidateId, customisationId);
            if (courseContent == null)
            {
                logger.LogError(
                    "Redirecting to 404 as course/centre id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId.ToString() ?? "null"}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }
            var isCompleted = courseDataService.IsCourseCompleted(candidateId, customisationId);
            if (isCompleted)
                TempData["LearningActivity"] = "Completed";
            else
                TempData["LearningActivity"] = "Available";

            var model = new InitialMenuViewModel(courseContent);
            return View(model);
        }
        [HttpPost]
        [Route("LearningMenu/{customisationId:int}/Password")]
        public IActionResult CoursePassword(int customisationId, string? password)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var candidateId = User.GetCandidateIdKnownNotNull();
            var coursePassword = courseContentService.GetCoursePassword(customisationId);
            if(coursePassword == null)
            {
                logger.LogError(
                    "Redirecting to 404 as course password was null. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId.ToString() ?? "null"}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }
            if (password == coursePassword)
            {
                var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);
                if (progressId == null)
                {
                    logger.LogError(
                        "Redirecting to 404 as no progress id was returned. " +
                        $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                    return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
                }
                courseContentService.LogPasswordSubmitted((int)progressId);
                return RedirectToAction("Index", "LearningMenu", new { customisationId });
            }
            else
            {
                return RedirectToAction("CoursePassword", "LearningMenu", new { customisationId, error = true });
            }
        }
            [Route("/LearningMenu/Close")]
        public IActionResult Close(string learningActivity)
        {
            var action = string.IsNullOrEmpty(learningActivity) ? "Current" : learningActivity;
            sessionService.StopDelegateSession(User.GetCandidateIdKnownNotNull(), HttpContext.Session);

            return RedirectToAction(action, "LearningPortal");
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}")]
        public IActionResult Section(int customisationId, int sectionId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var centreId = User.GetCentreIdKnownNotNull();
            var sectionContent = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            if (sectionContent == null)
            {
                logger.LogError(
                    "Redirecting to 404 as section/centre id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId.ToString() ?? "null"}, section id: {sectionId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }
            if (!String.IsNullOrEmpty(sectionContent.Password) && !sectionContent.PasswordSubmitted)
            {
                return RedirectToAction("CoursePassword", "LearningMenu", new { customisationId });
            }
            var hasDiagnosticAssessment = sectionContent.DiagnosticAssessmentPath != null && sectionContent.DiagnosticStatus;
            var hasPostLearningAssessment = sectionContent.PostLearningAssessmentPath != null && sectionContent.IsAssessed;
            var hasConsolidationMaterial = sectionContent.ConsolidationPath != null;

            if (sectionContent.Tutorials.Count == 1
                && !hasDiagnosticAssessment
                && !hasPostLearningAssessment
                && !hasConsolidationMaterial
            )
            {
                var tutorialId = sectionContent.Tutorials.First().Id;
                return RedirectToAction("Tutorial", "LearningMenu", new { customisationId, sectionId, tutorialId });
            }

            var justHasAssessments = sectionContent.Tutorials.Count == 0
                                     && !hasConsolidationMaterial
                                     && (hasDiagnosticAssessment || hasPostLearningAssessment);

            if (justHasAssessments && !hasDiagnosticAssessment)
            {
                return RedirectToAction("PostLearning", "LearningMenu", new { customisationId, sectionId });
            }

            if (justHasAssessments && !hasPostLearningAssessment)
            {
                return RedirectToAction("Diagnostic", "LearningMenu", new { customisationId, sectionId });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId}, section id: {sectionId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            SetTempData(candidateId, customisationId);

            var model = new SectionContentViewModel(config, sectionContent, customisationId, sectionId);
            return View("Section/Section", model);
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}/Diagnostic")]
        public IActionResult Diagnostic(int customisationId, int sectionId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var centreId = User.GetCentreIdKnownNotNull();
            var diagnosticAssessment =
                diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            if (diagnosticAssessment == null )
            {
                logger.LogError(
                    "Redirecting to 404 as section/centre id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId.ToString() ?? "null"}, section id: {sectionId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }
            if (!String.IsNullOrEmpty(diagnosticAssessment.Password) && !diagnosticAssessment.PasswordSubmitted)
            {
                return RedirectToAction("CoursePassword", "LearningMenu", new { customisationId });
            }
            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            SetTempData(candidateId, customisationId);
            var model = new DiagnosticAssessmentViewModel(diagnosticAssessment, customisationId, sectionId);
            return View("Diagnostic/Diagnostic", model);
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}/Diagnostic/Content")]
        public IActionResult DiagnosticContent(int customisationId, int sectionId, List<int> checkedTutorials)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var centreId = User.GetCentreIdKnownNotNull();
            var diagnosticContent = diagnosticAssessmentService.GetDiagnosticContent(customisationId, sectionId, checkedTutorials);

            if (diagnosticContent == null)
            {
                logger.LogError(
                    "Redirecting to 404 as customisation/section/centre id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId.ToString() ?? "null"}, section id: {sectionId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            SetTempData(candidateId, customisationId);
            var model = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                checkedTutorials,
                customisationId,
                centreId,
                sectionId,
                progressId.Value,
                candidateId
            );
            return View("Diagnostic/DiagnosticContent", model);
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}/PostLearning")]
        public IActionResult PostLearning(int customisationId, int sectionId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var centreId = User.GetCentreIdKnownNotNull();
            var postLearningAssessment =
                postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            if (postLearningAssessment == null)
            {
                logger.LogError(
                    "Redirecting to 404 as section/centre id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId.ToString() ?? "null"}, section id: {sectionId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }
            if (!String.IsNullOrEmpty(postLearningAssessment.Password) && !postLearningAssessment.PasswordSubmitted)
            {
                return RedirectToAction("CoursePassword", "LearningMenu", new { customisationId });
            }
            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            SetTempData(candidateId, customisationId);
            var model = new PostLearningAssessmentViewModel(postLearningAssessment, customisationId, sectionId);
            return View("PostLearning/PostLearning", model);
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}/PostLearning/Content")]
        public IActionResult PostLearningContent(int customisationId, int sectionId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var centreId = User.GetCentreIdKnownNotNull();
            var postLearningContent = postLearningAssessmentService.GetPostLearningContent(customisationId, sectionId);

            if (postLearningContent == null)
            {
                logger.LogError(
                    "Redirecting to 404 as customisation/section/centre id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId.ToString() ?? "null"}, section id: {sectionId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            SetTempData(candidateId, customisationId);
            var model = new PostLearningContentViewModel(
                config,
                postLearningContent,
                customisationId,
                centreId,
                sectionId,
                progressId.Value,
                candidateId
            );
            return View("PostLearning/PostLearningContent", model);
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}/{tutorialId:int}")]
        public async Task<IActionResult> Tutorial(int customisationId, int sectionId, int tutorialId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var centreId = User.GetCentreIdKnownNotNull();

            var tutorialInformation =
                tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            if (tutorialInformation == null)
            {
                logger.LogError(
                    "Redirecting to 404 as customisation/section/tutorial id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId.ToString() ?? "null"}, section id: {sectionId} tutorial id: {tutorialId}"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            if (!String.IsNullOrEmpty(tutorialInformation.Password) && !tutorialInformation.PasswordSubmitted)
            {
                return RedirectToAction("CoursePassword", "LearningMenu", new { customisationId });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            SetTempData(candidateId, customisationId);
            /* Course progress doesn't get updated if the auth token expires by the end of the tutorials.
              Some tutorials are longer than the default auth token lifetime of 1 hour, so we set the auth expiry to 8 hours.
              See HEEDLS-637 and HEEDLS-674 for more details */
            if (tutorialInformation.AverageTutorialDuration >= MinimumTutorialAverageTimeToIncreaseAuthExpiry)
            {
                await IncreaseAuthenticatedUserExpiry();
            }

            var viewModel = new TutorialViewModel(config, tutorialInformation, customisationId, sectionId);
            return View("Tutorial/Tutorial", viewModel);
        }

        [Route("/LearningMenu/{customisationId:int}/{sectionId:int}/{tutorialId:int}/Tutorial")]
        public IActionResult ContentViewer(int customisationId, int sectionId, int tutorialId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var centreId = User.GetCentreIdKnownNotNull();

            var tutorialContent = tutorialContentDataService.GetTutorialContent(customisationId, sectionId, tutorialId);

            if (tutorialContent?.TutorialPath == null)
            {
                logger.LogError(
                    "Redirecting to 404 as customisation/section/tutorial id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId.ToString() ?? "null"}, section id: {sectionId} tutorial id: {tutorialId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            SetTempData(candidateId, customisationId);
            var model = new ContentViewerViewModel(
                config,
                tutorialContent,
                customisationId,
                centreId,
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
            var candidateId = User.GetCandidateIdKnownNotNull();
            var centreId = User.GetCentreIdKnownNotNull();

            var tutorialVideo = tutorialContentDataService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            if (tutorialVideo == null)
            {
                logger.LogError(
                    "Redirecting to 404 as customisation/section/tutorial id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId.ToString() ?? "null"}, section id: {sectionId} tutorial id: {tutorialId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            SetTempData(candidateId, customisationId);
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
            var candidateId = User.GetCandidateIdKnownNotNull();
            var centreId = User.GetCentreIdKnownNotNull();

            var courseCompletion = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

            if (courseCompletion == null)
            {
                logger.LogError(
                    "Redirecting to 404 as customisation id was not found. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, " +
                    $"centre id: {centreId.ToString() ?? "null"}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);

            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session);
            courseContentService.UpdateProgress(progressId.Value);

            SetTempData(candidateId, customisationId);
            var model = new CourseCompletionViewModel(config, courseCompletion, progressId.Value);
            return View("Completion/Completion", model);
        }

        private async Task IncreaseAuthenticatedUserExpiry()
        {
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IssuedUtc = clockUtility.UtcNow,
                ExpiresUtc = clockUtility.UtcNow.AddHours(8)
            };
            await HttpContext.SignInAsync("Identity.Application", User, authProperties);
        }

        private void SetTempData(int candidateId, int customisationId)
        {
            var isCompleted = courseDataService.IsCourseCompleted(candidateId, customisationId);
            if (isCompleted)
                TempData["LearningActivity"] = "Completed";
            else
                TempData["LearningActivity"] = "Current";
        }
    }
}
