﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.Frameworks
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Controllers.FrameworksController;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using GDS.MultiPageFormData;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.AspNetCore.Hosting;

    public partial class FrameworkControllerTests
    {
        private const string BaseUrl = "https://www.dls.nhs.uk";
        private const int CentreId = 101;
        private const int AdminId = 1;
        private ICommonService commonService = null!;
        private ICompetencyLearningResourcesService competencyLearningResourcesService = null!;
        private IConfiguration config = null!;
        private FrameworksController controller = null!;
        private IFrameworkNotificationService frameworkNotificationService = null!;
        private IFrameworkService frameworkService = null!;
        private IImportCompetenciesFromFileService importCompetenciesFromFileService = null!;
        private ILearningHubApiClient learningHubApiClient = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private IMultiPageFormService multiPageFormService = null!;
        private IClockUtility clockUtility = null!;
        private IWebHostEnvironment webHostEnvironment = null!;

        [SetUp]
        public void SetUp()
        {
            frameworkService = A.Fake<IFrameworkService>();
            frameworkNotificationService = A.Fake<IFrameworkNotificationService>();
            commonService = A.Fake<ICommonService>();
            var logger = A.Fake<ILogger<FrameworksController>>();
            config = A.Fake<IConfiguration>();
            importCompetenciesFromFileService = A.Fake<IImportCompetenciesFromFileService>();
            competencyLearningResourcesService = A.Fake<ICompetencyLearningResourcesService>();
            learningHubApiClient = A.Fake<ILearningHubApiClient>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
            multiPageFormService = A.Fake<IMultiPageFormService>();
            clockUtility = A.Fake<ClockUtility>();
            webHostEnvironment = A.Fake<IWebHostEnvironment>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("UserAdminId", AdminId.ToString()),
                        new Claim("UserCentreID", CentreId.ToString()),
                        new Claim("IsFrameworkDeveloper", "True"),
                    },
                    "mock"
                )
            );
            controller = new FrameworksController(
                frameworkService,
                commonService,
                frameworkNotificationService,
                logger,
                importCompetenciesFromFileService,
                competencyLearningResourcesService,
                learningHubApiClient,
                searchSortFilterPaginateService,
                multiPageFormService,
                clockUtility,
                webHostEnvironment
            )
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } },
            };
        }
    }
}
