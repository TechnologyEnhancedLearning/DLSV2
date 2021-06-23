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
        private LearningPortalController controller = null!;
        private ICentresDataService centresDataService = null!;
        private ICourseDataService courseDataService = null!;
        private ISelfAssessmentService selfAssessmentService = null!;
        private INotificationService notificationService = null!;
        private IConfiguration config = null!;
        private IFilteredApiHelperService filteredApiHelperService = null!;
        private const string BaseUrl = "https://www.dls.nhs.uk";
        private const int CandidateId = 11;
        private const int SelfAssessmentId = 1;
        private const int CentreId = 2;

        [SetUp]
        public void SetUp()
        {
            centresDataService = A.Fake<ICentresDataService>();
            courseDataService = A.Fake<ICourseDataService>();
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
                courseDataService,
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
