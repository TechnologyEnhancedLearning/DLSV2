namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.IO;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using NUnit.Framework;

    public class DelegateDownloadFileServiceTests
    {
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
                AliasId = null,
                JobGroupId = 1
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
                AliasId = null,
                JobGroupId = 1
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
                AliasId = null,
                JobGroupId = 2
            }
        };

        private IDelegateDownloadFileService delegateDownloadFileService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void SetUp()
        {
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            userDataService = A.Fake<IUserDataService>();
            delegateDownloadFileService = new DelegateDownloadFileService(jobGroupsDataService, userDataService);
        }

        [Test]
        public void GetDelegateDownloadFileForCentre_returns_expected_excel_data()
        {
            // Given
            using var expectedWorkbook = new XLWorkbook(
                TestContext.CurrentContext.TestDirectory + DelegateUploadFileServiceTests.TestDelegateUploadRelativeFilePath
            );

            A.CallTo(() => jobGroupsDataService.GetJobGroupsAlphabetical()).Returns(
                JobGroupsTestHelper.GetDefaultJobGroupsAlphabetical()
            );

            A.CallTo(() => userDataService.GetDelegateUserCardsByCentreId(2)).Returns(delegateUserCards);

            // When
            var resultBytes = delegateDownloadFileService.GetDelegateDownloadFileForCentre(2);
            using var resultsStream = new MemoryStream(resultBytes);
            using var resultWorkbook = new XLWorkbook(resultsStream);

            // Then
            SpreadsheetTestHelper.AssertSpreadsheetsAreEquivalent(expectedWorkbook, resultWorkbook);
        }
    }
}
