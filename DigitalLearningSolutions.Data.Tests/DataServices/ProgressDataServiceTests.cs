﻿namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class ProgressDataServiceTests
    {
        private SqlConnection connection = null!;
        private ProgressDataService progressDataService = null!;
        private ProgressTestHelper progressTestHelper = null!;
        private TutorialContentTestHelper tutorialContentTestHelper = null!;

        [SetUp]
        public void SetUp()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<ProgressDataService>>();
            progressDataService = new ProgressDataService(connection, logger);
            tutorialContentTestHelper = new TutorialContentTestHelper(connection);
            progressTestHelper = new ProgressTestHelper(connection);
        }

        [Test]
        public void GetDelegateProgressForCourse_returns_expected_progress()
        {
            // Given
            const int delegateId = 1;
            const int customisationId = 100;

            // When
            var delegateProgress = progressDataService.GetDelegateProgressForCourse(delegateId, customisationId)
                .FirstOrDefault();

            // Then
            using (new AssertionScope())
            {
                delegateProgress.Should().NotBeNull();
                delegateProgress?.CandidateId.Should().Be(delegateId);
                delegateProgress?.CustomisationId.Should().Be(customisationId);
                delegateProgress?.ProgressId.Should().Be(1);
                delegateProgress?.Completed.Should().BeNull();
                delegateProgress?.RemovedDate.Should().BeNull();
                delegateProgress?.SupervisorAdminId.Should().Be(0);
            }
        }

        [Test]
        public void UpdateProgressSupervisorAndCompleteByDate_updates_those_columns()
        {
            // Given
            const int progressId = 1;
            const int candidateIdForProgressRecord = 1;
            const int customisationIdForProgressRecord = 100;
            const int supervisorAdminId = 5;
            var completeByDate = new DateTime(2021, 12, 25);

            using var transaction = new TransactionScope();
            try
            {
                // When
                progressDataService.UpdateProgressSupervisorAndCompleteByDate(
                    progressId,
                    supervisorAdminId,
                    completeByDate
                );
                var progressRecords = progressDataService.GetDelegateProgressForCourse(
                    candidateIdForProgressRecord,
                    customisationIdForProgressRecord
                );

                // Then
                var updatedProgressRecord = progressRecords.First(p => p.ProgressId == progressId);
                updatedProgressRecord.SupervisorAdminId.Should().Be(supervisorAdminId);
                updatedProgressRecord.CompleteByDate.Should().Be(completeByDate);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void CreateNewDelegateProgress_adds_new_record()
        {
            // Given
            const int delegateId = 1;
            const int customisationId = 100;
            const int currentVersion = 2;
            var submittedTime = new DateTime(2021, 3, 3);
            const int enrollmentMethodId = 3;
            const int enrolledByAdminId = 5;
            const int supervisorAdminId = 7;
            var completeByDate = new DateTime(2020, 1, 1);

            using var transaction = new TransactionScope();
            try
            {
                // When
                var newProgressId = progressDataService.CreateNewDelegateProgress(
                    delegateId,
                    customisationId,
                    currentVersion,
                    submittedTime,
                    enrollmentMethodId,
                    enrolledByAdminId,
                    completeByDate,
                    supervisorAdminId
                );
                var progressRecords = progressDataService.GetDelegateProgressForCourse(
                    delegateId,
                    customisationId
                );

                // Then
                var newProgressRecord = progressRecords.First(p => p.ProgressId == newProgressId);
                using (new AssertionScope())
                {
                    newProgressRecord.CandidateId.Should().Be(delegateId);
                    newProgressRecord.CustomisationId.Should().Be(customisationId);
                    newProgressRecord.CustomisationVersion.Should().Be(currentVersion);
                    newProgressRecord.SubmittedTime.Should().Be(submittedTime);
                    newProgressRecord.EnrollmentMethodId.Should().Be(enrollmentMethodId);
                    newProgressRecord.EnrolledByAdminId.Should().Be(enrolledByAdminId);
                    newProgressRecord.SupervisorAdminId.Should().Be(supervisorAdminId);
                    newProgressRecord.CompleteByDate.Should().Be(completeByDate);
                    newProgressRecord.Completed.Should().BeNull();
                    newProgressRecord.RemovedDate.Should().BeNull();
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void CreateNewAspProgressRecord_adds_new_record()
        {
            // Given
            const int progressId = 1;
            const int tutorialId = 2;

            using var transaction = new TransactionScope();
            try
            {
                // When
                progressDataService.CreateNewAspProgress(tutorialId, progressId);
                var createdAspProgressId =
                    tutorialContentTestHelper.GetAspProgressFromTutorialAndProgressId(tutorialId, progressId);

                // Then
                createdAspProgressId.Should().NotBeNull();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void InsertNewAspProgressRecordsForTutorialIfNoneExist_inserts_new_records()
        {
            // Given
            const int tutorialId = 12732;
            const int customisationId = 14019;

            using var transaction = new TransactionScope();
            try
            {
                // When
                var initialProgressIdsOnAspProgressRecords = tutorialContentTestHelper
                    .GetDistinctProgressIdsOnAspProgressRecordsFromTutorialId(tutorialId).ToList();
                progressDataService.InsertNewAspProgressRecordsForTutorialIfNoneExist(tutorialId, customisationId);
                var resultProgressIdsOnAspProgressRecords = tutorialContentTestHelper
                    .GetDistinctProgressIdsOnAspProgressRecordsFromTutorialId(tutorialId).ToList();

                // Then
                using (new AssertionScope())
                {
                    initialProgressIdsOnAspProgressRecords.Count.Should().Be(3);
                    resultProgressIdsOnAspProgressRecords.Count.Should().Be(6);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void
            InsertNewAspProgressRecordsForTutorialIfNoneExist_does_not_insert_new_records_when_they_already_exist()
        {
            // Given
            const int tutorialId = 12925;
            const int customisationId = 27816;

            using var transaction = new TransactionScope();
            try
            {
                // When
                var initialAspProgressIds = tutorialContentTestHelper
                    .GetAspProgressFromTutorialId(tutorialId).ToList();
                progressDataService.InsertNewAspProgressRecordsForTutorialIfNoneExist(tutorialId, customisationId);
                var resultAspProgressIds = tutorialContentTestHelper
                    .GetAspProgressFromTutorialId(tutorialId).ToList();

                // Then
                using (new AssertionScope())
                {
                    initialAspProgressIds.Count.Should()
                        .Be(resultAspProgressIds.Count);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void ClearAspProgressVerificationRequest_updates_aspProgress_records()
        {
            // Given
            const int progressId = 285046;
            const int aspProgressId = 8509834;

            using var transaction = new TransactionScope();
            try
            {
                // When
                progressDataService.ClearAspProgressVerificationRequest(progressId);
                var result = progressTestHelper.GetSupervisorVerificationRequestedByAspProgressId(aspProgressId);

                // Then
                result.Should().BeNull();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void Set_completion_date_should_update_db()
        {
            // Given
            const int progressId = 1;
            const int candidateIdForProgressRecord = 1;
            const int customisationIdForProgressRecord = 100;
            var completionDate = new DateTime(2020, 12, 25);

            using var transaction = new TransactionScope();

            // When
            progressDataService.SetCompletionDate(
                progressId,
                completionDate
            );
            var progressRecords = progressDataService.GetDelegateProgressForCourse(
                candidateIdForProgressRecord,
                customisationIdForProgressRecord
            );

            // Then
            var updatedProgressRecord = progressRecords.First(p => p.ProgressId == progressId);
            updatedProgressRecord.Completed.Should().Be(completionDate);
        }

        [Test]
        [TestCase(0, 7, 0)]
        [TestCase(3, 7, 3)]
        [TestCase(4, 7, 4)]
        [TestCase(5, 7, 4)]
        [TestCase(7, 7, 4)]
        [TestCase(10, 10, 4)]
        public void UpdateDiagnosticScore_should_update_db(int myScore, int diagHigh, int diagLow)
        {
            // Given
            const int aspProgressId = 159709;
            const int progressId = 40771;
            const int tutorialId = 321;

            using var transaction = new TransactionScope();

            // When
            progressDataService.UpdateDiagnosticScore(
                progressId,
                tutorialId,
                myScore
            );
            var diagnosticInfo = progressTestHelper.GetDiagnosticInfoByAspProgressId(aspProgressId);

            // Then
            diagnosticInfo.DiagHigh.Should().Be(diagHigh);
            diagnosticInfo.DiagLow.Should().Be(diagLow);
            diagnosticInfo.DiagLast.Should().Be(myScore);
            diagnosticInfo.DiagAttempts.Should().Be(5);
        }

        [Test]
        public void UpdateDiagnosticScore_should_update_DiagLow_when_DiagAttempts_is_zero()
        {
            // Given
            const int aspProgressId = 100;
            const int progressId = 15913;
            const int tutorialId = 90;
            const int myScore = 3;

            using var transaction = new TransactionScope();

            // When
            progressDataService.UpdateDiagnosticScore(
                progressId,
                tutorialId,
                myScore
            );
            var diagnosticInfo = progressTestHelper.GetDiagnosticInfoByAspProgressId(aspProgressId);

            // Then
            diagnosticInfo.DiagHigh.Should().Be(myScore);
            diagnosticInfo.DiagLow.Should().Be(myScore);
            diagnosticInfo.DiagLast.Should().Be(myScore);
            diagnosticInfo.DiagAttempts.Should().Be(1);
        }

        [Test]
        public void UnlockCourseProgress_updates_progress_record()
        {
            // Given
            const int progressId = 280244;
            var statusBeforeUnlock = progressTestHelper.GetCourseProgressLockedStatusByProgressId(progressId);

            using var transaction = new TransactionScope();
            try
            {
                // When
                progressDataService.UnlockProgress(progressId);
                var statusAfterUnlocked = progressTestHelper.GetCourseProgressLockedStatusByProgressId(progressId);

                // Then
                statusBeforeUnlock.Should().BeTrue();
                statusAfterUnlocked.Should().BeFalse();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void LockProgress_updates_progress_record()
        {
            using var transaction = new TransactionScope();

            // Given
            const int progressId = 1;
            var statusBeforeLock = progressTestHelper.GetCourseProgressLockedStatusByProgressId(progressId);

            // When
            progressDataService.LockProgress(progressId);
            var statusAfterLocked = progressTestHelper.GetCourseProgressLockedStatusByProgressId(progressId);

            // Then
            statusBeforeLock.Should().BeFalse();
            statusAfterLocked.Should().BeTrue();
        }

        [Test]
        public void GetLearningLogEntries_gets_records_correctly()
        {
            // Given
            const int progressId = 1;
            var expectedRecordFromSessionsTable = new LearningLogEntry
            {
                When = new DateTime(2010, 09, 22, 06, 52, 09, 540),
                LearningTime = 51,
                AssessmentScore = null,
                AssessmentTaken = null,
                AssessmentStatus = null,
            };
            var expectedRecordFromAssessAttemptsTable = new LearningLogEntry
            {
                When = new DateTime(2010, 10, 13, 07, 00, 26, 640),
                LearningTime = null,
                AssessmentScore = 100,
                AssessmentTaken = "Using Windows",
                AssessmentStatus = true,
            };

            // When
            var result = progressDataService.GetLearningLogEntries(progressId).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(88);
                result[0].Should().BeEquivalentTo(expectedRecordFromSessionsTable);
                result[82].Should().BeEquivalentTo(expectedRecordFromAssessAttemptsTable);
            }
        }

        [Test]
        public void GetProgressByProgressId_returns_expected_progress()
        {
            // When
            var result = progressDataService.GetProgressByProgressId(1);

            // Then
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.CandidateId.Should().Be(1);
                result?.CustomisationId.Should().Be(100);
                result?.ProgressId.Should().Be(1);
                result?.Completed.Should().BeNull();
                result?.RemovedDate.Should().BeNull();
                result?.SupervisorAdminId.Should().Be(0);
            }
        }

        [Test]
        public void GetSectionProgressDataForProgressEntry_returns_expected_data()
        {
            // When
            var result = progressDataService.GetSectionProgressDataForProgressEntry(15885).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count().Should().Be(9);
                var first = result.First();

                first.SectionId.Should().Be(74);
                first.SectionName.Should().Be("Working with documents");
                first.Completion.Should().Be(75);
                first.TotalTime.Should().Be(5);
                first.AverageTime.Should().Be(28);

                first.PostLearningAssessPath.Should().Be(
                    "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Post_1.dcr"
                );
                first.IsAssessed.Should().BeTrue();
                first.Attempts.Should().Be(0);
                first.Outcome.Should().Be(0);
                first.Passed.Should().BeFalse();

                first.Tutorials.Should().BeNull();
            }
        }

        [Test]
        public void GetTutorialProgressDataForSection_returns_expected_data()
        {
            // When
            var result = progressDataService.GetTutorialProgressDataForSection(157704, 75).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count().Should().Be(7);
                var first = result.First();

                first.TutorialName.Should().Be("Format characters");
                first.TutorialStatus.Should().Be("Started");
                first.TimeTaken.Should().Be(0);
                first.AvgTime.Should().Be(7);
                first.DiagnosticScore.Should().Be(4);
                first.PossibleScore.Should().Be(4);
            }
        }

        [Test]
        public void GetTutorialProgressDataForSection_returns_expected_data_for_overridden_average_time()
        {
            // Given
            using var transaction = new TransactionScope();
            connection.Execute(
                @"UPDATE tutorials
                        SET OverrideTutorialMins = 1
                        WHERE TutorialID = 53"
            );

            // When
            var result = progressDataService.GetTutorialProgressDataForSection(157704, 75).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(7);
                var first = result.First();

                first.AvgTime.Should().Be(1);
            }
        }

        [Test]
        public void GetTutorialProgressDataForSection_returns_expected_data_for_zero_diagattempts()
        {
            // Given
            using var transaction = new TransactionScope();
            connection.Execute(
                @"UPDATE aspProgress
                        SET DiagAttempts = 0
                        WHERE aspProgressID = 3373869"
            );

            // When
            var result = progressDataService.GetTutorialProgressDataForSection(157704, 75).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(7);
                var first = result.First();

                first.DiagnosticScore.Should().Be(null);
            }
        }

        [Test]
        public void GetTutorialProgressDataForSection_returns_expected_data_for_diagstatus_false()
        {
            // Given
            using var transaction = new TransactionScope();
            connection.Execute(
                @"UPDATE CustomisationTutorials
                        SET DiagStatus = 0
                        WHERE CusTutID = 324886"
            );

            // When
            var result = progressDataService.GetTutorialProgressDataForSection(157704, 75).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(7);
                var first = result.First();

                first.DiagnosticScore.Should().Be(null);
            }
        }

        [Test]
        public void UpdateCourseAdminFieldForDelegate_updates_admin_field_answer_on_progress_record()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string answer = "Test answer";

                // When
                progressDataService.UpdateCourseAdminFieldForDelegate(100, 1, answer);
                var progressAnswer1 = progressTestHelper.GetAdminFieldAnswer1ByProgressId(100);

                // Then
                progressAnswer1.Should().Be(answer);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void UpdateProgressDetailsForStoreAspProgressV2_updates_correct_fields_on_progress_record()
        {
            using var transaction = new TransactionScope();

            // Given
            const int progressId = 261317;
            const int customisationVersion = 1;
            var submittedTime = new DateTime(2022, 1, 1, 1, 1, 1);
            const string progressText = "Test progress text";
            const int diagnosticScore = 100;
            var expectedProgressDetails = new ProgressDetails(
                customisationVersion,
                submittedTime,
                progressText,
                diagnosticScore
            );

            // When
            progressDataService.UpdateProgressDetailsForStoreAspProgressV2(
                progressId,
                customisationVersion,
                submittedTime,
                progressText
            );

            // Then
            var progressDetails = progressTestHelper.GetProgressDetailsByProgressId(progressId);
            progressDetails.Should().BeEquivalentTo(expectedProgressDetails);
        }

        [Test]
        public void
            UpdateProgressDetailsForStoreAspProgressV2_sets_diagnostic_score_to_zero_if_score_cannot_be_calculated()
        {
            using var transaction = new TransactionScope();

            // Given
            const int progressIdForRecordWithSumOfDiagAssessOutOfEqualingZero = 175824;
            const int customisationVersion = 1;
            var submittedTime = new DateTime(2022, 1, 1, 1, 1, 1);
            const string progressText = "Test progress text";
            const int expectedDiagnosticScore = 0;

            // When
            progressDataService.UpdateProgressDetailsForStoreAspProgressV2(
                progressIdForRecordWithSumOfDiagAssessOutOfEqualingZero,
                customisationVersion,
                submittedTime,
                progressText
            );

            // Then
            var progressDetails =
                progressTestHelper.GetProgressDetailsByProgressId(
                    progressIdForRecordWithSumOfDiagAssessOutOfEqualingZero
                );
            progressDetails.DiagnosticScore.Should().Be(expectedDiagnosticScore);
        }

        [Test]
        public void UpdateAspProgressTutTime_adds_new_tut_time_value_to_existing()
        {
            using var transaction = new TransactionScope();

            // Given
            const int inputTutTime = 1;
            const int expectedTutTime = 3;

            // When
            progressDataService.UpdateAspProgressTutTime(91, 15885, inputTutTime);

            // Then
            var progressTutTime = progressTestHelper.GetAspProgressTutTimeById(53);
            progressTutTime.Should().Be(expectedTutTime);
        }

        [Test]
        public void UpdateAspProgressTutStat_updates_tut_stat_with_new_value_if_greater_than_existing()
        {
            using var transaction = new TransactionScope();

            // Given
            const int expectedTutStat = 3;

            // When
            progressDataService.UpdateAspProgressTutStat(91, 15885, expectedTutStat);

            // Then
            var progressTutTime = progressTestHelper.GetAspProgressTutStatById(53);
            progressTutTime.Should().Be(expectedTutStat);
        }

        [Test]
        public void UpdateAspProgressTutStat_does_not_update_tut_stat_with_new_value_if_less_than_existing()
        {
            using var transaction = new TransactionScope();

            // Given
            const int inputTutStat = 1;
            const int expectedTutStat = 2;

            // When
            progressDataService.UpdateAspProgressTutStat(91, 15885, inputTutStat);

            // Then
            var progressTutTime = progressTestHelper.GetAspProgressTutStatById(53);
            progressTutTime.Should().Be(expectedTutStat);
        }

        [Test]
        public void UpdateProgressCompletedDate_updates_progress_record_correctly()
        {
            using var transaction = new TransactionScope();

            // Given
            const int progressId = 100;
            var expectedCompletedDate = new DateTime(2022, 1, 1, 1, 1, 1);

            // When
            progressDataService.SetCompletionDate(progressId, expectedCompletedDate);

            // Then
            var progressCompletedDate = progressTestHelper.GetProgressCompletedDateById(progressId);
            progressCompletedDate.Should().Be(expectedCompletedDate);
        }

        [Test]
        public void GetAssessAttemptsForProgressSection_gets_all_appropriate_records()
        {
            // Given
            const int progressId = 1;
            const int sectionNumber = 2;

            // When
            var results = progressDataService.GetAssessAttemptsForProgressSection(progressId, sectionNumber);

            // Then
            var expectedAssessAttemptResults = Builder<AssessAttempt>.CreateListOfSize(2).All()
                .With(a => a.CandidateId = 1)
                .With(a => a.CustomisationId = 100)
                .With(a => a.CustomisationVersion = 1)
                .With(a => a.AssessInstance = 3)
                .With(a => a.SectionNumber = 2)
                .With(a => a.Score = 100)
                .With(a => a.Status = true)
                .With(a => a.ProgressId = 1)
                .TheFirst(1).With(a => a.AssessAttemptId = 3)
                .And(a => a.Date = new DateTime(2010, 09, 22, 7, 54, 40, 307))
                .TheLast(1).With(a => a.AssessAttemptId = 4)
                .And(a => a.Date = new DateTime(2010, 09, 22, 7, 58, 04, 937))
                .Build();
            results.Should().BeEquivalentTo(expectedAssessAttemptResults);
        }

        [Test]
        public void InsertAssessAttempt_inserts_details_correctly()
        {
            using var transaction = new TransactionScope();

            // Given
            const int candidateId = 987;
            const int customisationId = 123;
            const int customisationVersion = 2;
            const int sectionNumber = 4;
            const int score = 42;
            const bool status = false;
            const int progressId = 1;
            var insertionDate = new DateTime(2022, 06, 14, 12, 23, 54, 937);

            // When
            var recordsPriorToInsertion = progressDataService.GetAssessAttemptsForProgressSection(
                progressId,
                sectionNumber
            );

            progressDataService.InsertAssessAttempt(
                candidateId,
                customisationId,
                customisationVersion,
                insertionDate,
                sectionNumber,
                score,
                status,
                progressId
            );
            var result = progressDataService.GetAssessAttemptsForProgressSection(
                progressId,
                sectionNumber
            ).ToList();

            // Then
            using (new AssertionScope())
            {
                recordsPriorToInsertion.Count().Should().Be(1);
                result.Count.Should().Be(2);
                var insertedRecord = result.OrderByDescending(aa => aa.AssessAttemptId).First();
                insertedRecord.CandidateId.Should().Be(candidateId);
                insertedRecord.CustomisationId.Should().Be(customisationId);
                insertedRecord.CustomisationVersion.Should().Be(customisationVersion);
                insertedRecord.Date.Should().Be(insertionDate);
                insertedRecord.AssessInstance.Should().Be(1);
                insertedRecord.SectionNumber.Should().Be(sectionNumber);
                insertedRecord.Score.Should().Be(score);
                insertedRecord.Status.Should().Be(status);
                insertedRecord.ProgressId.Should().Be(progressId);
            }
        }

        [Test]
        public void GetSectionAndApplicationDetailsForAssessAttempts_gets_all_appropriate_details()
        {
            // Given
            const int sectionId = 1609;
            const int customisationId = 21072;

            // When
            var result =
                progressDataService.GetSectionAndApplicationDetailsForAssessAttempts(sectionId, customisationId);

            // Then
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.AssessAttempts.Should().Be(2);
                result.PlaPassThreshold.Should().Be(50);
                result.SectionNumber.Should().Be(1);
            }
        }

        [Test]
        public void
            GetSectionAndApplicationDetailsForAssessAttempts_returns_null_if_section_and_customisation_do_not_match()
        {
            // Given
            const int sectionId = 11;
            const int customisationId = 12;

            // When
            var result =
                progressDataService.GetSectionAndApplicationDetailsForAssessAttempts(sectionId, customisationId);

            // Then
            result.Should().BeNull();
        }
    }
}
