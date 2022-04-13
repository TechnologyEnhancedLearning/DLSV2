namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.LearningMenuController;
    using FakeItEasy;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        private IAuthenticationService authenticationService = null!;
        private IConfiguration config = null!;
        private IConfigDataService configDataService = null!;
        private LearningMenuController controller = null!;
        private ICourseCompletionService courseCompletionService = null!;
        private ICourseContentService courseContentService = null!;
        private IDiagnosticAssessmentService diagnosticAssessmentService = null!;
        private ISession httpContextSession = null!;
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

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("learnCandidateID", CandidateId.ToString()),
                        new Claim("UserCentreID", CentreId.ToString()),
                    },
                    "mock"
                )
            );
            httpContextSession = new MockHttpContextSession();

            authenticationService = A.Fake<IAuthenticationService>();
            A.CallTo
            (
                () => authenticationService.SignInAsync(
                    A<HttpContext>._,
                    A<string>._,
                    A<ClaimsPrincipal>._,
                    A<AuthenticationProperties>._
                )
            ).Returns(Task.CompletedTask);
            var urlHelperFactory = A.Fake<IUrlHelperFactory>();
            var services = A.Fake<IServiceProvider>();
            A.CallTo(() => services.GetService(typeof(IAuthenticationService))).Returns(authenticationService);
            A.CallTo(() => services.GetService(typeof(IUrlHelperFactory))).Returns(urlHelperFactory);

            var httpContext = new DefaultHttpContext
            {
                User = user,
                Session = httpContextSession,
                RequestServices = services,
            };
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
                    HttpContext = httpContext,
                },
                TempData = new TempDataDictionary(httpContext, A.Fake<ITempDataProvider>()),
            };
        }
    }
}
