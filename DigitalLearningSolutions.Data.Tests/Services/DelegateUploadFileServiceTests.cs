namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class DelegateUploadFileServiceTests
    {
        private const int centreId = 101;
        private IDelegateUploadFileService delegateUploadFileService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IRegistrationDataService registrationDataService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void SetUp()
        {
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            A.CallTo(() => jobGroupsDataService.GetJobGroupsAlphabetical()).Returns(
                new[] { (2, "Doctor"), (3, "Health Professional"), (1, "Nursing") }
            );
            userDataService = A.Fake<IUserDataService>();
            registrationDataService = A.Fake<IRegistrationDataService>();
            delegateUploadFileService = new DelegateUploadFileService(
                jobGroupsDataService,
                userDataService,
                registrationDataService
            );
        }

        private IXLTable CreateTableFromData(IEnumerable<DelegateDataRow> data)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet();
            return worksheet.Cell(1, 1).InsertTable(data);
        }

        private DelegateDataRow SampleDelegateDataRow(
            string firstName = "A",
            string lastName = "Test",
            string emailAddress = "",
            string candidateNumber = "TT95",
            string answer1 = "xxxx",
            string answer2 = "xxxxxxxxx",
            string answer3 = "",
            string answer4 = "",
            string answer5 = "",
            string answer6 = "",
            string active = "True",
            string aliasId = "",
            string jobGroupId = "1"
        )
        {
            return new DelegateDataRow(
                candidateNumber,
                firstName,
                lastName,
                jobGroupId,
                active,
                answer1,
                answer2,
                answer3,
                answer4,
                answer5,
                answer6,
                aliasId,
                emailAddress
            );
        }

        private void ShouldNotCreateOrUpdateDelegate()
        {
            A.CallTo(() => userDataService.UpdateDelegateRecord(A<DelegateRecord>._))
                .MustNotHaveHappened();
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .MustNotHaveHappened();
        }

        private void ShouldJustHaveOneError(BulkUploadResult result)
        {
            result.Processed.Should().Be(1);
            result.Updated.Should().Be(0);
            result.Registered.Should().Be(0);
            result.Skipped.Should().Be(0);
            result.Errors.Should().HaveCount(1);
        }

        [Test]
        public void OpenDelegatesTable_returns_table_correctly()
        {
            // Given
            var stream = File.OpenRead(
                TestContext.CurrentContext.TestDirectory + "\\TestData\\DelegateUploadTest.xlsx"
            );
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

            // When
            var table = delegateUploadFileService.OpenDelegatesTable(file);

            // Then
            using (new AssertionScope())
            {
                var headers = table.Fields.Select(x => x.Name).ToList();
                headers[0].Should().Be("LastName");
                headers[1].Should().Be("FirstName");
                headers[2].Should().Be("DelegateID");
                headers[3].Should().Be("AliasID");
                headers[4].Should().Be("JobGroupID");
                headers[5].Should().Be("Answer1");
                headers[6].Should().Be("Answer2");
                headers[7].Should().Be("Answer3");
                headers[8].Should().Be("Answer4");
                headers[9].Should().Be("Answer5");
                headers[10].Should().Be("Answer6");
                headers[11].Should().Be("Active");
                headers[12].Should().Be("EmailAddress");
                table.RowCount().Should().Be(4);
                var row = table.Row(2);
                row.Cell(1).GetString().Should().Be("Person");
                row.Cell(2).GetString().Should().Be("Fake");
                row.Cell(3).GetString().Should().Be("TU67");
                row.Cell(4).GetString().Should().BeEmpty();
                row.Cell(5).GetString().Should().Be("1");
                row.Cell(6).GetString().Should().BeEmpty();
                row.Cell(7).GetString().Should().BeEmpty();
                row.Cell(8).GetString().Should().BeEmpty();
                row.Cell(9).GetString().Should().BeEmpty();
                row.Cell(10).GetString().Should().BeEmpty();
                row.Cell(11).GetString().Should().BeEmpty();
                row.Cell(12).GetString().Should().Be("True");
                row.Cell(13).GetString().Should().Be("Test@Test");
            }
        }

        [Test]
        public void OpenDelegatesTable_throws_exception_if_headers_dont_match()
        {
            // Given
            var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet();
            worksheet.Name = "DelegatesBulkUpload";
            var table = worksheet.Cell(1, 1).InsertTable(new[] { SampleDelegateDataRow() });
            table.Cell(1, 4).Value = "blah";
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var file = new FormFile(stream, 0, stream.Length, string.Empty, string.Empty);

            // Then
            Assert.Throws<InvalidHeadersException>(() => delegateUploadFileService.OpenDelegatesTable(file));
        }

        [Test]
        public void ProcessDelegateTable_has_jobgroup_error_for_invalid_jobgroup()
        {
            // Given
            var row = SampleDelegateDataRow(jobGroupId: "999");
            var table = CreateTableFromData(new[] { row });

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            ShouldNotCreateOrUpdateDelegate();
            ShouldJustHaveOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReasons.InvalidJobGroupId);
        }

        [Test]
        public void ProcessDelegateTable_has_lastname_error_for_missing_lastname()
        {
            // Given
            var row = SampleDelegateDataRow(lastName: "");
            var table = CreateTableFromData(new[] { row });

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            ShouldNotCreateOrUpdateDelegate();
            ShouldJustHaveOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReasons.InvalidLastName);
        }

        [Test]
        public void ProcessDelegateTable_has_firstname_error_for_missing_firstname()
        {
            // Given
            var row = SampleDelegateDataRow("");
            var table = CreateTableFromData(new[] { row });

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            ShouldNotCreateOrUpdateDelegate();
            ShouldJustHaveOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReasons.InvalidFirstName);
        }

        [Test]
        public void ProcessDelegateTable_has_active_error_for_invalid_active_status()
        {
            // Given
            var row = SampleDelegateDataRow(active: "hello");
            var table = CreateTableFromData(new[] { row });

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            ShouldNotCreateOrUpdateDelegate();
            ShouldJustHaveOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReasons.InvalidActive);
        }

        [Test]
        public void ProcessDelegateTable_searches_for_delegate_by_delegateId_if_specified()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = SampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row });

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            A.CallTo(() => userDataService.GetApprovedStatusFromCandidateNumber(delegateId, centreId))
                .MustHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_calls_update_if_delegate_found_by_delegateId()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = SampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row });
            A.CallTo(() => userDataService.GetApprovedStatusFromCandidateNumber(delegateId, centreId))
                .Returns(true);

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            A.CallTo(() => userDataService.UpdateDelegateRecord(A<DelegateRecord>._))
                .MustHaveHappened();
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_has_delegateId_error_if_delegate_not_found_by_delegateId()
        {
            // Given
            const string fakeDelegateId = "FAKE";
            var row = SampleDelegateDataRow(candidateNumber: fakeDelegateId);
            var table = CreateTableFromData(new[] { row });
            A.CallTo(() => userDataService.GetApprovedStatusFromCandidateNumber(fakeDelegateId, centreId))
                .Returns(null);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            result.Errors.Should().HaveCount(1);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReasons.NoRecordForDelegateId);
            ShouldNotCreateOrUpdateDelegate();
        }

        [Test]
        public void ProcessDelegateTable_searches_for_delegate_by_aliasId_if_delegateId_not_specified()
        {
            // Given
            const string aliasId = "ALIAS";
            var row = SampleDelegateDataRow(candidateNumber: "", aliasId: aliasId);
            var table = CreateTableFromData(new[] { row });

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            A.CallTo(() => userDataService.GetApprovedStatusFromCandidateNumber(A<string>._, A<int>._))
                .MustNotHaveHappened();
            A.CallTo(() => userDataService.GetApprovedStatusFromAliasId(aliasId, centreId))
                .MustHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_calls_update_if_delegate_found_by_aliasId()
        {
            // Given
            const string aliasId = "ALIAS";
            var row = SampleDelegateDataRow(candidateNumber: "", aliasId: aliasId);
            var table = CreateTableFromData(new[] { row });
            A.CallTo(() => userDataService.GetApprovedStatusFromAliasId(aliasId, centreId))
                .Returns(true);

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            A.CallTo(() => userDataService.UpdateDelegateRecord(A<DelegateRecord>._))
                .MustHaveHappened();
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_calls_create_if_delegate_not_found_by_aliasId()
        {
            // Given
            const string? aliasId = "ALIAS";
            var row = SampleDelegateDataRow(candidateNumber: "", aliasId: aliasId);
            var table = CreateTableFromData(new[] { row });
            A.CallTo(() => userDataService.GetApprovedStatusFromAliasId(aliasId, centreId))
                .Returns(null);

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            A.CallTo(() => userDataService.UpdateDelegateRecord(A<DelegateRecord>._))
                .MustNotHaveHappened();
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .MustHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_calls_create_if_delegateId_and_aliasId_not_specified()
        {
            // Given
            var row = SampleDelegateDataRow(candidateNumber: "", aliasId: "");
            var table = CreateTableFromData(new[] { row });

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            A.CallTo(() => userDataService.GetApprovedStatusFromCandidateNumber(A<string>._, A<int>._))
                .MustNotHaveHappened();
            A.CallTo(() => userDataService.GetApprovedStatusFromAliasId(A<string>._, A<int>._))
                .MustNotHaveHappened();
            A.CallTo(() => userDataService.UpdateDelegateRecord(A<DelegateRecord>._))
                .MustNotHaveHappened();
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .MustHaveHappened();
        }

        [TestCase(-1, BulkUploadResult.ErrorReasons.UnexpectedErrorForUpdate)]
        [TestCase(-2, BulkUploadResult.ErrorReasons.ParameterError)]
        [TestCase(-3, BulkUploadResult.ErrorReasons.AliasIdInUse)]
        [TestCase(-4, BulkUploadResult.ErrorReasons.UnexpectedErrorForUpdate)]
        [TestCase(-5, BulkUploadResult.ErrorReasons.EmailAddressInUse)]
        public void ProcessDelegateTable_has_correct_error_reason_based_on_update_return_value(
            int returnValue,
            BulkUploadResult.ErrorReasons expectedErrorReason
        )
        {
            // Given
            var table = CreateTableFromData(new[] { SampleDelegateDataRow() });
            A.CallTo(() => userDataService.GetApprovedStatusFromCandidateNumber(A<string>._, A<int>._))
                .Returns(true);
            A.CallTo(() => userDataService.UpdateDelegateRecord(A<DelegateRecord>._))
                .Returns(returnValue);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            ShouldJustHaveOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(expectedErrorReason);
        }

        [TestCase("-1", BulkUploadResult.ErrorReasons.UnexpectedErrorForCreate)]
        [TestCase("-2", BulkUploadResult.ErrorReasons.ParameterError)]
        [TestCase("-3", BulkUploadResult.ErrorReasons.AliasIdInUse)]
        [TestCase("-4", BulkUploadResult.ErrorReasons.EmailAddressInUse)]
        public void ProcessDelegateTable_has_correct_error_reason_based_on_create_return_value(
            string returnValue,
            BulkUploadResult.ErrorReasons expectedErrorReason
        )
        {
            // Given
            var row = SampleDelegateDataRow(candidateNumber: "", aliasId: "");
            var table = CreateTableFromData(new[] { row });
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .Returns(returnValue);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            ShouldJustHaveOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(expectedErrorReason);
        }

        [Test]
        public void ProcessDelegateTable_calls_update_with_correct_delegate_details()
        {
            // Given
            const bool approved = true;
            var row = SampleDelegateDataRow();
            var table = CreateTableFromData(new[] { row });
            A.CallTo(() => userDataService.GetApprovedStatusFromCandidateNumber(A<string>._, A<int>._))
                .Returns(approved);

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            A.CallTo(
                    () => userDataService.UpdateDelegateRecord(
                        A<DelegateRecord>.That.Matches(
                            record =>
                                record.CandidateNumber == row.DelegateID &&
                                record.CentreId == centreId &&
                                record.FirstName == row.FirstName &&
                                record.LastName == row.LastName &&
                                record.JobGroupId.ToString() == row.JobGroupID &&
                                record.Answer1 == row.Answer1 &&
                                record.Answer2 == row.Answer2 &&
                                record.Answer3 == row.Answer3 &&
                                record.Answer4 == row.Answer4 &&
                                record.Answer5 == row.Answer5 &&
                                record.Answer6 == row.Answer6 &&
                                record.Email == row.EmailAddress &&
                                record.AliasId == row.AliasID &&
                                record.Active.ToString() == row.Active &&
                                record.Approved == approved
                        )
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_calls_create_with_correct_delegate_details()
        {
            // Given
            const string? aliasId = "NEW ALIAS";
            var row = SampleDelegateDataRow(candidateNumber: "", aliasId: aliasId);
            var table = CreateTableFromData(new[] { row });

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            A.CallTo(
                    () => registrationDataService.RegisterDelegateByCentre(
                        A<DelegateRegistrationModel>.That.Matches(
                            model =>
                                model.FirstName == row.FirstName &&
                                model.LastName == row.LastName &&
                                model.JobGroup.ToString() == row.JobGroupID &&
                                model.Answer1 == row.Answer1 &&
                                model.Answer2 == row.Answer2 &&
                                model.Answer3 == row.Answer3 &&
                                model.Answer4 == row.Answer4 &&
                                model.Answer5 == row.Answer5 &&
                                model.Answer6 == row.Answer6 &&
                                model.Email == row.EmailAddress &&
                                model.AliasId == aliasId
                        )
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_calls_create_with_notifyDate_if_welcomeEmailDate_set()
        {
            // Given
            var welcomeEmailDate = new DateTime(3000, 01, 01);
            var row = SampleDelegateDataRow(candidateNumber: "", aliasId: "");
            var table = CreateTableFromData(new[] { row });

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, centreId, welcomeEmailDate);

            // Then
            A.CallTo(
                    () => registrationDataService.RegisterDelegateByCentre(
                        A<DelegateRegistrationModel>.That.Matches(model => model.NotifyDate == welcomeEmailDate)
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_calls_create_without_notifyDate_if_welcomeEmailDate_not_set()
        {
            // Given
            var row = SampleDelegateDataRow(candidateNumber: "", aliasId: "");
            var table = CreateTableFromData(new[] { row });

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            A.CallTo(
                    () => registrationDataService.RegisterDelegateByCentre(
                        A<DelegateRegistrationModel>.That.Matches(model => !model.NotifyDate.HasValue)
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_counts_updated_correctly()
        {
            // Given
            var row = SampleDelegateDataRow();
            var table = CreateTableFromData(new[] { row, row, row, row, row });
            A.CallTo(() => userDataService.GetApprovedStatusFromCandidateNumber(A<string>._, A<int>._))
                .Returns(true);
            A.CallTo(() => userDataService.UpdateDelegateRecord(A<DelegateRecord>._))
                .Returns(0);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            result.Processed.Should().Be(5);
            result.Updated.Should().Be(5);
        }

        [Test]
        public void ProcessDelegateTable_counts_skipped_correctly()
        {
            // Given
            var row = SampleDelegateDataRow();
            var table = CreateTableFromData(new[] { row, row, row, row, row });
            A.CallTo(() => userDataService.GetApprovedStatusFromCandidateNumber(A<string>._, A<int>._))
                .Returns(true);
            A.CallTo(() => userDataService.UpdateDelegateRecord(A<DelegateRecord>._))
                .Returns(1);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            result.Processed.Should().Be(5);
            result.Skipped.Should().Be(5);
        }

        [Test]
        public void ProcessDelegateTable_counts_registered_from_update_correctly()
        {
            // Given
            var row = SampleDelegateDataRow();
            var table = CreateTableFromData(new[] { row, row, row, row, row });
            A.CallTo(() => userDataService.GetApprovedStatusFromCandidateNumber(A<string>._, A<int>._))
                .Returns(true);
            A.CallTo(() => userDataService.UpdateDelegateRecord(A<DelegateRecord>._))
                .Returns(2);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            result.Processed.Should().Be(5);
            result.Registered.Should().Be(5);
        }

        [Test]
        public void ProcessDelegateTable_counts_registered_from_create_correctly()
        {
            // Given
            var row = SampleDelegateDataRow(candidateNumber: "", aliasId: "");
            var table = CreateTableFromData(new[] { row, row, row, row, row });
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .Returns("ANY");

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            result.Processed.Should().Be(5);
            result.Registered.Should().Be(5);
        }

        [Test]
        public void ProcessDelegateTable_counts_mixed_outcomes_correctly()
        {
            // Given
            var errorRow = SampleDelegateDataRow(jobGroupId: "");
            var registerRow = SampleDelegateDataRow(candidateNumber: "", aliasId: "");
            var updateRow = SampleDelegateDataRow(candidateNumber: "UPDATE ME");
            var skipRow = SampleDelegateDataRow(candidateNumber: "SKIP ME");
            var data = new List<DelegateDataRow>
            {
                updateRow, skipRow, registerRow, errorRow, registerRow, skipRow, updateRow, skipRow, updateRow,
                updateRow
            };
            var table = CreateTableFromData(data);

            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .Returns("ANY");
            A.CallTo(() => userDataService.GetApprovedStatusFromCandidateNumber(A<string>._, A<int>._))
                .Returns(true);
            A.CallTo(
                    () => userDataService.UpdateDelegateRecord(
                        A<DelegateRecord>.That.Matches(record => record.CandidateNumber == "UPDATE ME")
                    )
                )
                .Returns(0);
            A.CallTo(
                    () => userDataService.UpdateDelegateRecord(
                        A<DelegateRecord>.That.Matches(record => record.CandidateNumber == "SKIP ME")
                    )
                )
                .Returns(1);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, centreId);

            // Then
            result.Processed.Should().Be(10);
            result.Updated.Should().Be(4);
            result.Skipped.Should().Be(3);
            result.Registered.Should().Be(2);
            result.Errors.Should().HaveCount(1);
        }

        private class DelegateDataRow
        {
            public DelegateDataRow(
                string candidateNumber,
                string firstName,
                string lastName,
                string jobGroupId,
                string active,
                string answer1,
                string answer2,
                string answer3,
                string answer4,
                string answer5,
                string answer6,
                string aliasId,
                string emailAddress
            )
            {
                DelegateID = candidateNumber;
                FirstName = firstName;
                LastName = lastName;
                JobGroupID = jobGroupId;
                Active = active;
                Answer1 = answer1;
                Answer2 = answer2;
                Answer3 = answer3;
                Answer4 = answer4;
                Answer5 = answer5;
                Answer6 = answer6;
                AliasID = aliasId;
                EmailAddress = emailAddress;
            }

            public string DelegateID { get; }
            public string FirstName { get; }
            public string LastName { get; }
            public string JobGroupID { get; }
            public string Active { get; }
            public string Answer1 { get; }
            public string Answer2 { get; }
            public string Answer3 { get; }
            public string Answer4 { get; }
            public string Answer5 { get; }
            public string Answer6 { get; }
            public string AliasID { get; }
            public string EmailAddress { get; }
        }
    }
}
