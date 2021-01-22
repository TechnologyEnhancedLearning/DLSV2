namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Web.Controllers.LearningMenuController;
    using FakeItEasy;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public partial class LearningMenuControllerTests
    {
        private LearningMenuController controller;
        private ICourseContentService courseContentService;
        private ITutorialContentService tutorialContentService;
        private ISessionService sessionService;
        private ISectionContentService sectionContentService;
        private IDiagnosticAssessmentDataService diagnosticAssessmentDataService;
        private IDiagnosticAssessmentService diagnosticAssessmentService;
        private IPostLearningAssessmentService postLearningAssessmentService;
        private ICourseCompletionService courseCompletionService;
        private ISession httpContextSession;
        private IConfiguration config;
        private const int CandidateId = 11;
        private const int CentreId = 2;
        private const int CustomisationId = 12;
        private const int SectionId = 199;
        private const int TutorialId = 842;

        [SetUp]
        public void SetUp()
        {
            var logger = A.Fake<ILogger<LearningMenuController>>();
            config = A.Fake<IConfiguration>();
            courseContentService = A.Fake<ICourseContentService>();
            tutorialContentService = A.Fake<ITutorialContentService>();
            sessionService = A.Fake<ISessionService>();
            sectionContentService = A.Fake<ISectionContentService>();
            diagnosticAssessmentDataService = A.Fake<IDiagnosticAssessmentDataService>();
            diagnosticAssessmentService = A.Fake<IDiagnosticAssessmentService>();
            postLearningAssessmentService = A.Fake<IPostLearningAssessmentService>();
            courseCompletionService = A.Fake<ICourseCompletionService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("learnCandidateID", CandidateId.ToString()),
                new Claim("UserCentreID", CentreId.ToString())
            }, "mock"));
            httpContextSession = new MockHttpContextSession();

            controller = new LearningMenuController(
                logger,
                config,
                courseContentService,
                sectionContentService,
                tutorialContentService,
                diagnosticAssessmentDataService,
                diagnosticAssessmentService,
                postLearningAssessmentService,
                sessionService,
                courseCompletionService
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = user,
                        Session = httpContextSession
                    }
                }
            };
        }
    }
}
