namespace DigitalLearningSolutions.Web.Tests.Controllers.Support
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Services;
    
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers.SupervisorController;
    using DigitalLearningSolutions.Web.Controllers.Support;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Support.Faqs;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
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
        private IConfigDataService configDataService = null!;
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private IUserDataService userDataService = null!;
        private ILogger<SupervisorController> logger = null!;
        private IConfiguration config = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private IMultiPageFormService multiPageFormService = null!;
        private IRegistrationService registrationService = null!;
        private ICentresDataService centresDataService = null!;
        private IUserService userService = null!;
        private IEmailGenerationService emailGenerationService = null!;
        private IEmailService emailService = null!;
        private IClockUtility clockUtility = null!;
        private ICandidateAssessmentDownloadFileService candidateAssessmentDownloadFileService = null!;

        [SetUp]
        public void Setup()
        {
            supervisorService = A.Fake<ISupervisorService>();
            commonService = A.Fake<ICommonService>();
            frameworkNotificationService = A.Fake<IFrameworkNotificationService>();
            selfAssessmentService = A.Fake<ISelfAssessmentService>();
            frameworkService = A.Fake<IFrameworkService>();
            configDataService = A.Fake<IConfigDataService>();
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            userDataService = A.Fake<IUserDataService>();
            logger = A.Fake<ILogger<SupervisorController>>();
            config = A.Fake<IConfiguration>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
            multiPageFormService = A.Fake<IMultiPageFormService>();
            registrationService = A.Fake<IRegistrationService>();
            centresDataService = A.Fake<ICentresDataService>();
            userService = A.Fake<IUserService>();
            emailGenerationService = A.Fake<IEmailGenerationService>();
            emailService = A.Fake<IEmailService>();
            clockUtility = A.Fake<IClockUtility>();
            candidateAssessmentDownloadFileService = A.Fake<ICandidateAssessmentDownloadFileService>();

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
                   configDataService,
                   centreRegistrationPromptsService,
                   userDataService,
                   logger,
                   config,
                   searchSortFilterPaginateService,
                   multiPageFormService,
                   registrationService,
                   centresDataService,
                   userService,
                   emailGenerationService,
                   emailService,
                   candidateAssessmentDownloadFileService,
                   clockUtility
               );
            string expectedFileName = $"{((selfAssessmentName.Length > 30) ? selfAssessmentName.Substring(0, 29) : selfAssessmentName)}-{delegateName}-{clockUtility.UtcNow:yyyy-MM-dd}.xlsx";

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
