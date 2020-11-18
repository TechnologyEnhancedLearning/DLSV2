namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningPortal
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.LearningPortalController;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using FakeItEasy;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public partial class LearningPortalControllerTests
    {
        private LearningPortalController controller;
        private ICentresService centresService;
        private ICourseService courseService;
        private ISelfAssessmentService selfAssessmentService;
        private IUnlockService unlockService;
        private IConfiguration config;
        private IFilteredApiHelperService filteredApiHelperService;
        private const string BaseUrl = "https://www.dls.nhs.uk";
        private const int CandidateId = 11;
        private const int SelfAssessmentId = 1;
        private const int CentreId = 2;

        [SetUp]
        public void SetUp()
        {
            centresService = A.Fake<ICentresService>();
            courseService = A.Fake<ICourseService>();
            selfAssessmentService = A.Fake<ISelfAssessmentService>();
            unlockService = A.Fake<IUnlockService>();
            var logger = A.Fake<ILogger<LearningPortalController>>();
            config = A.Fake<IConfiguration>();
            filteredApiHelperService = A.Fake<IFilteredApiHelperService>();
            
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("learnCandidateID", CandidateId.ToString()),
                new Claim("UserCentreID", CentreId.ToString())
            }, "mock"));
            controller = new LearningPortalController(
                centresService,
                courseService,
                selfAssessmentService,
                unlockService,
                logger,
                config,
                filteredApiHelperService
            )
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext { User = user } }
            };
        }
    }
}
