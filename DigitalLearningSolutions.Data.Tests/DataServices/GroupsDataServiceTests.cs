namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Mappers;
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

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MapperHelper.SetUpFluentMapper();
        }

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            groupsDataService = new GroupsDataService(connection);
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
        public async Task RemoveRelatedProgressRecordsForGroup_updates_progress_record()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                var removedDate = DateTime.UtcNow;
                const int groupId = 60;
                const int delegateId = 298304;
                const bool removeStartedEnrolments = true;
                const int progressId = 282560;

                // When
                groupsDataService.RemoveRelatedProgressRecordsForGroup(groupId, delegateId, removeStartedEnrolments, removedDate);
                var progressFields = await connection.GetProgressRemovedFields(progressId);

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
        public async Task RemoveRelatedProgressRecordsForGroup_does_not_remove_started_progress_record_when_specified()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                var removedDate = DateTime.UtcNow;
                const int groupId = 60;
                const int delegateId = 298304;
                const bool removeStartedEnrolments = false;
                const int progressId = 282560;

                // When
                groupsDataService.RemoveRelatedProgressRecordsForGroup(groupId, delegateId, removeStartedEnrolments, removedDate);
                var progressFields = await connection.GetProgressRemovedFields(progressId);

                // Then
                progressFields.Item1.Should().Be(0);
                progressFields.Item2.Should().BeNull();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public async Task
            RemoveRelatedProgressRecordsForGroup_does_not_update_progress_record_when_course_is_shared_by_another_group()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                var removedDate = DateTime.UtcNow;
                const int delegateId = 299228;
                const bool removeStartedEnrolments = false;
                AddDelegateToGroupWithSharedCourse();
                AddProgressRecordForGroupWithSharedCourse();

                // When
                groupsDataService.RemoveRelatedProgressRecordsForGroup(8, delegateId, removeStartedEnrolments, removedDate);
                var progressFields = await connection.GetProgressRemovedFields(285172);

                // Then
                progressFields.Item1.Should().Be(0);
                progressFields.Item2.Should().BeNull();
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
        public void AddDelegateGroup_sets_all_fields_correctly()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given

                var groupDetails = new GroupDetails
                {
                    CentreId = 101,
                    GroupLabel = "Group name",
                    GroupDescription = "Group description",
                    AdminUserId = 1,
                    CreatedDate = DateTime.UtcNow,
                    LinkedToField = 0,
                    SyncFieldChanges = false,
                    AddNewRegistrants = false,
                    PopulateExisting = false
                };

                // When
                var groupId = groupsDataService.AddDelegateGroup(groupDetails);

                // Then
                var group = groupsDataService.GetGroupsForCentre(groupDetails.CentreId)
                    .First(g => g.GroupId == groupId);
                group.GroupLabel.Should().Be(groupDetails.GroupLabel);
                group.GroupDescription.Should().Be(groupDetails.GroupDescription);
                group.AddedByAdminId.Should().Be(groupDetails.AdminUserId);
                group.LinkedToField.Should().Be(groupDetails.LinkedToField);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void AddDelegateToGroup_adds_the_delegate_to_the_expected_group()
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
        public async Task RemoveRelatedProgressRecordsForGroup_deletes_records_correctly_when_deleteStartedEnrolment_is_false()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                const int groupId = 8;
                const bool deleteStartedEnrolment = false;
                var removedDate = DateTime.UtcNow;

                // When
                groupsDataService.RemoveRelatedProgressRecordsForGroup(groupId, deleteStartedEnrolment, removedDate);

                // Then
                var notStartedProgress = await connection.GetProgressRemovedFields(271518);
                notStartedProgress.Item1.Should().Be(3);
                notStartedProgress.Item2.Should().BeCloseTo(removedDate);

                var startedProgress = await connection.GetProgressRemovedFields(285128);
                startedProgress.Item1.Should().Be(0);
                startedProgress.Item2.Should().BeNull();

                var progressWithAnotherGroup = await connection.GetProgressRemovedFields(285122);
                progressWithAnotherGroup.Item1.Should().Be(0);
                progressWithAnotherGroup.Item2.Should().BeNull();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public async Task RemoveRelatedProgressRecordsForGroup_deletes_records_correctly_when_deleteStartedEnrolment_is_true()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                const int groupId = 8;
                const bool deleteStartedEnrolment = true;
                var removedDate = DateTime.UtcNow;

                // When
                groupsDataService.RemoveRelatedProgressRecordsForGroup(groupId, deleteStartedEnrolment, removedDate);

                // Then
                var notStartedProgress = await connection.GetProgressRemovedFields(271518);
                notStartedProgress.Item1.Should().Be(3);
                notStartedProgress.Item2.Should().BeCloseTo(removedDate);

                var startedProgress = await connection.GetProgressRemovedFields(285128);
                startedProgress.Item1.Should().Be(3);
                startedProgress.Item2.Should().BeCloseTo(removedDate);

                var progressWithAnotherGroup = await connection.GetProgressRemovedFields(285122);
                progressWithAnotherGroup.Item1.Should().Be(0);
                progressWithAnotherGroup.Item2.Should().BeNull();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public async Task DeleteGroupDelegates_deletes_all_group_delegates()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                const int groupId = 8;

                // When
                groupsDataService.DeleteGroupDelegates(groupId);

                // Then
                var groupDelegates = await connection.GetCandidatesForGroup(groupId);
                groupDelegates.Should().BeEmpty();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public async Task DeleteGroupCustomisations_deletes_all_group_customisations()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                const int groupId = 8;

                // When
                groupsDataService.DeleteGroupCustomisations(groupId);

                // Then
                var groupCustomisations = await connection.GetCustomisationsForGroup(groupId);
                groupCustomisations.Should().BeEmpty();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void DeleteGroup_deletes_group()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const int groupId = 25;
                const int centreId = 101;

                // When
                groupsDataService.DeleteGroup(groupId);

                // Then
                var groupName = groupsDataService.GetGroupName(groupId, centreId);
                groupName.Should().BeNull();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetGroupCourse_returns_expected_course()
        {
            // Given
            var expectedDateTime = new DateTime(2019, 11, 15, 13, 53, 26, 510);
            var expectedGroupCourse = GroupTestHelper.GetDefaultGroupCourse(
                groupCustomisationId: 25,
                groupId: 103,
                supervisorAdminId: 1,
                completeWithinMonths: 0,
                supervisorFirstName: "Kevin",
                supervisorLastName: "Whittaker (Developer)",
                addedToGroup: expectedDateTime);

            // When
            var result = groupsDataService.GetGroupCourse(25, 103, 101);

            // Then
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(expectedGroupCourse);
            }
        }

        [Test]
        public async Task DeleteGroupCustomisation_deletes_expected_group_customisation()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                var groupId = 103;
                var customisationId = 14675;
                AddDelegateGroupCustomisationIfNotExist(groupId, customisationId);
                var groupCustomisationId = (await connection.GetCustomisationsForGroup(groupId))
                    .OrderByDescending(o => o)
                    .First();

                // When
                groupsDataService.DeleteGroupCustomisation(groupCustomisationId);

                // Then
                var groupCustomisations = await connection.GetCustomisationsForGroup(groupId);
                groupCustomisations.Should().NotContain(groupCustomisationId);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [TestCase("self_enrolled", 5, 15937, 299238, 1, 0, false, 0, false)]
        [TestCase("course_started", 5, 25918, 299238, 3, 5, false, 0, false)]
        [TestCase("course_completed", 5, 25918, 299238, 3, 5, true, 80, true)]
        [TestCase("non_group_candidate", 5, 25918, 298272, 3, 0, false, 0, false)]
        public async Task RemoveRelatedProgressRecordsForCourse_does_not_update_for_case(string testCase, int groupId, int customisationId, int candidateId,
            int enrollmentMethod, int loginCount, bool deleteStartedEnrolment, int progressDuration, bool isCompleted)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                var progressStarted = new DateTime(2021, 11, 5, 22, 23, 24, 567);
                var progressRemoved = new DateTime(2021, 11, 6, 22, 23, 24, 567);
                var progressCompleted = isCompleted ? new DateTime(2021, 11, 7, 22, 23, 24, 567) : (DateTime?)null;

                AddDelegateGroupCustomisationIfNotExist(groupId, customisationId);
                AddProgressIfNotExistOrIsComplete(candidateId, customisationId, progressStarted, enrollmentMethod, loginCount, progressDuration, progressCompleted);

                var groupCustomisationIds = await connection.GetCustomisationsForGroupAndCustomisation(groupId, customisationId);

                // When
                groupsDataService.RemoveRelatedProgressRecordsForCourse(groupId, groupCustomisationIds.First(), deleteStartedEnrolment, progressRemoved);

                // Then
                var progressRecords = await connection.GetProgressRemovalsByCourse(customisationId);

                var addedProgress = progressRecords.Where(p => p.submittedTime == progressStarted);
                var existingProgress = progressRecords.Where(p => p.submittedTime != progressStarted);
                addedProgress.Should().OnlyContain(p => p.removalMethodId == 0 && p.removedDate == null);
                existingProgress.Should().OnlyContain(p => p.removedDate == null || p.removedDate != progressRemoved);

                DeleteProgressById(addedProgress.First().progressId);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [TestCase("course_not_started", 5, 25918, 299238, 3, 0, false)]
        [TestCase("course_started_force_update", 5, 25918, 299238, 3, 5, true)]
        public async Task RemoveRelatedProgressRecordsForCourse_does_update_for_case(string testCase, int groupId, int customisationId, int candidateId,
            int enrollmentMethod, int loginCount, bool deleteStartedEnrolment)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                var progressStarted = new DateTime(2021, 11, 5, 22, 23, 24, 567);
                var progressRemoved = new DateTime(2021, 11, 6, 22, 23, 24, 567);

                AddDelegateGroupCustomisationIfNotExist(groupId, customisationId);
                AddProgressIfNotExistOrIsComplete(candidateId, customisationId, progressStarted, enrollmentMethod, loginCount);

                var groupCustomisationIds = await connection.GetCustomisationsForGroupAndCustomisation(groupId, customisationId);

                // When
                groupsDataService.RemoveRelatedProgressRecordsForCourse(groupId, groupCustomisationIds.First(), deleteStartedEnrolment, progressRemoved);

                // Then
                var progressRecords = await connection.GetProgressRemovalsByCourse(customisationId);

                var addedProgress = progressRecords.Where(p => p.submittedTime == progressStarted);
                var existingProgress = progressRecords.Where(p => p.submittedTime != progressStarted);
                addedProgress.Should().OnlyContain(p => p.removalMethodId == 3 && p.removedDate == progressRemoved);
                existingProgress.Should().OnlyContain(p => p.removedDate == null || p.removedDate != progressRemoved);

                DeleteProgressById(addedProgress.First().progressId);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public async Task RemoveRelatedProgressRecordsForCourse_does_not_update_when_shared_course()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                var groupId = 5;
                var alternativeGroupId = 6;
                var customisationId = 6644;
                var candidateId = 299238;
                var progressStarted = new DateTime(2021, 11, 5, 22, 23, 24, 567);
                var progressRemoved = new DateTime(2021, 11, 6, 22, 23, 24, 567);
                var enrollmentMethod = 3;

                var loginCount = 0;
                var deleteStartedEnrolment = false;

                AddDelegateGroupCustomisationIfNotExist(groupId, customisationId);
                AddDelegateGroupCustomisationIfNotExist(alternativeGroupId, customisationId);
                AddDelegateToGroupIfNotExist(groupId, candidateId, progressStarted);
                AddDelegateToGroupIfNotExist(alternativeGroupId, candidateId, progressStarted);
                AddProgressIfNotExistOrIsComplete(candidateId, customisationId, progressStarted, enrollmentMethod, loginCount);

                var groupCustomisationIds = await connection.GetCustomisationsForGroupAndCustomisation(groupId, customisationId);

                // When
                groupsDataService.RemoveRelatedProgressRecordsForCourse(groupId, groupCustomisationIds.First(), deleteStartedEnrolment, progressRemoved);

                // Then
                var progressRecords = await connection.GetProgressRemovalsByCourse(customisationId);

                var addedProgress = progressRecords.Where(p => p.submittedTime == progressStarted);
                var existingProgress = progressRecords.Where(p => p.submittedTime != progressStarted);
                addedProgress.Should().OnlyContain(p => p.removalMethodId == 0 && p.removedDate == null);
                existingProgress.Should().OnlyContain(p => p.removedDate == null || p.removedDate != progressRemoved);

                DeleteProgressById(addedProgress.First().progressId);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        private void AddDelegateToGroupWithSharedCourse()
        {
            connection.Execute(
                @"INSERT INTO dbo.GroupDelegates (GroupID, DelegateID, AddedDate, AddedByFieldLink)
                    VALUES (8, 299228, GETUTCDATE(), 1)"
            );
        }

        private void AddProgressRecordForGroupWithSharedCourse()
        {
            connection.Execute(
                @"SET IDENTITY_INSERT dbo.Progress ON
                    INSERT INTO Progress(ProgressID, CandidateID, CustomisationID, CustomisationVersion, SubmittedTime, EnrollmentMethodID, SupervisorAdminID)
                    VALUES (285172,299228,25918,1,GETUTCDATE(),3,0)
                    SET IDENTITY_INSERT dbo.Progress OFF"
            );
        }

        private void AddDelegateGroupCustomisationIfNotExist(int groupId, int customisationId)
        {
            connection.Execute(
                @"INSERT INTO dbo.GroupCustomisations (GroupID, CustomisationID, AddedByAdminUserID, SupervisorAdminID) 
                    SELECT @groupId, @customisationId, 1, 1 
                        WHERE NOT EXISTS ( SELECT GroupCustomisationID FROM dbo.GroupCustomisations WHERE GroupID=@groupId AND CustomisationID=@customisationId)",
                new { groupId, customisationId }
            );
        }
        private void AddDelegateToGroupIfNotExist(int groupId, int delegateId, DateTime dateAdded)
        {
            connection.Execute(
                @"INSERT INTO dbo.GroupDelegates (GroupID, DelegateID, AddedDate, AddedByFieldLink) 
                    SELECT @groupId, @delegateId, @dateAdded, 1 
                        WHERE NOT EXISTS ( SELECT DelegateID FROM dbo.GroupDelegates WHERE GroupID=@groupId AND DelegateID=@delegateId)",
                new { groupId, delegateId, dateAdded }
            );
        }

        private void AddProgressIfNotExistOrIsComplete(int candidateId, int customisationId, DateTime submittedTime, int enrollmentMethodId = 3,
            int loginCount = 0, int duration = 0, DateTime? completed = null)
        {
            connection.Execute(
                @"INSERT INTO dbo.Progress
                    ([CandidateID], [CustomisationID], [CustomisationVersion], [SubmittedTime], [EnrollmentMethodID], [EnrolledByAdminID], [LoginCount], [Duration], [Completed])
                        SELECT @candidateId, @customisationId, 2, @submittedTime, @enrollmentMethodId, 1, @loginCount, @duration, @completed
                            WHERE NOT EXISTS ( SELECT ProgressID FROM dbo.Progress
                                WHERE CandidateID=@candidateId AND CustomisationID=@customisationId AND Completed IS NULL AND RemovedDate IS NULL)",
                new { candidateId, customisationId, submittedTime, enrollmentMethodId, loginCount, duration, completed }
            );
        }

        public void DeleteProgressById(int progressId)
        {
            connection.Execute(
                @"DELETE FROM Progress
                     WHERE ProgressID = @progressId",
                new { progressId }
            );
        }

    }
}
