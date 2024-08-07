﻿namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;    
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using NUnit.Framework;
    using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
    public class DelegateDownloadFileServiceTests
    {
        public const string TestAllDelegatesExportRelativeFilePath = "/TestData/AllDelegatesExportTest.xlsx";

        private readonly List<DelegateUserCard> delegateUserCards = new List<DelegateUserCard>
        {
            new DelegateUserCard
            {
                FirstName = "A",
                LastName = "Test",
                EmailAddress = null,
                CandidateNumber = "TT95",
                Answer1 = "xxxx",
                Answer2 = "xxxxxxxxx",
                Answer3 = null,
                Answer4 = null,
                Answer5 = null,
                Answer6 = null,
                Active = true,
                JobGroupId = 1,
                JobGroupName = "Nursing",
                Approved = true,
                Password = null,
                DateRegistered = new DateTime(2022, 3, 31),
                AdminId = null,
                HasBeenPromptedForPrn = false,
                ProfessionalRegistrationNumber = "should not appear"
            },
            new DelegateUserCard
            {
                FirstName = "Fake",
                LastName = "Person",
                EmailAddress = "Test@Test",
                CandidateNumber = "TU67",
                Answer1 = null,
                Answer2 = null,
                Answer3 = null,
                Answer4 = null,
                Answer5 = null,
                Answer6 = null,
                Active = true,
                JobGroupId = 1,
                JobGroupName = "Nursing",
                Approved = true,
                Password = "testpassword",
                DateRegistered = new DateTime(2022, 2, 28),
                AdminId = null,
                HasBeenPromptedForPrn = true,
                ProfessionalRegistrationNumber = null
            },
            new DelegateUserCard
            {
                FirstName = "Test",
                LastName = "User",
                EmailAddress = null,
                CandidateNumber = "TM323",
                Answer1 = "xx",
                Answer2 = "xxxxxxxx",
                Answer3 = null,
                Answer4 = null,
                Answer5 = null,
                Answer6 = null,
                Active = true,
                JobGroupId = 2,
                JobGroupName = "Doctor",
                Approved = true,
                Password = "testpassword",
                DateRegistered = new DateTime(2000, 1, 1),
                AdminId = 1,
                HasBeenPromptedForPrn = true,
                ProfessionalRegistrationNumber = "MammalHands"
            }
        };

        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private IDelegateDownloadFileService delegateDownloadFileService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IUserDataService userDataService = null!;
        private IConfiguration configuration = null!;
        [SetUp]
        public void SetUp()
        {
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            userDataService = A.Fake<IUserDataService>();
            configuration = A.Fake<IConfiguration>();
            A.CallTo(() => configuration["FeatureManagement:ExportQueryRowLimit"]).Returns("50");
            delegateDownloadFileService = new DelegateDownloadFileService(centreRegistrationPromptsService, jobGroupsDataService, userDataService, configuration);
        }

        [Test]
        public void GetDelegatesAndJobGroupDownloadFileForCentre_returns_expected_excel_data()
        {
            // Given
            A.CallTo(() => jobGroupsDataService.GetJobGroupsAlphabetical()).Returns(
                JobGroupsTestHelper.GetDefaultJobGroupsAlphabetical()
            );

            A.CallTo(() => userDataService.GetDelegateUserCardsByCentreId(2)).Returns(delegateUserCards);

            // When
            var resultBytes = delegateDownloadFileService.GetDelegatesAndJobGroupDownloadFileForCentre(2, false);
            using var resultsStream = new MemoryStream(resultBytes);
            using var resultWorkbook = new XLWorkbook(resultsStream);

            // Then
            using var expectedWorkbook = new XLWorkbook(
                TestContext.CurrentContext.TestDirectory + DelegateUploadFileServiceTests.TestDelegateUploadRelativeFilePath
            );
            SpreadsheetTestHelper.AssertSpreadsheetsAreEquivalent(expectedWorkbook, resultWorkbook);
        }

        [Test]
        public void GetAllDelegatesFileForCentre_returns_expected_excel_data()
        {
            // TODO: HEEDLS-810 - run the formatter once review complete.
            // Given
            const int centreId = 2;

            var centreRegistrationPrompts = new List<CentreRegistrationPrompt>
            {
                new CentreRegistrationPrompt(RegistrationField.CentreRegistrationField1, 1, "Role type", null, true),
                new CentreRegistrationPrompt(RegistrationField.CentreRegistrationField2, 2, "Manager", null, true),
                new CentreRegistrationPrompt(RegistrationField.CentreRegistrationField3, 3, "Base / office / place of work", null, true),
                new CentreRegistrationPrompt(RegistrationField.CentreRegistrationField4, 4, "Base / office / place of work", null, true),
                new CentreRegistrationPrompt(RegistrationField.CentreRegistrationField5, 5, "Contact telephone number", null, true),
            };
            A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId))
                .Returns(new CentreRegistrationPrompts(centreId, centreRegistrationPrompts));

            A.CallTo(() => userDataService.GetDelegateUserCardsByCentreId(2)).Returns(delegateUserCards);
            A.CallTo(() => userDataService.GetCountDelegateUserCardsForExportByCentreId("", "", "", 2, "", "", "", "", "", "", 0, null,"", "", "", "", "", "")).Returns(17);
            A.CallTo(() => userDataService.GetDelegateUserCardsForExportByCentreId("Test", "SearchableName", "Ascending",2,"Any", "Any", "Any", "Any", "Any", "Any",2, null,"Any", "Any", "Any", "Any", "Any", "Any", 10, 1)).Returns(delegateUserCards);
            // When
            var resultBytes = delegateDownloadFileService.GetAllDelegatesFileForCentre(2, null, "", GenericSortingHelper.Ascending, null);
            using var resultsStream = new MemoryStream(resultBytes);
            using var resultWorkbook = new XLWorkbook(resultsStream);

            // Then
            using var expectedWorkbook = new XLWorkbook(
                TestContext.CurrentContext.TestDirectory + TestAllDelegatesExportRelativeFilePath
            );
        }
    }
}
