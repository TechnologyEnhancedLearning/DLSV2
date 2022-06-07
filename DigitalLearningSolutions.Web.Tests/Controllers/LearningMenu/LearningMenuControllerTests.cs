namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.LearningMenuController;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public partial class LearningMenuControllerTests
    {
        private const int CandidateId = 11;
        private const int CentreId = 2;
        private const int CustomisationId = 12;
        private const int SectionId = 199;
        private const int TutorialId = 842;
        private ISession httpContextSession = null!;
        private IAuthenticationService authenticationService = null!;
        private IClockService clockService = null!;
        private IConfiguration config = null!;
        private IConfigDataService configDataService = null!;
        private LearningMenuController controller = null!;
        private ICourseCompletionService courseCompletionService = null!;
        private ICourseContentService courseContentService = null!;
        private IDiagnosticAssessmentService diagnosticAssessmentService = null!;
        private IPostLearningAssessmentService postLearningAssessmentService = null!;
        private ISectionContentDataService sectionContentDataService = null!;
        private ISessionService sessionService = null!;
        private ITutorialContentDataService tutorialContentDataService = null!;

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
            clockService = A.Fake<IClockService>();

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
                    courseCompletionService,
                    clockService
                ).WithDefaultContext()
                .WithMockHttpContextSession()
                .WithMockUser(true, CentreId, adminId: null, delegateId: CandidateId)
                .WithMockTempData()
                .WithMockServices();

            authenticationService =
                (IAuthenticationService)controller.HttpContext.RequestServices.GetService(
                    typeof(IAuthenticationService)
                );
            httpContextSession = controller.HttpContext.Session;
        }
    }
}
