namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class ProgressDataServiceTests
    {
        private SqlConnection connection = null!;
        private ProgressDataService progressDataService = null!;
        private TutorialContentTestHelper tutorialContentTestHelper = null!;

        [SetUp]
        public void SetUp()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            progressDataService = new ProgressDataService(connection);
            tutorialContentTestHelper = new TutorialContentTestHelper(connection);
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
        public void InsertNewAspProgressForTutorialIfNoneExist_inserts_new_record()
        {
            // Given
            const int tutorialId = 12732;
            const int customisationId = 24286;

            using var transaction = new TransactionScope();
            try
            {
                // When
                var initialAspProgressIds = tutorialContentTestHelper.GetAspProgressFromTutorialId(tutorialId).ToList();
                progressDataService.InsertNewAspProgressForTutorialIfNoneExist(tutorialId, customisationId);
                var resultAspProgressIds = tutorialContentTestHelper.GetAspProgressFromTutorialId(tutorialId).ToList();

                // Then
                using (new AssertionScope())
                {
                    initialAspProgressIds.Count.Should().Be(3);
                    resultAspProgressIds.Count.Should().Be(12);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}
