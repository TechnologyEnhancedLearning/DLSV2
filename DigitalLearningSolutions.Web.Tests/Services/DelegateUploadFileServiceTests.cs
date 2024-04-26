﻿namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Data.Models.User;
    
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    // Note that all tests in this file test the internal methods of DelegateUploadFileService
    // so that we don't have to have several Excel files to test each case via the public interface.
    // This is achieved via the InternalsVisibleTo attribute in DelegateUploadFileService.cs
    // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.internalsvisibletoattribute?view=netcore-3.1
    public class DelegateUploadFileServiceTests
    {
        private const int CentreId = 101;
        public const string TestDelegateUploadRelativeFilePath = "/TestData/DelegateUploadTest.xlsx";
        private static readonly DateTime WelcomeEmailDate = new DateTime(3000, 1, 1);
        private static readonly (int, string, int) NewDelegateIdAndCandidateNumber = (5, "DELEGATE", 281054);
        private IClockUtility clockUtility = null!;
        private IConfiguration configuration = null!;

        private DelegateUploadFileService delegateUploadFileService = null!;
        private IGroupsService groupsService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IPasswordResetService passwordResetService = null!;
        private IRegistrationService registrationService = null!;
        private ISupervisorDelegateService supervisorDelegateService = null!;
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
            userService = A.Fake<IUserService>();
            registrationService = A.Fake<IRegistrationService>(x => x.Strict());
            supervisorDelegateService = A.Fake<ISupervisorDelegateService>();
            passwordResetService = A.Fake<IPasswordResetService>();
            groupsService = A.Fake<IGroupsService>();
            configuration = A.Fake<IConfiguration>();
            clockUtility = A.Fake<IClockUtility>();

            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(A<string>._))
                .Returns(UserTestHelper.GetDefaultDelegateEntity());

            A.CallTo(() => clockUtility.UtcNow).Returns(DateTime.UtcNow);

            delegateUploadFileService = new DelegateUploadFileService(
                jobGroupsDataService,
                userDataService,
                userService,
                registrationService,
                supervisorDelegateService,
                passwordResetService,
                groupsService,
                clockUtility,
                configuration
            );
        }

        [Test]
        public void OpenDelegatesTable_returns_table_correctly()
        {
            // Given
            var stream = File.OpenRead(
                TestContext.CurrentContext.TestDirectory + TestDelegateUploadRelativeFilePath
            );
            var file = new FormFile(stream, 0, stream.Length, null!, Path.GetFileName(stream.Name));

            // When
            var workbook = new XLWorkbook(TestContext.CurrentContext.TestDirectory + TestDelegateUploadRelativeFilePath);
            var table = delegateUploadFileService.OpenDelegatesTable(workbook);

            // Then
            using (new AssertionScope())
            {
                var headers = table.Fields.Select(x => x.Name).ToList();
                headers[0].Should().Be("DelegateID");
                headers[1].Should().Be("LastName");
                headers[2].Should().Be("FirstName");
                headers[3].Should().Be("JobGroupID");
                headers[4].Should().Be("JobGroup");
                headers[5].Should().Be("Answer1");
                headers[6].Should().Be("Answer2");
                headers[7].Should().Be("Answer3");
                headers[8].Should().Be("Answer4");
                headers[9].Should().Be("Answer5");
                headers[10].Should().Be("Answer6");
                headers[11].Should().Be("Active");
                headers[12].Should().Be("EmailAddress");
                headers[13].Should().Be("HasPRN");
                headers[14].Should().Be("PRN");
                table.RowCount().Should().Be(4);
                var row = table.Row(2);
                row.Cell(1).GetString().Should().Be("TU67");
                row.Cell(2).GetString().Should().Be("Person");
                row.Cell(3).GetString().Should().Be("Fake");
                row.Cell(4).GetString().Should().Be("1");
                row.Cell(5).GetString().Should().Be("Nursing");
                row.Cell(6).GetString().Should().BeEmpty();
                row.Cell(7).GetString().Should().BeEmpty();
                row.Cell(8).GetString().Should().BeEmpty();
                row.Cell(9).GetString().Should().BeEmpty();
                row.Cell(10).GetString().Should().BeEmpty();
                row.Cell(11).GetString().Should().BeEmpty();
                row.Cell(12).GetString().Should().Be("True");
                row.Cell(13).GetString().Should().Be("Test@Test");
                row.Cell(14).GetString().Should().Be("False");
                row.Cell(15).GetString().Should().BeEmpty();
            }
        }

        [Test]
        public void OpenDelegatesTable_throws_exception_if_headers_are_incorrect()
        {
            // Given
            var workbook = new XLWorkbook(CreateWorkbookStreamWithInvalidHeaders());
            // Then
            Assert.Throws<InvalidHeadersException>(() => delegateUploadFileService.OpenDelegatesTable(workbook));
        }

        [Test]
        public void PreProcessDelegateTable_has_job_group_error_for_invalid_job_group()
        {
            var row = GetSampleDelegateDataRow(jobGroupId: "999");
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.InvalidJobGroupId);
        }

        [Test]
        public void PreProcessDelegateTable_has_missing_lastname_error_for_missing_lastname()
        {
            var row = GetSampleDelegateDataRow(lastName: string.Empty);
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.MissingLastName);
        }

        [Test]
        public void PreProcessDelegateTable_has_missing_firstname_error_for_missing_firstname()
        {
            var row = GetSampleDelegateDataRow(string.Empty);
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.MissingFirstName);
        }

        [Test]
        public void PreProcessDelegateTable_has_missing_email_error_for_missing_email()
        {
            var row = GetSampleDelegateDataRow(emailAddress: string.Empty);
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.MissingEmail);
        }

        [Test]
        public void PreProcessDelegateTable_has_invalid_active_error_for_invalid_active_status()
        {
            var row = GetSampleDelegateDataRow(active: "hello");
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.InvalidActive);
        }

        [Test]
        public void PreProcessDelegateTable_has_too_long_firstname_error_for_too_long_firstname()
        {
            var row = GetSampleDelegateDataRow(new string('x', 251));
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.TooLongFirstName);
        }

        [Test]
        public void PreProcessDelegateTable_has_too_long_lastname_error_for_too_long_lastname()
        {
            var row = GetSampleDelegateDataRow(lastName: new string('x', 251));
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.TooLongLastName);
        }

        [Test]
        public void PreProcessDelegateTable_has_too_long_email_error_for_too_long_email()
        {
            var row = GetSampleDelegateDataRow(emailAddress: $"test@{new string('x', 250)}");
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.TooLongEmail);
        }

        [Test]
        public void PreProcessDelegateTable_has_too_long_answer1_error_for_too_long_answer1()
        {
            var row = GetSampleDelegateDataRow(answer1: new string('x', 101));
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.TooLongAnswer1);
        }

        [Test]
        public void PreProcessDelegateTable_has_too_long_answer2_error_for_too_long_answer2()
        {
            var row = GetSampleDelegateDataRow(answer2: new string('x', 101));
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.TooLongAnswer2);
        }

        [Test]
        public void PreProcessDelegateTable_has_too_long_answer3_error_for_too_long_answer3()
        {
            var row = GetSampleDelegateDataRow(answer3: new string('x', 101));
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.TooLongAnswer3);
        }

        [Test]
        public void PreProcessDelegateTable_has_too_long_answer4_error_for_too_long_answer4()
        {
            var row = GetSampleDelegateDataRow(answer4: new string('x', 101));
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.TooLongAnswer4);
        }

        [Test]
        public void PreProcessDelegateTable_has_too_long_answer5_error_for_too_long_answer5()
        {
            var row = GetSampleDelegateDataRow(answer5: new string('x', 101));
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.TooLongAnswer5);
        }

        [Test]
        public void PreProcessDelegateTable_has_too_long_answer6_error_for_too_long_answer6()
        {
            var row = GetSampleDelegateDataRow(answer6: new string('x', 101));
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.TooLongAnswer6);
        }

        [Test]
        public void PreProcessDelegateTable_has_bad_format_email_error_for_wrong_format_email()
        {
            var row = GetSampleDelegateDataRow(emailAddress: "bademail");
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.BadFormatEmail);
        }

        [Test]
        public void PreProcessDelegateTable_has_whitespace_in_email_error_for_email_with_whitespace()
        {
            var row = GetSampleDelegateDataRow(emailAddress: "white space@test.com");
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.WhitespaceInEmail);
        }

        [Test]
        public void PreProcessDelegateTable_has_missing_PRN_error_for_HasPRN_true_with_missing_PRN()
        {
            var row = GetSampleDelegateDataRow(hasPrn: true.ToString());
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.HasPrnButMissingPrnValue);
        }

        [Test]
        public void PreProcessDelegateTable_has_false_HasPRN_error_for_PRN_with_value_and_false_HasPRN()
        {
            var row = GetSampleDelegateDataRow(hasPrn: false.ToString(), prn: "PRN1234");
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.PrnButHasPrnIsFalse);
        }

        [Test]
        public void PreProcessDelegateTable_has_invalid_PRN_characters_error_for_PRN_with_invalid_characters()
        {
            var row = GetSampleDelegateDataRow(hasPrn: true.ToString(), prn: "^%£PRN");
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.InvalidPrnCharacters);
        }

        [Test]
        public void PreProcessDelegateTable_has_invalid_PRN_length_error_for_PRN_too_short()
        {
            var row = GetSampleDelegateDataRow(hasPrn: true.ToString(), prn: "PRN1");
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.InvalidPrnLength);
        }

        [Test]
        public void PreProcessDelegateTable_has_invalid_PRN_length_error_for_PRN_too_long()
        {
            var row = GetSampleDelegateDataRow(hasPrn: true.ToString(), prn: "PRNAboveAllowedLength");
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.InvalidPrnLength);
        }

        [Test]
        public void PreProcessDelegateTable_has_invalid_HasPRN_error_for_HasPRN_not_parsable_to_bool()
        {
            var row = GetSampleDelegateDataRow(hasPrn: "ThisDoesNotMatchTRUE");
            Test_PreProcessDelegateTable_row_has_error(row, BulkUploadResult.ErrorReason.InvalidHasPrnValue);
        }

        [Test]
        public void ProcessDelegateTable_has_no_delegate_error_if_delegateId_provided_but_no_record_found()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row });
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId)).Returns(null);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors?.First().RowNumber.Should().Be(2);
            result.Errors?.First().Reason.Should().Be(BulkUploadResult.ErrorReason.NoRecordForDelegateId);
        }

        [Test]
        public void ProcessDelegateTable_has_email_in_use_error_if_new_delegate_has_email_matching_existing_delegate()
        {
            var row = GetSampleDelegateDataRow(emailAddress: "email@centre.com", candidateNumber: "");
            var table = CreateTableFromData(new[] { row });
            A.CallTo(() => userService.EmailIsHeldAtCentre("email@centre.com", CentreId)).Returns(true);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            using (var _ = new AssertionScope())
            {
                AssertBulkUploadResultHasOnlyOneError(result);
                result.Errors?.First().RowNumber.Should().Be(2);
                result.Errors?.First().Reason.Should().Be(BulkUploadResult.ErrorReason.EmailAddressInUse);
                A.CallTo(() => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                    A<DelegateRegistrationModel>._,
                    A<bool>._,
                    A<bool>._
                )).MustNotHaveHappened();
            }
        }

        [Test]
        public void
            ProcessDelegateTable_has_email_in_use_error_if_delegate_is_found_by_delegateId_but_email_exists_on_another_delegate()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(
                candidateNumber: delegateId,
                primaryEmail: "different@test.com"
            );

            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);
            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(true);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors?.First().RowNumber.Should().Be(2);
            result.Errors?.First().Reason.Should().Be(BulkUploadResult.ErrorReason.EmailAddressInUse);
        }

        [Test]
        public void
            ProcessDelegateTable_does_not_have_email_in_use_error_if_delegate_found_by_delegateId_has_matching_email()
        {
            // Given
            const string delegateId = "DELEGATE";
            const string email = "centre@email.com";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId, emailAddress: email);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(
                candidateNumber: delegateId,
                userCentreDetailsId: 1,
                centreSpecificEmail: email
            );

            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);
            CallsToUserDataServiceUpdatesDoNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentre(A<string>._, A<int>._)
                )
                .MustNotHaveHappened();

            result.ProcessedCount.Should().Be(1);
            result.UpdatedActiveCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_skips_updating_delegate_found_by_delegateId_if_all_details_match()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId, prn: "PRN1234");
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(
                firstName: row.FirstName,
                lastName: row.LastName,
                candidateNumber: delegateId,
                answer1: row.Answer1,
                answer2: row.Answer2,
                active: true,
                jobGroupId: 1,
                hasBeenPromptedForPrn: true,
                professionalRegistrationNumber: row.PRN
            );

            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            AssertCreateOrUpdateDelegateWereNotCalled();
            result.ProcessedCount.Should().Be(1);
            result.SkippedCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_calls_register_if_delegateId_is_empty_and_email_is_unused()
        {
            // Given
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty);
            var table = CreateTableFromData(new[] { row });

            A.CallTo(() => userService.EmailIsHeldAtCentre("email@test.com", CentreId)).Returns(false);
            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                        A<DelegateRegistrationModel>._,
                        false,
                        true
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);
            A.CallTo(
                () => userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).DoesNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            result.ProcessedCount.Should().Be(1);
            result.RegisteredActiveCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_update_updates_delegate_account()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(candidateNumber: delegateId);

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);

            CallsToUserDataServiceUpdatesDoNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                    () => userDataService.UpdateDelegateAccount(
                        candidateNumberDelegate.DelegateAccount.Id,
                        true,
                        row.Answer1,
                        row.Answer2,
                        row.Answer3,
                        row.Answer4,
                        row.Answer5,
                        row.Answer6
                    )
                )
                .MustHaveHappened();

            A.CallTo(
                () => passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                    A<int>._,
                    A<string>._,
                    A<DateTime>._,
                    A<string>._
                )
            ).MustNotHaveHappened();

            result.ProcessedCount.Should().Be(1);
            result.UpdatedActiveCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_update_updates_user_details()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(candidateNumber: delegateId);

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);

            CallsToUserDataServiceUpdatesDoNothing();

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                () => userDataService.UpdateUserDetails(
                    row.FirstName,
                    row.LastName,
                    candidateNumberDelegate.UserAccount.PrimaryEmail,
                    int.Parse(row.JobGroupID),
                    candidateNumberDelegate.UserAccount.Id
                )
            ).MustHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_update_updates_centre_email_when_email_is_changed()
        {
            // Given
            const string delegateId = "DELEGATE";
            const string oldEmail = "old_email@test.com";
            const string newEmail = "new_email@test.com";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId, emailAddress: newEmail);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(
                candidateNumber: delegateId,
                centreSpecificEmail: oldEmail
            );

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre(newEmail, CentreId))
                .Returns(false);
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);

            CallsToUserDataServiceUpdatesDoNothing();

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                () => userDataService.SetCentreEmail(
                    candidateNumberDelegate.UserAccount.Id,
                    candidateNumberDelegate.DelegateAccount.CentreId,
                    newEmail,
                    A<DateTime?>._,
                    A<IDbTransaction?>._
                )
            ).MustHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_update_does_not_call_SetCentreEmail_when_email_is_unchanged()
        {
            // Given
            const string delegateId = "DELEGATE";
            const string email = "email@test.com";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId, emailAddress: email);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(
                candidateNumber: delegateId,
                centreSpecificEmail: email
            );

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre(email, CentreId))
                .Returns(false);
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);

            CallsToUserDataServiceUpdatesDoNothing();

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                () => userDataService.SetCentreEmail(
                    A<int>._,
                    A<int>._,
                    A<string>._,
                    null,
                    A<IDbTransaction?>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_update_updates_delegate_groups()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(candidateNumber: delegateId);

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);

            CallsToUserDataServiceUpdatesDoNothing();

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                () => groupsService.UpdateSynchronisedDelegateGroupsBasedOnUserChanges(
                    candidateNumberDelegate.DelegateAccount.Id,
                    A<AccountDetailsData>.That.Matches(
                        data =>
                            data.FirstName == row.FirstName &&
                            data.Surname == row.LastName &&
                            data.Email == candidateNumberDelegate.UserAccount.PrimaryEmail
                    ),
                    A<RegistrationFieldAnswers>.That.Matches(
                        answers =>
                            answers.CentreId == candidateNumberDelegate.DelegateAccount.CentreId &&
                            answers.JobGroupId == int.Parse(row.JobGroupID) &&
                            answers.Answer1 == row.Answer1 &&
                            answers.Answer2 == row.Answer2 &&
                            answers.Answer3 == row.Answer3 &&
                            answers.Answer4 == row.Answer4 &&
                            answers.Answer5 == row.Answer5 &&
                            answers.Answer6 == row.Answer6
                    ),
                    A<RegistrationFieldAnswers>.That.Matches(
                        answers =>
                            answers.CentreId == candidateNumberDelegate.DelegateAccount.CentreId &&
                            answers.JobGroupId == candidateNumberDelegate.UserAccount.JobGroupId &&
                            answers.Answer1 == candidateNumberDelegate.DelegateAccount.Answer1 &&
                            answers.Answer2 == candidateNumberDelegate.DelegateAccount.Answer2 &&
                            answers.Answer3 == candidateNumberDelegate.DelegateAccount.Answer3 &&
                            answers.Answer4 == candidateNumberDelegate.DelegateAccount.Answer4 &&
                            answers.Answer5 == candidateNumberDelegate.DelegateAccount.Answer5 &&
                            answers.Answer6 == candidateNumberDelegate.DelegateAccount.Answer6
                    ),
                    row.EmailAddress
                )
            ).MustHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_update_updates_delegate_PRN_if_HasPRN_is_true_and_PRN_has_value()
        {
            // Given
            const string delegateId = "DELEGATE";
            const string prn = "PRN1234";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId, hasPrn: true.ToString(), prn: prn);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(candidateNumber: delegateId);

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);
            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).DoesNothing();
            CallsToUserDataServiceUpdatesDoNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(
                        candidateNumberDelegate.DelegateAccount.Id,
                        prn,
                        true
                    )
            ).MustHaveHappened();
            result.ProcessedCount.Should().Be(1);
            result.UpdatedActiveCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_update_updates_delegate_PRN_if_HasPRN_is_false_and_PRN_does_not_have_value()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId, hasPrn: false.ToString());
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(candidateNumber: delegateId);

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);
            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).DoesNothing();
            CallsToUserDataServiceUpdatesDoNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(
                        candidateNumberDelegate.DelegateAccount.Id,
                        null,
                        true
                    )
            ).MustHaveHappened();
            result.ProcessedCount.Should().Be(1);
            result.UpdatedActiveCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_update_updates_delegate_PRN_if_HasPRN_is_null_and_PRN_has_value()
        {
            // Given
            const string delegateId = "DELEGATE";
            const string prn = "PRN1234";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId, hasPrn: null, prn: prn);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(candidateNumber: delegateId);

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);
            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).DoesNothing();
            CallsToUserDataServiceUpdatesDoNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(
                        candidateNumberDelegate.DelegateAccount.Id,
                        prn,
                        true
                    )
            ).MustHaveHappened();
            result.ProcessedCount.Should().Be(1);
            result.UpdatedActiveCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_update_updates_delegate_PRN_if_HasPRN_is_empty_and_PRN_does_not_have_value()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(candidateNumber: delegateId);

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);
            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).DoesNothing();
            CallsToUserDataServiceUpdatesDoNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(
                        candidateNumberDelegate.DelegateAccount.Id,
                        null,
                        false
                    )
            ).MustHaveHappened();
            result.ProcessedCount.Should().Be(1);
            result.UpdatedActiveCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_calls_register_with_expected_values()
        {
            // Given
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty);
            var table = CreateTableFromData(new[] { row });
            Guid primaryEmailIsGuid;

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                        A<DelegateRegistrationModel>._,
                        false,
                        true
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);
            A.CallTo(
                () => userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).DoesNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
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
                            model.ProfessionalRegistrationNumber == row.PRN &&
                            model.CentreSpecificEmail == row.EmailAddress &&
                            Guid.TryParse(model.PrimaryEmail, out primaryEmailIsGuid) &&
                            model.NotifyDate == WelcomeEmailDate &&
                            model.IsSelfRegistered == false &&
                            model.UserIsActive == true &&
                            model.CentreAccountIsActive == true &&
                            model.Approved == true &&
                            model.PasswordHash == null
                    ),
                    false,
                    true
                )
            ).MustHaveHappenedOnceExactly();

            A.CallTo(
                () => passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                    NewDelegateIdAndCandidateNumber.Item1,
                    configuration.GetAppRootPath(),
                    WelcomeEmailDate,
                    "DelegateBulkUpload_Refactor"
                )
            ).MustHaveHappenedOnceExactly();
            result.ProcessedCount.Should().Be(1);
            result.RegisteredActiveCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_calls_register_with_expected_values_when_welcomeEmailDate_is_populated()
        {
            // Given
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty);
            var table = CreateTableFromData(new[] { row });
            Guid primaryEmailIsGuid;

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                        A<DelegateRegistrationModel>._,
                        false,
                        true
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);
            A.CallTo(
                () => userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).DoesNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
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
                                model.ProfessionalRegistrationNumber == row.PRN &&
                                model.CentreSpecificEmail == row.EmailAddress &&
                                Guid.TryParse(model.PrimaryEmail, out primaryEmailIsGuid) &&
                                model.NotifyDate == WelcomeEmailDate
                        ),
                        false,
                        true
                    )
                )
                .MustHaveHappened();
            result.ProcessedCount.Should().Be(1);
            result.RegisteredActiveCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_makes_call_to_generate_welcome_email_when_welcomeEmailDate_is_populated()
        {
            // Given
            var welcomeEmailDate = new DateTime(3000, 01, 01);
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty);
            var table = CreateTableFromData(new[] { row });

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                        A<DelegateRegistrationModel>._,
                        false,
                        true
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);
            A.CallTo(
                () => userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).DoesNothing();
            A.CallTo(
                () => passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                    A<int>._,
                    A<string>._!,
                    A<DateTime>._,
                    A<string>._!
                )
            ).DoesNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, welcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                    () => passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                        A<int>._,
                        A<string>._!,
                        welcomeEmailDate,
                        A<string>._!
                    )
                )
                .MustHaveHappened();
            result.ProcessedCount.Should().Be(1);
            result.RegisteredActiveCount.Should().Be(1);
        }

        [Test]
        public void ProcessDelegateTable_successful_register_updates_supervisor_delegates()
        {
            // Given
            const string candidateNumber = "DELEGATE";
            const string delegateEmail = "email@test.com";
            const int newDelegateRecordId = 281054;

            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty);
            var table = CreateTableFromData(new[] { row });
            var supervisorDelegates = new List<SupervisorDelegate>
                { new SupervisorDelegate { ID = 8 }, new SupervisorDelegate { ID = 9 } };
            var supervisorDelegateIds = new List<int> { 8, 9 };
            var delegateEmailList = new List<string?> { delegateEmail };

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre(delegateEmail, CentreId))
                .Returns(false);
            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                        A<DelegateRegistrationModel>._,
                        false,
                        true
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);
            A.CallTo(
                () => userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).DoesNothing();
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(candidateNumber))
                .Returns(UserTestHelper.GetDefaultDelegateEntity(newDelegateRecordId));
            A.CallTo(
                () =>
                    supervisorDelegateService.GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
                        CentreId,
                        A<List<string?>>.That.IsSameSequenceAs(delegateEmailList)
                    )
            ).Returns(supervisorDelegates);
            A.CallTo(
                () =>
                    supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(A<IEnumerable<int>>._, A<int>._)
            ).DoesNothing();

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                        A<DelegateRegistrationModel>._,
                        false,
                        true
                    )
                )
                .MustHaveHappened();
            A.CallTo(
                () => supervisorDelegateService.GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
                    CentreId,
                    A<IEnumerable<string?>>.That.IsSameSequenceAs(delegateEmailList)
                )
            ).MustHaveHappened();
            A.CallTo(
                () => supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    A<List<int>>.That.IsSameSequenceAs(supervisorDelegateIds),
                    newDelegateRecordId
                )
            ).MustHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_successful_register_updates_delegate_PRN_if_HasPRN_is_true_and_PRN_has_value()
        {
            // Given
            const string candidateNumber = "DELEGATE";
            const int newDelegateRecordId = 5;
            const string prn = "PRN1234";
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty, hasPrn: true.ToString(), prn: prn);
            var table = CreateTableFromData(new[] { row });

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                        A<DelegateRegistrationModel>._,
                        false,
                        true
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(candidateNumber))
                .Returns(UserTestHelper.GetDefaultDelegateEntity(newDelegateRecordId));
            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).DoesNothing();

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                        A<DelegateRegistrationModel>._,
                        false,
                        true
                    )
                )
                .MustHaveHappened();
            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(newDelegateRecordId, prn, true)
            ).MustHaveHappened();
        }

        [Test]
        public void
            ProcessDelegateTable_successful_register_updates_delegate_PRN_if_HasPRN_is_false_and_PRN_does_not_have_value()
        {
            // Given
            const string candidateNumber = "DELEGATE";
            const int newDelegateRecordId = 5;
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty, hasPrn: false.ToString());
            var table = CreateTableFromData(new[] { row });

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                        A<DelegateRegistrationModel>._,
                        false,
                        true
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(candidateNumber))
                .Returns(UserTestHelper.GetDefaultDelegateEntity(newDelegateRecordId));
            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).DoesNothing();

            // When
            delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                        A<DelegateRegistrationModel>._,
                        false,
                        true
                    )
                )
                .MustHaveHappened();
            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(newDelegateRecordId, null, true)
            ).MustHaveHappened();
        }

        [Test]
        public void ProcessDelegateTable_counts_updated_correctly()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row, row, row, row, row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(candidateNumber: delegateId);

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);
            CallsToUserDataServiceUpdatesDoNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            result.ProcessedCount.Should().Be(5);
            result.UpdatedActiveCount.Should().Be(5);
        }

        [Test]
        public void ProcessDelegateTable_counts_skipped_correctly()
        {
            // Given
            const string delegateId = "DELEGATE";
            var row = GetSampleDelegateDataRow(candidateNumber: delegateId);
            var table = CreateTableFromData(new[] { row, row, row, row, row });
            var candidateNumberDelegate = UserTestHelper.GetDefaultDelegateEntity(
                firstName: row.FirstName,
                lastName: row.LastName,
                candidateNumber: delegateId,
                answer1: row.Answer1,
                answer2: row.Answer2,
                active: true,
                jobGroupId: 1
            );

            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(delegateId))
                .Returns(candidateNumberDelegate);

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            result.ProcessedCount.Should().Be(5);
            result.SkippedCount.Should().Be(5);
        }

        [Test]
        public void ProcessDelegateTable_counts_registered_correctly()
        {
            // Given
            var row = GetSampleDelegateDataRow(candidateNumber: string.Empty);
            var table = CreateTableFromData(new[] { row, row, row, row, row });

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);
            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                        A<DelegateRegistrationModel>._,
                        false,
                        true
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);
            A.CallTo(
                () => userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).DoesNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            result.ProcessedCount.Should().Be(5);
            result.RegisteredActiveCount.Should().Be(5);
        }

        [Test]
        public void ProcessDelegateTable_counts_mixed_outcomes_correctly()
        {
            // Given
            const string updateDelegateId = "UPDATE ME";
            const string skipDelegateId = "SKIP ME";
            var errorRow = GetSampleDelegateDataRow(jobGroupId: string.Empty);
            var registerRow = GetSampleDelegateDataRow(candidateNumber: string.Empty);
            var updateRow = GetSampleDelegateDataRow(candidateNumber: updateDelegateId);
            var skipRow = GetSampleDelegateDataRow(candidateNumber: skipDelegateId);
            var data = new List<DelegateDataRow>
            {
                updateRow, skipRow, registerRow, errorRow, registerRow, skipRow, updateRow, skipRow, updateRow,
                updateRow,
            };
            var table = CreateTableFromData(data);

            var updateDelegate = UserTestHelper.GetDefaultDelegateEntity(candidateNumber: updateDelegateId);
            var skipDelegate = UserTestHelper.GetDefaultDelegateEntity(
                firstName: skipRow.FirstName,
                lastName: skipRow.LastName,
                candidateNumber: skipRow.DelegateID,
                answer1: skipRow.Answer1,
                answer2: skipRow.Answer2,
                active: true,
                jobGroupId: 1
            );

            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(skipDelegateId))
                .Returns(skipDelegate);
            A.CallTo(() => userDataService.GetDelegateByCandidateNumber(updateDelegateId))
                .Returns(updateDelegate);

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email@test.com", CentreId))
                .Returns(false);

            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                        A<DelegateRegistrationModel>._,
                        false,
                        true
                    )
                )
                .Returns((2, "ANY", 61188));
            CallsToUserDataServiceUpdatesDoNothing();

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            using (new AssertionScope())
            {
                result.ProcessedCount.Should().Be(10);
                result.UpdatedActiveCount.Should().Be(4);
                result.SkippedCount.Should().Be(3);
                result.RegisteredActiveCount.Should().Be(2);
                result.Errors.Should().HaveCount(1);
            }
        }

        private void Test_PreProcessDelegateTable_row_has_error(
            DelegateDataRow row,
            BulkUploadResult.ErrorReason errorReason
        )
        {
            // Given
            var table = CreateTableFromData(new[] { row });

            // When
            var result = delegateUploadFileService.PreProcessDelegatesTable(table);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors?.First().RowNumber.Should().Be(2);
            result.Errors?.First().Reason.Should().Be(errorReason);
        }

        private void Test_ProcessDelegateTable_row_has_error(
            DelegateDataRow row,
            BulkUploadResult.ErrorReason errorReason
        )
        {
            // Given
            var table = CreateTableFromData(new[] { row });

            // When
            var result = delegateUploadFileService.ProcessDelegatesTable(table, CentreId, WelcomeEmailDate, 1, 250, true, 1, null);

            // Then
            AssertBulkUploadResultHasOnlyOneError(result);
            result.Errors?.First().RowNumber.Should().Be(2);
            result.Errors?.First().Reason.Should().Be(errorReason);
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
            string jobGroupId = "1",
            string? hasPrn = null,
            string? prn = null
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
                emailAddress,
                hasPrn,
                prn
            );
        }

        private void AssertCreateOrUpdateDelegateWereNotCalled()
        {
            A.CallTo(
                () => userDataService.UpdateUserDetails(
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();

            A.CallTo(
                () => userDataService.UpdateDelegateAccount(
                    A<int>._,
                    A<bool>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._
                )
            ).MustNotHaveHappened();

            A.CallTo(
                () => userDataService.SetCentreEmail(
                    A<int>._,
                    A<int>._,
                    A<string>._,
                    null,
                    A<IDbTransaction?>._
                )
            ).MustNotHaveHappened();

            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).MustNotHaveHappened();

            A.CallTo(
                () => groupsService.UpdateSynchronisedDelegateGroupsBasedOnUserChanges(
                    A<int>._,
                    A<AccountDetailsData>._,
                    A<RegistrationFieldAnswers>._,
                    A<RegistrationFieldAnswers>._,
                    A<string?>._
                )
            ).MustNotHaveHappened();

            A.CallTo(
                    () => registrationService.CreateAccountAndReturnCandidateNumberAndDelegateId(
                        A<DelegateRegistrationModel>._,
                        false,
                        A<bool>._
                    )
                )
                .MustNotHaveHappened();
        }

        private void AssertBulkUploadResultHasOnlyOneError(BulkUploadResult result)
        {
            result.ProcessedCount.Should().Be(1);
            result.UpdatedActiveCount.Should().Be(0);
            result.RegisteredActiveCount.Should().Be(0);
            result.SkippedCount.Should().Be(0);
            result.Errors.Should().HaveCount(1);
        }

        private void CallsToUserDataServiceUpdatesDoNothing()
        {
            A.CallTo(
                () => userDataService.UpdateUserDetails(
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<int>._,
                    A<int>._
                )
            ).DoesNothing();

            A.CallTo(
                () => userDataService.UpdateDelegateAccount(
                    A<int>._,
                    A<bool>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._
                )
            ).DoesNothing();

            A.CallTo(
                () => userDataService.SetCentreEmail(
                    A<int>._,
                    A<int>._,
                    A<string>._,
                    A<DateTime?>._,
                    A<IDbTransaction?>._
                )
            ).DoesNothing();

            A.CallTo(
                () =>
                    userDataService.UpdateDelegateProfessionalRegistrationNumber(A<int>._, A<string?>._, A<bool>._)
            ).DoesNothing();

            A.CallTo(
                () => groupsService.UpdateSynchronisedDelegateGroupsBasedOnUserChanges(
                    A<int>._,
                    A<AccountDetailsData>._,
                    A<RegistrationFieldAnswers>._,
                    A<RegistrationFieldAnswers>._,
                    A<string?>._
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
                string emailAddress,
                string? hasPrn,
                string? prn
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
                EmailAddress = emailAddress;
                HasPRN = hasPrn;
                PRN = prn;
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
            public string EmailAddress { get; }
            public string? HasPRN { get; }
            public string? PRN { get; }
        }
    }
}
