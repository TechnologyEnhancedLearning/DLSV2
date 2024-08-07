namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers.LearningMenuController;
    using DigitalLearningSolutions.Web.Services;
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
        const int progressID = 34;
        private const int SectionId = 199;
        private const int TutorialId = 842;
        private ISession httpContextSession = null!;
        private IAuthenticationService? authenticationService = null!;
        private IClockUtility clockUtility = null!;
        private IConfiguration config = null!;
        //private IConfigDataService configDataService = null!;
        private LearningMenuController controller = null!;
        private ICourseCompletionService courseCompletionService = null!;
        private ICourseContentService courseContentService = null!;
        private IDiagnosticAssessmentService diagnosticAssessmentService = null!;
        private IPostLearningAssessmentService postLearningAssessmentService = null!;
        private ISectionContentService sectionContentService = null!;
        private ISessionService sessionService = null!;
        private ITutorialContentService tutorialContentService = null!;
        private ICourseService courseService = null!;

        [SetUp]
        public void SetUp()
        {
            var logger = A.Fake<ILogger<LearningMenuController>>();
            config = A.Fake<IConfiguration>();
            //configDataService = A.Fake<IConfigDataService>();
            courseContentService = A.Fake<ICourseContentService>();
            tutorialContentService = A.Fake<ITutorialContentService>();
            sessionService = A.Fake<ISessionService>();
            sectionContentService = A.Fake<ISectionContentService>();
            diagnosticAssessmentService = A.Fake<IDiagnosticAssessmentService>();
            postLearningAssessmentService = A.Fake<IPostLearningAssessmentService>();
            courseCompletionService = A.Fake<ICourseCompletionService>();
            courseService = A.Fake<ICourseService>();
            clockUtility = A.Fake<IClockUtility>();

            controller = new LearningMenuController(
                    logger,
                    config,
                    courseContentService,
                    sectionContentService,
                    tutorialContentService,
                    diagnosticAssessmentService,
                    postLearningAssessmentService,
                    sessionService,
                    courseCompletionService,
                    courseService,
                    clockUtility
                ).WithDefaultContext()
                .WithMockHttpContextSession()
                .WithMockUser(true, CentreId, adminId: null, delegateId: CandidateId)
                .WithMockTempData()
                .WithMockServices();

            authenticationService =
                (IAuthenticationService?)controller.HttpContext.RequestServices.GetService(
                    typeof(IAuthenticationService)
                );
            httpContextSession = controller.HttpContext.Session;
        }
    }
}
