namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.LearningMenuController;
    using FakeItEasy;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public partial class LearningMenuControllerTests
    {
        private LearningMenuController controller = null!;
        private ICourseContentService courseContentService = null!;
        private ITutorialContentDataService tutorialContentDataService = null!;
        private ISessionService sessionService = null!;
        private ISectionContentDataService sectionContentDataService = null!;
        private IDiagnosticAssessmentService diagnosticAssessmentService = null!;
        private IPostLearningAssessmentService postLearningAssessmentService = null!;
        private ICourseCompletionService courseCompletionService = null!;
        private ISession httpContextSession = null!;
        private IConfiguration config = null!;
        private IConfigDataService configDataService = null!;
        
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
            configDataService = A.Fake<IConfigDataService>();
            courseContentService = A.Fake<ICourseContentService>();
            tutorialContentDataService = A.Fake<ITutorialContentDataService>();
            sessionService = A.Fake<ISessionService>();
            sectionContentDataService = A.Fake<ISectionContentDataService>();
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
                configDataService,
                courseContentService,
                sectionContentDataService,
                tutorialContentDataService,
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
