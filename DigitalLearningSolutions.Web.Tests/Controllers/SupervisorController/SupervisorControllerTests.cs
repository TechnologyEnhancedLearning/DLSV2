namespace DigitalLearningSolutions.Web.Tests.Controllers.Support
{

    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers.SupervisorController;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using GDS.MultiPageFormData;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class SupervisorControllerTests
    {
        private ISupervisorService supervisorService = null!;
        private ICommonService commonService = null!;
        private IFrameworkNotificationService frameworkNotificationService = null!;
        private ISelfAssessmentService selfAssessmentService = null!;
        private IFrameworkService frameworkService = null!;
        private IConfigService configService = null!;
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private IUserService userService = null!;
        private ILogger<SupervisorController> logger = null!;
        private IConfiguration config = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private IMultiPageFormService multiPageFormService = null!;
        private IRegistrationService registrationService = null!;
        private ICentresService centresService = null!;
        private IEmailGenerationService emailGenerationService = null!;
        private IEmailService emailService = null!;
        private IClockUtility clockUtility = null!;
        private ICandidateAssessmentDownloadFileService candidateAssessmentDownloadFileService = null!;
        private IPdfService pdfService = null!;
        private  ICourseCategoriesService courseCategoriesService = null!;

        [SetUp]
        public void Setup()
        {
            supervisorService = A.Fake<ISupervisorService>();
            commonService = A.Fake<ICommonService>();
            frameworkNotificationService = A.Fake<IFrameworkNotificationService>();
            selfAssessmentService = A.Fake<ISelfAssessmentService>();
            frameworkService = A.Fake<IFrameworkService>();
            configService = A.Fake<IConfigService>();
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            userService = A.Fake<IUserService>();
            logger = A.Fake<ILogger<SupervisorController>>();
            config = A.Fake<IConfiguration>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
            multiPageFormService = A.Fake<IMultiPageFormService>();
            registrationService = A.Fake<IRegistrationService>();
            centresService = A.Fake<ICentresService>();
            emailGenerationService = A.Fake<IEmailGenerationService>();
            emailService = A.Fake<IEmailService>();
            clockUtility = A.Fake<IClockUtility>();
            candidateAssessmentDownloadFileService = A.Fake<ICandidateAssessmentDownloadFileService>();
            pdfService = A.Fake<IPdfService>();
            courseCategoriesService = A.Fake<ICourseCategoriesService>();
            A.CallTo(() => candidateAssessmentDownloadFileService.GetCandidateAssessmentDownloadFileForCentre(A<int>._, A<int>._, A<bool>._))
                .Returns(new byte[] { });
        }

        [TestCase(1, "test", "Digital Capability Self Assessment Deprecated", 1)]
        [TestCase(1, "test", "IV Therapy Passport", 1)]
        public void ExportCandidateAssessment_should_return_file_object_with_file_name_is_equal_to_expectedFileName(int candidateAssessmentId, string delegateName, string selfAssessmentName, int delegateUserID)
        {
            // Arrange
            var controller = new SupervisorController(
                   supervisorService,
                   commonService,
                   frameworkNotificationService,
                   selfAssessmentService,
                   frameworkService,
                   configService,
                   centreRegistrationPromptsService,
                   userService,
                   logger,
                   config,
                   searchSortFilterPaginateService,
                   multiPageFormService,
                   registrationService,
                   centresService,
                   emailGenerationService,
                   emailService,
                   candidateAssessmentDownloadFileService,
                   clockUtility,
                   pdfService,
                   courseCategoriesService
               );
            string expectedFileName = $"{((selfAssessmentName.Length > 30) ? selfAssessmentName.Substring(0, 30) : selfAssessmentName)} - {delegateName} - {clockUtility.UtcNow:yyyy-MM-dd}.xlsx";

            // Act
            var result = controller.ExportCandidateAssessment(candidateAssessmentId, delegateName, selfAssessmentName, delegateUserID) as FileResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedFileName, result!.FileDownloadName);
            });
        }
    }
}
