﻿namespace DigitalLearningSolutions.Web.Controllers.LearningMenuController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
        private readonly ICourseContentService courseContentService;
        private readonly ISessionService sessionService;
        private readonly ISectionContentService sectionContentService;
        private readonly ITutorialContentService tutorialContentService;
        private readonly IDiagnosticAssessmentService diagnosticAssessmentService;
        private readonly IPostLearningAssessmentService postLearningAssessmentService;
        private readonly ICourseCompletionService courseCompletionService;
        private readonly ICourseService courseService;
        private readonly IClockUtility clockUtility;

        public LearningMenuController(
            ILogger<LearningMenuController> logger,
            IConfiguration config,
            ICourseContentService courseContentService,
            ISectionContentService sectionContentService,
            ITutorialContentService tutorialContentService,
            IDiagnosticAssessmentService diagnosticAssessmentService,
            IPostLearningAssessmentService postLearningAssessmentService,
            ISessionService sessionService,
            ICourseCompletionService courseCompletionService,
            ICourseService courseService,
            IClockUtility clockUtility
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
            this.clockUtility = clockUtility;
            this.courseService = courseService;
        }

        [Route("/LearningMenu/{customisationId:int}")]
        public IActionResult Index(int customisationId, int progressID)
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
            if (!String.IsNullOrEmpty(courseContent.Password) && !courseContent.PasswordSubmitted)
            {
                return RedirectToAction("CoursePassword", "LearningMenu", new { customisationId });
            }

            if (courseContent.Sections.Count == 1)
            {
                var sectionId = courseContent.Sections.First().Id;
                return RedirectToAction("Section", "LearningMenu", new { customisationId, sectionId });
            }
            // Unique Id Manipulation Detection is being disabled as part of work on TD-3838 - a bug created by its introduction
            //if (UniqueIdManipulationDetected(candidateId, customisationId))
            //{
            //    return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            //}
            var progressId = courseContentService.GetOrCreateProgressId(candidateId, customisationId, centreId);
            if (progressId == null)
            {
                logger.LogError(
                    "Redirecting to 404 as no progress id was returned. " +
                    $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            if (sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session) > 0)
            {
                courseContentService.UpdateProgress(progressId.Value);
            };

            SetTempData(candidateId, customisationId, progressID);

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
            var isCompleted = courseService.IsCourseCompleted(candidateId, customisationId);
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
            if (coursePassword == null)
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
            var sectionContent = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

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
            if (sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session) > 0)
            {
                courseContentService.UpdateProgress(progressId.Value);
            };

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

            if (diagnosticAssessment == null)
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

            if (sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session) > 0)
            {
                courseContentService.UpdateProgress(progressId.Value);
            };

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

            if (sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session) > 0)
            {
                courseContentService.UpdateProgress(progressId.Value);
            };

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

            if (sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session) > 0)
            {
                courseContentService.UpdateProgress(progressId.Value);
            };

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

            if (sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session) >= 0)
            {
                courseContentService.UpdateProgress(progressId.Value);
            };

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
                tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

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

            if (sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session) > 0)
            {
                courseContentService.UpdateProgress(progressId.Value);
            };

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

            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

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

            if (sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session) > 0)
            {
                courseContentService.UpdateProgress(progressId.Value);
            };

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

            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

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

            if (sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session) > 0)
            {
                courseContentService.UpdateProgress(progressId.Value);
            };

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

            if (sessionService.StartOrUpdateDelegateSession(candidateId, customisationId, HttpContext.Session) > 0)
            {
                courseContentService.UpdateProgress(progressId.Value);
            };

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
            var isCompleted = courseService.IsCourseCompleted(candidateId, customisationId);
            if (isCompleted)
                TempData["LearningActivity"] = "Completed";
            else
                TempData["LearningActivity"] = "Current";
        }
        private void SetTempData(int candidateId, int customisationId,int progressID)
        {
            var isCompleted = courseService.IsCourseCompleted(candidateId, customisationId, progressID);
            if (isCompleted)
                TempData["LearningActivity"] = "Completed";
            else
                TempData["LearningActivity"] = "Current";
        }

        private bool UniqueIdManipulationDetected(int candidateId, int customisationId)
        {
            int? progressId = courseContentService.GetProgressId(candidateId, customisationId);
            if (progressId.HasValue)
            {
                return false;
            }

            bool isSelfRegister = courseService.GetSelfRegister(customisationId);
            if (isSelfRegister)
            {
                return false;
            }

            return true;
        }
    }
}
