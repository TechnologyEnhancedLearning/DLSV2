namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningPortal
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.DataServices;
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
        private ICentresDataService centresDataService;
        private ICourseService courseService;
        private ISelfAssessmentService selfAssessmentService;
        private INotificationService notificationService;
        private IConfiguration config;
        private IFilteredApiHelperService filteredApiHelperService;
        private const string BaseUrl = "https://www.dls.nhs.uk";
        private const int CandidateId = 11;
        private const int SelfAssessmentId = 1;
        private const int CentreId = 2;

        [SetUp]
        public void SetUp()
        {
            centresDataService = A.Fake<ICentresDataService>();
            courseService = A.Fake<ICourseService>();
            selfAssessmentService = A.Fake<ISelfAssessmentService>();
            notificationService = A.Fake<INotificationService>();
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
                centresDataService,
                courseService,
                selfAssessmentService,
                notificationService,
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
