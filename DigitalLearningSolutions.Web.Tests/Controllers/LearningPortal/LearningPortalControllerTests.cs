namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningPortal
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers.LearningPortalController;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public partial class LearningPortalControllerTests
    {
        private const string BaseUrl = "https://www.dls.nhs.uk";
        private const int CandidateId = 11;
        private const int UserId = 1;
        private const int SelfAssessmentId = 1;
        private const string Vocabulary = "Capabilities";
        private const int CentreId = 2;
        private IActionPlanService actionPlanService = null!;
        private ICentresDataService centresDataService = null!;
        private IConfiguration config = null!;
        private LearningPortalController controller = null!;
        private ICourseDataService courseDataService = null!;
        private IFrameworkNotificationService frameworkNotificationService = null!;
        private INotificationService notificationService = null!;
        private ISelfAssessmentService selfAssessmentService = null!;
        private ISupervisorService supervisorService = null!;
        private IFrameworkService frameworkService = null!;
        private ICandidateAssessmentDownloadFileService candidateAssessmentDownloadFileService = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private IMultiPageFormService multiPageFormService = null!;
        private IUserDataService userDataService = null!;
        private IClockUtility clockUtility = null!;

        [SetUp]
        public void SetUp()
        {
            actionPlanService = A.Fake<IActionPlanService>();
            centresDataService = A.Fake<ICentresDataService>();
            courseDataService = A.Fake<ICourseDataService>();
            userDataService = A.Fake<IUserDataService>();
            selfAssessmentService = A.Fake<ISelfAssessmentService>();
            supervisorService = A.Fake<ISupervisorService>();
            frameworkService = A.Fake<IFrameworkService>();
            notificationService = A.Fake<INotificationService>();
            frameworkNotificationService = A.Fake<IFrameworkNotificationService>();
            candidateAssessmentDownloadFileService = A.Fake<ICandidateAssessmentDownloadFileService>();
            var logger = A.Fake<ILogger<LearningPortalController>>();
            config = A.Fake<IConfiguration>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
            clockUtility = A.Fake<IClockUtility>();

            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("learnCandidateID", CandidateId.ToString()),
                        new Claim("UserCentreID", CentreId.ToString()),
                        new Claim("UserId", UserId.ToString()),
                    },
                    "mock"
                )
            );
            controller = new LearningPortalController(
                centresDataService,
                courseDataService,
                userDataService,
                selfAssessmentService,
                supervisorService,
                frameworkService,
                notificationService,
                frameworkNotificationService,
                logger,
                config,
                actionPlanService,
                candidateAssessmentDownloadFileService,
                searchSortFilterPaginateService,
                multiPageFormService,
                clockUtility
            );
            controller.ControllerContext = new ControllerContext
                { HttpContext = new DefaultHttpContext { User = user } };
            controller = controller.WithMockTempData();
        }
    }
}
