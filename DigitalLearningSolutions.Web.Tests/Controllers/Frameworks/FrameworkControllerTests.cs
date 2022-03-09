namespace DigitalLearningSolutions.Web.Tests.Controllers.Frameworks
{
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.FrameworksController;
    using FakeItEasy;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using System.Security.Claims;

    public partial class FrameworkControllerTests
    {
        private FrameworksController controller = null!;
        private IFrameworkService frameworkService = null!;
        private ICommonService commonService = null!;
        private IConfiguration config = null!;
        private IFrameworkNotificationService frameworkNotificationService = null!;
        private IImportCompetenciesFromFileService importCompetenciesFromFileService = null!;
        private ICompetencyLearningResourcesDataService competencyLearningResourcesDataService = null!;
        private ILearningHubApiClient learningHubApiClient = null!;
        
        private const string BaseUrl = "https://www.dls.nhs.uk";
        private const int CentreId = 101;
        private const int AdminId = 1;

        [SetUp]
        public void SetUp()
        {
            var logger = A.Fake<ILogger<FrameworksController>>();

            frameworkService = A.Fake<IFrameworkService>();
            frameworkNotificationService = A.Fake<IFrameworkNotificationService>();
            commonService = A.Fake<ICommonService>();
            config = A.Fake<IConfiguration>();
            importCompetenciesFromFileService = A.Fake<IImportCompetenciesFromFileService>();
            competencyLearningResourcesDataService = A.Fake<ICompetencyLearningResourcesDataService>();
            learningHubApiClient = A.Fake<ILearningHubApiClient>();

            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("UserAdminId", AdminId.ToString()),
                new Claim("UserCentreID", CentreId.ToString()),
                new Claim("IsFrameworkDeveloper", "True")
            }, "mock"));

            controller = new FrameworksController(
                frameworkService,
                commonService,
                frameworkNotificationService,
                logger,
                importCompetenciesFromFileService,
                competencyLearningResourcesDataService,
                learningHubApiClient
            )
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext { User = user } }
            };
        }
    }
}
