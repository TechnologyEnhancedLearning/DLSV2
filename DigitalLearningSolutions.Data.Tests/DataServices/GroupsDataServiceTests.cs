namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class GroupsDataServiceTests
    {
        private SqlConnection connection = null!;
        private GroupsDataService groupsDataService = null!;
        private TutorialContentTestHelper tutorialContentTestHelper = null!;

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            groupsDataService = new GroupsDataService(connection);
            tutorialContentTestHelper = new TutorialContentTestHelper(connection);
        }

        [Test]
        public void GetGroupsForCentre_returns_expected_groups()
        {
            // Given
            var expectedFirstGroup = new Group
            {
                GroupId = 34,
                GroupLabel = "Social care - unspecified",
                GroupDescription = null,
                DelegateCount = 1,
                CoursesCount = 0,
                AddedByAdminId = 1,
                AddedByFirstName = "Kevin",
                AddedByLastName = "Whittaker (Developer)",
                LinkedToField = 4,
                LinkedToFieldName = "Job group",
                ShouldAddNewRegistrantsToGroup = true,
                ChangesToRegistrationDetailsShouldChangeGroupMembership = true
            };

            // When
            var result = groupsDataService.GetGroupsForCentre(101).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(41);
                result.First(x => x.GroupId == 34).Should().BeEquivalentTo(expectedFirstGroup);
            }
        }

        [Test]
        public void GetGroupDelegates_returns_expected_delegates()
        {
            // Given
            var expectedFirstGroupDelegate = GroupTestHelper.GetDefaultGroupDelegate();

            // When
            var result = groupsDataService.GetGroupDelegates(5).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(24);
                result.First(x => x.GroupDelegateId == expectedFirstGroupDelegate.GroupDelegateId).Should()
                    .BeEquivalentTo(expectedFirstGroupDelegate);
            }
        }

        [Test]
        public void GetGroupCourses_returns_expected_courses()
        {
            // Given
            var expectedDateTime = new DateTime(2018, 11, 02, 10, 53, 38, 920);
            var expectedFirstGroupCourse = GroupTestHelper.GetDefaultGroupCourse(addedToGroup: expectedDateTime);

            // When
            var result = groupsDataService.GetGroupCourses(8, 101).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(2);
                result.First(x => x.GroupCustomisationId == 1).Should().BeEquivalentTo(expectedFirstGroupCourse);
            }
        }

        [Test]
        public void GetGroupName_returns_expected_name_with_correct_centre()
        {
            // When
            var result = groupsDataService.GetGroupName(5, 101);

            // Then
            result.Should().BeEquivalentTo("Activities worker or coordinator");
        }

        [Test]
        public void GetGroupName_returns_null_with_incorrect_centre()
        {
            // When
            var result = groupsDataService.GetGroupName(5, 1);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void DeleteGroupDelegatesRecordForDelegate_deletes_record()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const int delegateId = 245969;

                // When
                groupsDataService.DeleteGroupDelegatesRecordForDelegate(5, delegateId);
                var delegates = groupsDataService.GetGroupDelegates(1);

                // Then
                delegates.Any(d => d.DelegateId == delegateId).Should().BeFalse();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public async Task RemoveRelatedProgressRecordsForGroupDelegate_updates_progress_record()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                var removedDate = DateTime.Now;
                const int delegateId = 245969;

                // When
                groupsDataService.RemoveRelatedProgressRecordsForGroupDelegate(5, delegateId, removedDate);
                var progressFields = await connection.GetProgressRemovedFields(285146);

                // Then
                progressFields.Item1.Should().Be(3);
                progressFields.Item2.Should().BeCloseTo(removedDate);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetRelatedProgressIdForGroupDelegate_returns_expected_value()
        {
            // Given
            const int delegateId = 245969;

            // When
            var result = groupsDataService.GetRelatedProgressIdForGroupDelegate(5, delegateId);

            // Then
            result.Should().Be(285146);
        }

        [Test]
        public void GetRelatedProgressIdForGroupDelegate_returns_null_with_no_record()
        {
            // Given
            const int delegateId = 2;

            // When
            var result = groupsDataService.GetRelatedProgressIdForGroupDelegate(1, delegateId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void AddedDelegateToGroup_adds_the_delegate_to_the_expected_group()
        {
            // Given
            const int delegateId = 10;
            const int groupId = 5;
            const int addedByFieldLink = 1;
            var addedDate = new DateTime(2021, 12, 25);

            using var transaction = new TransactionScope();
            try
            {
                // When
                groupsDataService.AddDelegateToGroup(delegateId, groupId, addedDate, addedByFieldLink);
                var groupDelegates = groupsDataService.GetGroupDelegates(groupId).ToList();

                // Then
                groupDelegates.Count.Should().Be(25);
                groupDelegates.Any(gd => gd.DelegateId == delegateId).Should().BeTrue();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetDelegateProgressForCourse_returns_expected_progress()
        {
            // Given
            const int delegateId = 1;
            const int customisationId = 100;

            // When
            var delegateProgress = groupsDataService.GetDelegateProgressForCourse(delegateId, customisationId)
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
                delegateProgress?.SystemRefreshed.Should().BeFalse();
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
                groupsDataService.UpdateProgressSupervisorAndCompleteByDate(
                    progressId,
                    supervisorAdminId,
                    completeByDate
                );
                var progressRecords = groupsDataService.GetDelegateProgressForCourse(
                    candidateIdForProgressRecord,
                    customisationIdForProgressRecord
                );
                var updatedProgressRecord = progressRecords.First(p => p.ProgressId == progressId);

                // Then
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
                var newProgressId = groupsDataService.CreateNewDelegateProgress(
                    delegateId,
                    customisationId,
                    currentVersion,
                    submittedTime,
                    enrollmentMethodId,
                    enrolledByAdminId,
                    completeByDate,
                    supervisorAdminId
                );
                var progressRecords = groupsDataService.GetDelegateProgressForCourse(
                    delegateId,
                    customisationId
                );
                var newProgressRecord = progressRecords.First(p => p.ProgressId == newProgressId);

                // Then
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
                    newProgressRecord.SystemRefreshed.Should().BeFalse();
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
                groupsDataService.CreateNewAspProgress(tutorialId, progressId);
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
    }
}
