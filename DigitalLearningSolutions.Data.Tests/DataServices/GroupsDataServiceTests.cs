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
        public void UpdateGroupDescription_updates_record()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const int centerId = 101;
                const int groupId = 5;
                const string newDescription = "Test group description1";

                // When
                groupsDataService.UpdateGroupDescription(
                    groupId,
                    centerId,
                    newDescription);

                // Then
                var result = GetGroupDescriptionById(groupId);
                result.Should().Be(newDescription);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void UpdateGroupDescription_with_incorrect_centreId_does_not_update_record()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                const int incorrectCentreId = 107;
                const int groupId = 5;
                const string newDescription = "Test group description1";

                // When
                groupsDataService.UpdateGroupDescription(
                    groupId,
                    incorrectCentreId,
                    newDescription);

                //Then
                var result = GetGroupDescriptionById(groupId);
                result?.Should().BeNull();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetGroupAtCentreById_returns_expected_group()
        {
            // Given
            var expectedGroup = GroupTestHelper.GetDefaultGroup();

            // When
            var result = groupsDataService.GetGroupAtCentreById(34, 101);

            //Then
            result.Should().BeEquivalentTo(expectedGroup);
        }


        [Test]
        public void GetGroupAtCentreById_returns_null_with_incorrect_centreId()
        {
            // Given
            const int groupId = 5;
            const int incorrectCentreId = 1;

            // When
            var result = groupsDataService.GetGroupAtCentreById(groupId, incorrectCentreId);

            // Then
            result.Should().BeNull();
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

        private string? GetGroupDescriptionById(int groupId)
        {
            return connection.Query<string?>(
                @"SELECT GroupDescription FROM Groups 
                    WHERE GroupID = @groupId",
                new { groupId }
            ).FirstOrDefault();
        }
    }
}
