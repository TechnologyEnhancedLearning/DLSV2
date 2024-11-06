namespace DigitalLearningSolutions.Web.Tests.Controllers.Support
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers.SupervisorController;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.Supervisor;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using GDS.MultiPageFormData;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    public class SupervisorControllerTests
    {
        private const int DelegateUserId = 11;
        private const int SelfAssessmentId = 1;
        private const int CentreId = 2;
        public const int AdminId = 7;
        public const string EmailAddress = "email";
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
        private SupervisorController controller = null!;

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
            A.CallTo(() => candidateAssessmentDownloadFileService.GetCandidateAssessmentDownloadFileForCentre(A<int>._, A<int>._, A<bool>._))
                .Returns(new byte[] { });

            var user = new ClaimsPrincipal(
               new ClaimsIdentity(
                   new[]
                   {
                        new Claim("UserCentreID", CentreId.ToString()),
                        new Claim("UserId", DelegateUserId.ToString()),
                        new Claim("UserAdminId", AdminId.ToString())
                   },
                   "mock"
               )
           );

            controller = new SupervisorController(
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
               pdfService
           );
            controller.ControllerContext = new ControllerContext
            { HttpContext = new DefaultHttpContext { User = user } };
            controller = controller.WithMockTempData();
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
                   pdfService
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


        [Test]
        public void ReviewDelegateSelfAssessment_Should_Return_View_With_Optional_Competency()
        {
            // Given
            int candidateAssessmentId = 1;
            int supervisorDelegateId = 2;
            var superviseDelegate = SupervisorTagTestHelper.CreateDefaultSupervisorDelegateDetail();
            var delegateSelfAssessment = SupervisorTagTestHelper.CreateDefaultDelegateSelfAssessment();
            var appliedFilterViewModel = new List<AppliedFilterViewModel>();
            var competencySummaries = new CompetencySummary();
            var search = new SearchSupervisorCompetencyViewModel();
            var competencies = new List<Competency>
     {
         new Competency { CompetencyGroup = "A", Id = 1, CompetencyGroupID = 1,SelfAssessmentStructureId=1, Optional = true },
         new Competency { CompetencyGroup = "A", Id = 2, CompetencyGroupID = 1,SelfAssessmentStructureId=1, Optional = false },
     };
            var expectedCompetencyGroups = competencies.GroupBy(c => c.CompetencyGroup).ToList();
            var supervisorSignOffs = new List<SupervisorSignOff>();
            var expectedModel = new ReviewSelfAssessmentViewModel()
            {
                SupervisorDelegateDetail = superviseDelegate,
                DelegateSelfAssessment = delegateSelfAssessment,
                CompetencyGroups = expectedCompetencyGroups,
                IsSupervisorResultsReviewed = delegateSelfAssessment.IsSupervisorResultsReviewed,
                SearchViewModel = search,
                CandidateAssessmentId = candidateAssessmentId,
                ExportToExcelHide = delegateSelfAssessment.SupervisorRoleTitle?.Contains("Assessor") ?? false,
                SupervisorSignOffs = supervisorSignOffs,
                CompetencySummaries = competencySummaries
            };
            var loggedInAdmin = UserTestHelper.GetDefaultAdminEntity();
            A.CallTo(() => userService.GetAdminById(loggedInAdmin.AdminAccount.Id)).Returns(loggedInAdmin);

            A.CallTo(() => supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, AdminId, 0))
                .Returns(superviseDelegate);
            A.CallTo(() => supervisorService.GetSelfAssessmentByCandidateAssessmentId(candidateAssessmentId, AdminId))
                 .Returns(delegateSelfAssessment);
            A.CallTo(() => selfAssessmentService.GetMostRecentResults(SelfAssessmentId, DelegateUserId))
                .Returns(competencies);

            // When
            var result = controller.ReviewDelegateSelfAssessment(supervisorDelegateId, candidateAssessmentId, SelfAssessmentId);

            // Then
            result.Should().BeViewResult().ModelAs<ReviewSelfAssessmentViewModel>();

            result.Should().BeViewResult()
            .WithViewName("ReviewSelfAssessment")
            .ModelAs<ReviewSelfAssessmentViewModel>()
            .CompetencyGroups ?.SelectMany(group => group).FirstOrDefault(x => x.Id == 1)?.Optional.Should().Be(true);
            result.Should().BeViewResult()
           .WithViewName("ReviewSelfAssessment")
           .ModelAs<ReviewSelfAssessmentViewModel>()
           .CompetencyGroups?.SelectMany(group => group).FirstOrDefault(x => x.Id == 2)?.Optional.Should().Be(false);
        }
        [Test]
        public void ReviewDelegateSelfAssessment_Should_Return_View_With_Optional_Filter_Applied()
        {
            // Given
            int candidateAssessmentId = 1;
            int supervisorDelegateId = 2;
            var superviseDelegate = SupervisorTagTestHelper.CreateDefaultSupervisorDelegateDetail();
            var delegateSelfAssessment = SupervisorTagTestHelper.CreateDefaultDelegateSelfAssessment();
            SearchSupervisorCompetencyViewModel searchModel = null!;
            var appliedFilterViewModel = new List<AppliedFilterViewModel>
            {
                new AppliedFilterViewModel{DisplayText = "Optional", FilterCategory =  null!, FilterValue ="-4", TagClass = ""},
            };
            var searchViewModel = searchModel == null ? new SearchSupervisorCompetencyViewModel(supervisorDelegateId, searchModel?.SearchText!, delegateSelfAssessment.ID, delegateSelfAssessment.IsSupervisorResultsReviewed, false, null!, null!)
                : searchModel.Initialise(searchModel.AppliedFilters, null!, delegateSelfAssessment.IsSupervisorResultsReviewed, false);

            var competencySummaries = new CompetencySummary();
            var competencies = new List<Competency>
            {
         new Competency { CompetencyGroup = "A", Id = 1, CompetencyGroupID = 1,SelfAssessmentStructureId=1, Optional = true },
         new Competency { CompetencyGroup = "A", Id = 2, CompetencyGroupID = 1,SelfAssessmentStructureId=1, Optional = false },
      };
            var expectedCompetencyGroups = competencies.GroupBy(c => c.CompetencyGroup).ToList();
            var supervisorSignOffs = new List<SupervisorSignOff>();
            var expectedModel = new ReviewSelfAssessmentViewModel()
            {
                SupervisorDelegateDetail = superviseDelegate,
                DelegateSelfAssessment = delegateSelfAssessment,
                CompetencyGroups = expectedCompetencyGroups,
                IsSupervisorResultsReviewed = delegateSelfAssessment.IsSupervisorResultsReviewed,
                SearchViewModel = searchViewModel,
                CandidateAssessmentId = candidateAssessmentId,
                ExportToExcelHide = delegateSelfAssessment.SupervisorRoleTitle?.Contains("Assessor") ?? false,
                SupervisorSignOffs = supervisorSignOffs,
                CompetencySummaries = competencySummaries
            };
            var loggedInAdmin = UserTestHelper.GetDefaultAdminEntity();
            A.CallTo(() => userService.GetAdminById(loggedInAdmin.AdminAccount.Id)).Returns(loggedInAdmin);

            A.CallTo(() => supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, AdminId, 0))
                .Returns(superviseDelegate);
            A.CallTo(() => supervisorService.GetSelfAssessmentByCandidateAssessmentId(candidateAssessmentId, AdminId))
                 .Returns(delegateSelfAssessment);
            A.CallTo(() => selfAssessmentService.GetMostRecentResults(SelfAssessmentId, DelegateUserId))
                .Returns(competencies);

            // When
            var result = controller.ReviewDelegateSelfAssessment(supervisorDelegateId, candidateAssessmentId, SelfAssessmentId, searchViewModel);

            // Then
            result.Should().BeViewResult().ModelAs<ReviewSelfAssessmentViewModel>();

            var competencyGroups = result.Should().BeViewResult()
            .WithViewName("ReviewSelfAssessment")
            .ModelAs<ReviewSelfAssessmentViewModel>()
            .CompetencyGroups?.Should().NotBeNull();
        }

    }
}
