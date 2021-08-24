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
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    // Note that all tests in this file test the internal methods of DelegateUploadFileService
    // so that we don't have to have several Excel files to test each case via the public interface.
    // This is achieved via the InternalsVisibleTo attribute in DelegateUploadFileService.cs
    // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.internalsvisibletoattribute?view=netcore-3.1
    public class DelegateUploadFileServiceTests
    {
        private const int CentreId = 101;
        public const string TestDelegateUploadRelativeFilePath = "\\TestData\\DelegateUploadTest.xlsx";
        
        private DelegateUploadFileService delegateUploadFileService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IRegistrationDataService registrationDataService = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            jobGroupsDataService = A.Fake<IJobGroupsDataService>(x => x.Strict());
            A.CallTo(() => jobGroupsDataService.GetJobGroupsAlphabetical()).Returns(
                JobGroupsTestHelper.GetDefaultJobGroupsAlphabetical()
            );

            userDataService = A.Fake<IUserDataService>(x => x.Strict());
            userService = A.Fake<IUserService>(x => x.Strict());
            registrationDataService = A.Fake<IRegistrationDataService>(x => x.Strict());

            delegateUploadFileService = new DelegateUploadFileService(
                jobGroupsDataService,
                userDataService,
                registrationDataService,
                userService
            );
        }

        [Test]
        public void OpenDelegatesTable_returns_table_correctly()
        {
            // Given
            var stream = File.OpenRead(
                TestContext.CurrentContext.TestDirectory + TestDelegateUploadRelativeFilePath
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
        public void OpenDelegatesTable_throws_exception_if_headers_are_incorrect()
        {
            // Given
            using var stream = CreateWorkbookStreamWithInvalidHeaders();
            var file = new FormFile(stream, 0, stream.Length, string.Empty, string.Empty);

            // Then
            Assert.Throws<InvalidHeadersException>(() => delegateUploadFileService.OpenDelegatesTable(file));
        }

        [Test]
        public void ProcessDelegateTable_has_job_group_error_for_invalid_job_group()
        {
            // Given
            var row = GetSampleDelegateDataRow(jobGroupId: "999");
            var table = CreateTableFromData(new[] { row });

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReason.InvalidJobGroupId);
        }

        [Test]
        public void ProcessDelegateTable_has_lastname_error_for_missing_lastname()
        {
            // Given
            var row = GetSampleDelegateDataRow(lastName: string.Empty);
            var table = CreateTableFromData(new[] { row });

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReason.InvalidLastName);
        }

        [Test]
        public void ProcessDelegateTable_has_firstname_error_for_missing_firstname()
        {
            // Given
            var row = GetSampleDelegateDataRow(string.Empty);
            var table = CreateTableFromData(new[] { row });

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReason.InvalidFirstName);
        }

        [Test]
        public void ProcessDelegateTable_has_email_error_for_missing_email()
        {
            // Given
            var row = GetSampleDelegateDataRow(emailAddress: string.Empty);
            var table = CreateTableFromData(new[] { row });

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReason.InvalidEmail);
        }

        [Test]
        public void ProcessDelegateTable_has_active_error_for_invalid_active_status()
        {
            // Given
            var row = GetSampleDelegateDataRow(active: "hello");
            var table = CreateTableFromData(new[] { row });

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReason.InvalidActive);
        }

        [Test]
        public void ProcessDelegateTable_has_no_delegate_error_if_delegateId_provided_but_no_record_found()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row });
            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(delegateId, CentreId)).Returns(null);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReason.NoRecordForDelegateId);
        }

        [Test]
        public void
            ProcessDelegateTable_has_alias_in_use_error_if_alias_id_matches_different_user()
        {
            // Given
            const string delegateId = "DELEGATE";
            const string aliasId = "ALIAS";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId, aliasId: aliasId);
            var table = CreateTableFromData(new[] { row });

            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateUser(candidateNumber: delegateId);
            var aliasIdDelegate = UserTestHelper.GetDefaultDelegateUser(aliasId: aliasId);

            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(delegateId, CentreId))
                .Returns(candidateNumberDelegate);
            A.CallTo(() => userDataService.GetDelegateUserByAliasId(aliasId, CentreId)).Returns(aliasIdDelegate);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReason.AliasIdInUse);
        }

        [Test]
        public void
            ProcessDelegateTable_has_email_in_use_error_if_delegate_is_found_by_delegateId_but_email_exists_on_another_delegate()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateUser(
                candidateNumber: delegateId,
                emailAddress: "different@test.com"
            );

            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(delegateId, CentreId))
                .Returns(candidateNumberDelegate);
            A.CallTo(() => userService.IsEmailValidForCentre("email@test.com", CentreId)).Returns(false);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReason.EmailAddressInUse);
        }

        [Test]
        public void ProcessDelegateTable_has_does_not_check_email_if_delegate_found_by_delegateId_has_matching_email()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateUser(candidateNumber: delegateId);

            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(delegateId, CentreId))
                .Returns(candidateNumberDelegate);
            ACallToUserDataServiceUpdateDelegateDoesNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            result.ProcessedCount.Should().Be(1);
            result.UpdatedCount.Should().Be(1);
        }

        [Test]
        public void
            ProcessDelegateTable_has_email_in_use_error_if_delegate_is_found_by_alias_but_email_exists_on_another_delegate()
        {
            // Given
            const string delegateId = "DELEGATE";
            const string aliasId = "ALIAS";
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty, aliasId: aliasId);
            var table = CreateTableFromData(new[] { row });
            var aliasIdDelegate = UserTestHelper.GetDefaultDelegateUser(
                candidateNumber: delegateId,
                emailAddress: "different@test.com"
            );

            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(delegateId, CentreId)).Returns(null);
            A.CallTo(() => userDataService.GetDelegateUserByAliasId(aliasId, CentreId)).Returns(aliasIdDelegate);
            A.CallTo(() => userService.IsEmailValidForCentre("email@test.com", CentreId)).Returns(false);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReason.EmailAddressInUse);
        }

        [Test]
        public void ProcessDelegateTable_has_does_not_check_email_if_delegate_found_by_alias_has_matching_email()
        {
            // Given
            const string delegateId = "DELEGATE";
            const string aliasId = "ALIAS";
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty, aliasId: aliasId);
            var table = CreateTableFromData(new[] { row });
            var aliasIdDelegate = UserTestHelper.GetDefaultDelegateUser(candidateNumber: delegateId);

            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(delegateId, CentreId)).Returns(null);
            A.CallTo(() => userDataService.GetDelegateUserByAliasId(aliasId, CentreId)).Returns(aliasIdDelegate);
            ACallToUserDataServiceUpdateDelegateDoesNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            result.ProcessedCount.Should().Be(1);
            result.UpdatedCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_skips_updating_delegate_found_by_delegateId_if_all_details_match()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateUser(
                firstName: row.FirstName,
                lastName: row.LastName,
                candidateNumber: delegateId,
                answer1: row.Answer1,
                answer2: row.Answer2,
                active: true,
                jobGroupId: 1
            );

            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(delegateId, CentreId))
                .Returns(candidateNumberDelegate);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            AssertCreateOrUpdateDelegateWereNotCalled();
            result.ProcessedCount.Should().Be(1);
            result.SkippedCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_skips_updating_delegate_found_by_alias_if_all_details_match()
        {
            // Given
            const string delegateId = "DELEGATE";
            const string aliasId = "ALIAS";
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty, aliasId: aliasId);
            var table = CreateTableFromData(new[] { row });
            var aliasIdDelegate = UserTestHelper.GetDefaultDelegateUser(
                firstName: row.FirstName,
                lastName: row.LastName,
                candidateNumber: delegateId,
                answer1: row.Answer1,
                answer2: row.Answer2,
                active: true,
                jobGroupId: 1,
                aliasId: aliasId
            );

            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(delegateId, CentreId)).Returns(null);
            A.CallTo(() => userDataService.GetDelegateUserByAliasId(aliasId, CentreId)).Returns(aliasIdDelegate);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            AssertCreateOrUpdateDelegateWereNotCalled();
            A.CallTo(() => userService.IsEmailValidForCentre(A<string>._, CentreId)).MustNotHaveHappened();
            result.ProcessedCount.Should().Be(1);
            result.SkippedCount.Should().Be(1);
        }

        [Test]
        public void
            ProcessDelegateTable_has_email_in_use_error_if_delegate_not_found_by_alias_but_email_exists_on_another_delegate()
        {
            // Given
            const string aliasId = "ALIAS";
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty, aliasId: aliasId);
            var table = CreateTableFromData(new[] { row });

            A.CallTo(() => userDataService.GetDelegateUserByAliasId(aliasId, CentreId)).Returns(null);
            A.CallTo(() => userService.IsEmailValidForCentre("email@test.com", CentreId)).Returns(false);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors.First().RowNumber.Should().Be(2);
            result.Errors.First().Reason.Should().Be(BulkUploadResult.ErrorReason.EmailAddressInUse);
        }

        [Test]
        public void ProcessDelegateTable_calls_register_if_delegate_not_found_by_alias_and_email_is_unused()
        {
            // Given
            const string delegateId = "DELEGATE";
            const string aliasId = "ALIAS";
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty, aliasId: aliasId);
            var table = CreateTableFromData(new[] { row });

            A.CallTo(() => userDataService.GetDelegateUserByAliasId(aliasId, CentreId)).Returns(null);
            A.CallTo(() => userService.IsEmailValidForCentre("email@test.com", CentreId)).Returns(true);
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .Returns(delegateId);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            result.ProcessedCount.Should().Be(1);
            result.RegisteredCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_calls_register_if_delegateId_and_aliasId_are_empty_email_is_unused()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty, aliasId: string.Empty);
            var table = CreateTableFromData(new[] { row });

            A.CallTo(() => userService.IsEmailValidForCentre("email@test.com", CentreId)).Returns(true);
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .Returns(delegateId);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            result.ProcessedCount.Should().Be(1);
            result.RegisteredCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_calls_update_with_expected_values()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateUser(candidateNumber: delegateId);

            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(delegateId, CentreId))
                .Returns(candidateNumberDelegate);
            ACallToUserDataServiceUpdateDelegateDoesNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            A.CallTo(
                    () => userDataService.UpdateDelegate(
                        candidateNumberDelegate.Id,
                        row.FirstName,
                        row.LastName,
                        1,
                        true,
                        row.Answer1,
                        row.Answer2,
                        row.Answer3,
                        row.Answer4,
                        row.Answer5,
                        row.Answer6,
                        row.AliasID,
                        row.EmailAddress
                    )
                )
                .MustHaveHappened();
            result.ProcessedCount.Should().Be(1);
            result.UpdatedCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_calls_register_with_expected_values()
        {
            // Given
            const string delegateId = "DELEGATE";
            const string aliasId = "ALIAS";
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty, aliasId: aliasId);
            var table = CreateTableFromData(new[] { row });

            A.CallTo(() => userDataService.GetDelegateUserByAliasId(aliasId, CentreId)).Returns(null);
            A.CallTo(() => userService.IsEmailValidForCentre("email@test.com", CentreId)).Returns(true);
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .Returns(delegateId);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

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
                                model.AliasId == aliasId &&
                                model.NotifyDate == null
                        )
                    )
                )
                .MustHaveHappened();
            result.ProcessedCount.Should().Be(1);
            result.RegisteredCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_calls_register_with_expected_values_when_welcomeEmailDate_is_populates()
        {
            // Given
            var welcomeEmailDate = new DateTime(3000, 01, 01);
            const string delegateId = "DELEGATE";
            const string aliasId = "ALIAS";
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty, aliasId: aliasId);
            var table = CreateTableFromData(new[] { row });

            A.CallTo(() => userDataService.GetDelegateUserByAliasId(aliasId, CentreId)).Returns(null);
            A.CallTo(() => userService.IsEmailValidForCentre("email@test.com", CentreId)).Returns(true);
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .Returns(delegateId);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, welcomeEmailDate);

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
                                model.AliasId == aliasId &&
                                model.NotifyDate == welcomeEmailDate
                        )
                    )
                )
                .MustHaveHappened();
            result.ProcessedCount.Should().Be(1);
            result.RegisteredCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_counts_updated_correctly()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row, row, row, row, row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateUser(candidateNumber: delegateId);

            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(delegateId, CentreId))
                .Returns(candidateNumberDelegate);
            ACallToUserDataServiceUpdateDelegateDoesNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            result.ProcessedCount.Should().Be(5);
            result.UpdatedCount.Should().Be(5);
        }

        [Test]
        public void ProcessDelegateTable_counts_skipped_correctly()
        {
            // Given
            const string delegateId = "DELEGATE";
            const string aliasId = "ALIAS";
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty, aliasId: aliasId);
            var table = CreateTableFromData(new[] { row, row, row, row, row });
            var aliasIdDelegate = UserTestHelper.GetDefaultDelegateUser(
                firstName: row.FirstName,
                lastName: row.LastName,
                candidateNumber: delegateId,
                answer1: row.Answer1,
                answer2: row.Answer2,
                active: true,
                jobGroupId: 1,
                aliasId: aliasId
            );

            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(delegateId, CentreId)).Returns(null);
            A.CallTo(() => userDataService.GetDelegateUserByAliasId(aliasId, CentreId)).Returns(aliasIdDelegate);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            result.ProcessedCount.Should().Be(5);
            result.SkippedCount.Should().Be(5);
        }

        [Test]
        public void ProcessDelegateTable_counts_registered_correctly()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty, aliasId: string.Empty);
            var table = CreateTableFromData(new[] { row, row, row, row, row });

            A.CallTo(() => userService.IsEmailValidForCentre("email@test.com", CentreId)).Returns(true);
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .Returns(delegateId);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            result.ProcessedCount.Should().Be(5);
            result.RegisteredCount.Should().Be(5);
        }

        [Test]
        public void ProcessDelegateTable_counts_mixed_outcomes_correctly()
        {
            // Given
            const string updateDelegateId = "UPDATE ME";
            const string skipDelegateId = "SKIP ME";
            var errorRow = GetSampleDelegateDataRow(jobGroupId: string.Empty);
            var registerRow = GetSampleDelegateDataRow(candidateNumber: string.Empty, aliasId: string.Empty);
            var updateRow = GetSampleDelegateDataRow(candidateNumber: updateDelegateId);
            var skipRow = GetSampleDelegateDataRow(candidateNumber: skipDelegateId);
            var data = new List<DelegateDataRow>
            {
                updateRow, skipRow, registerRow, errorRow, registerRow, skipRow, updateRow, skipRow, updateRow,
                updateRow
            };
            var table = CreateTableFromData(data);

            var updateDelegate = UserTestHelper.GetDefaultDelegateUser(candidateNumber: updateDelegateId);
            var skipDelegate = UserTestHelper.GetDefaultDelegateUser(
                firstName: skipRow.FirstName,
                lastName: skipRow.LastName,
                candidateNumber: skipRow.DelegateID,
                answer1: skipRow.Answer1,
                answer2: skipRow.Answer2,
                active: true,
                jobGroupId: 1
            );

            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(skipDelegateId, CentreId)).Returns(skipDelegate);
            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(updateDelegateId, CentreId))
                .Returns(updateDelegate);

            A.CallTo(() => userService.IsEmailValidForCentre("email@test.com", CentreId)).Returns(true);

            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .Returns("ANY");
            ACallToUserDataServiceUpdateDelegateDoesNothing();
            
            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId);

            // Then
            using (new AssertionScope())
            {
                result.ProcessedCount.Should().Be(10);
                result.UpdatedCount.Should().Be(4);
                result.SkippedCount.Should().Be(3);
                result.RegisteredCount.Should().Be(2);
                result.Errors.Should().HaveCount(1);
            }
        }

        private IXLTable CreateTableFromData(IEnumerable<DelegateDataRow> data)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet();
            return worksheet.Cell(1, 1).InsertTable(data);
        }

        private MemoryStream CreateWorkbookStreamWithInvalidHeaders()
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet();
            worksheet.Name = "DelegatesBulkUpload";
            var table = worksheet.Cell(1, 1).InsertTable(new[] { GetSampleDelegateDataRow() });
            table.Cell(1, 4).Value = "blah";
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream;
        }

        private DelegateDataRow GetSampleDelegateDataRow(
            string firstName = "A",
            string lastName = "Test",
            string emailAddress = "email@test.com",
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

        private void AssertCreateOrUpdateDelegateWereNotCalled()
        {
            A.CallTo(
                    () => userDataService.UpdateDelegate(
                        A<int>._,
                        A<string>._,
                        A<string>._,
                        A<int>._,
                        A<bool>._,
                        A<string>._,
                        A<string>._,
                        A<string>._,
                        A<string>._,
                        A<string>._,
                        A<string>._,
                        A<string>._,
                        A<string>._
                    )
                )
                .MustNotHaveHappened();
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(A<DelegateRegistrationModel>._))
                .MustNotHaveHappened();
        }

        private void AssertBulkUploadResultHasOnlyOneError(BulkUploadResult result)
        {
            result.ProcessedCount.Should().Be(1);
            result.UpdatedCount.Should().Be(0);
            result.RegisteredCount.Should().Be(0);
            result.SkippedCount.Should().Be(0);
            result.Errors.Should().HaveCount(1);
        }

        private void ACallToUserDataServiceUpdateDelegateDoesNothing()
        {
            A.CallTo(
                () => userDataService.UpdateDelegate(
                    A<int>._,
                    A<string>._,
                    A<string>._,
                    A<int>._,
                    A<bool>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._
                )
            ).DoesNothing();
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
