namespace DigitalLearningSolutions.Web.Tests.Controllers.Frameworks
{
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
        private FrameworksController controller;
        private IFrameworkService frameworkService;
        private ICommonService commonService;
        private IConfigService configService;
        private IConfiguration config;
        private IFrameworkNotificationService frameworkNotificationService;
        private IImportCompetenciesFromFileService importCompetenciesFromFileService;
        private ICompetencyLearningResourcesDataService competencyLearningResourcesDataService;
        private const string BaseUrl = "https://www.dls.nhs.uk";
        private const int CentreId = 101;
        private const int AdminId = 1;

        [SetUp]
        public void SetUp()
        {
            frameworkService = A.Fake<IFrameworkService>();
            frameworkNotificationService = A.Fake<IFrameworkNotificationService>();
            commonService = A.Fake<ICommonService>();
            configService = A.Fake<ConfigService>();
            var logger = A.Fake<ILogger<FrameworksController>>();
            config = A.Fake<IConfiguration>();
            importCompetenciesFromFileService = A.Fake<IImportCompetenciesFromFileService>();
            competencyLearningResourcesDataService = A.Fake<ICompetencyLearningResourcesDataService>();

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
                competencyLearningResourcesDataService
            )
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext { User = user } }
            };
        }
    }
}
