namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.IO;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class DelegateDownloadFileServiceTests
    {
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
            using var expectedWorkbook = new XLWorkbook(
                TestContext.CurrentContext.TestDirectory + "\\TestData\\DelegateUploadTest.xlsx"
            );

            A.CallTo(() => jobGroupsDataService.GetJobGroupsAlphabetical()).Returns(
                new[] { (2, "Doctor"), (3, "Health Professional"), (1, "Nursing") }
            );

            A.CallTo(() => userDataService.GetDelegateUserCardsByCentreId(2)).Returns(
                new List<DelegateUserCard>
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
                }
            );

            // When
            var resultBytes = delegateDownloadFileService.GetDelegateDownloadFileForCentre(2);
            using var resultsStream = new MemoryStream(resultBytes);
            using var resultWorkbook = new XLWorkbook(resultsStream);

            // Then
            using (new AssertionScope())
            {
                foreach (var resultWorksheet in resultWorkbook.Worksheets)
                {
                    var expectedWorksheet = expectedWorkbook.Worksheets.Worksheet(resultWorksheet.Name);
                    var cells = resultWorksheet.CellsUsed();

                    foreach (var cell in cells)
                    {
                        var expectedCell = expectedWorksheet.Cell(cell.Address);
                        cell.Value.Should().BeEquivalentTo(expectedCell.Value);
                    }
                }
            }
        }
    }
}
