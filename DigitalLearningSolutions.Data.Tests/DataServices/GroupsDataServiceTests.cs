namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Collections.Generic;
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
                ChangesToRegistrationDetailsShouldChangeGroupMembership = true,
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
        public void GetGroupCoursesVisibleToCentre_returns_expected_courses()
        {
            // Given
            var expectedGroupCourseIds = new List<int>
            {
                1,
                2,
                25,
                26,
                27,
                28,
            };

            // When
            var result = groupsDataService.GetGroupCoursesVisibleToCentre(101).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().HaveCount(6);
                result.Should().OnlyHaveUniqueItems();
                result.Should().OnlyContain(c => expectedGroupCourseIds.Contains(c.GroupCustomisationId));
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
                groupsDataService.RemoveRelatedProgressRecordsForGroup(
                    groupId,
                    delegateId,
                    removeStartedEnrolments,
                    removedDate
                );
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
                groupsDataService.RemoveRelatedProgressRecordsForGroup(
                    groupId,
                    delegateId,
                    removeStartedEnrolments,
                    removedDate
                );
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
                groupsDataService.RemoveRelatedProgressRecordsForGroup(
                    8,
                    delegateId,
                    removeStartedEnrolments,
                    removedDate
                );
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
                    PopulateExisting = false,
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
        public async Task
            RemoveRelatedProgressRecordsForGroup_deletes_records_correctly_when_deleteStartedEnrolment_is_false()
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
        public async Task
            RemoveRelatedProgressRecordsForGroup_deletes_records_correctly_when_deleteStartedEnrolment_is_true()
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
        public async Task
            RemoveRelatedProgressRecordsForGroup_does_not_remove_progress_for_delegates_outside_specified_group()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                const int groupId = 5;
                const bool deleteStartedEnrolment = true;
                var removedDate = DateTime.UtcNow;

                // When
                groupsDataService.RemoveRelatedProgressRecordsForGroup(groupId, deleteStartedEnrolment, removedDate);

                // Then
                var progressWithNoGroup = await connection.GetProgressRemovedFields(284968);
                progressWithNoGroup.Item1.Should().Be(0);
                progressWithNoGroup.Item2.Should().BeNull();
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
        public void GetGroupCourseIfVisibleToCentre_returns_expected_course()
        {
            // Given
            var expectedDateTime = new DateTime(2019, 11, 15, 13, 53, 26, 510);
            var expectedGroupCourse = GroupTestHelper.GetDefaultGroupCourse(
                25,
                103,
                supervisorAdminId: 1,
                completeWithinMonths: 0,
                supervisorFirstName: "Kevin",
                supervisorLastName: "Whittaker (Developer)",
                addedToGroup: expectedDateTime
            );

            // When
            var result = groupsDataService.GetGroupCourseIfVisibleToCentre(25, 101);

            // Then
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(expectedGroupCourse);
            }
        }

        [Test]
        public async Task DeleteGroupCustomisations_deletes_all_group_customisations()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            const int groupId = 8;

            // When
            groupsDataService.DeleteGroupCustomisations(groupId);

            // Then
            var groupCustomisations = await connection.GetGroupCustomisationIdsForGroup(groupId);
            groupCustomisations.Should().BeEmpty();

            transaction.Dispose();
        }

        [Test]
        public async Task DeleteGroupCustomisation_deletes_expected_group_customisation()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            const int groupCustomisationId = 25;
            const int groupId = 101;

            // When
            groupsDataService.DeleteGroupCustomisation(groupCustomisationId);

            // Then
            var groupCustomisations = await connection.GetGroupCustomisationIdsForGroup(groupId);
            groupCustomisations.Should().NotContain(groupCustomisationId);

            transaction.Dispose();
        }

        [TestCase("self_enrolled", 60, 13, 273606)]
        [TestCase("course_started", 60, 9, 282560)]
        [TestCase("course_completed", 60, 9, 282564)]
        [TestCase("non_group_candidate", 5, 28, 284992)]
        public async Task RemoveRelatedProgressRecordsForGroupCourse_should_not_remove_progress_for_case(
            string testCase,
            int groupId,
            int groupCustomisationId,
            int progressId
        )
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // When
            groupsDataService.RemoveRelatedProgressRecordsForGroupCourse(
                groupId,
                groupCustomisationId,
                false,
                DateTime.Now
            );

            // Then
            var (removalMethod, removalDate) = await connection.GetProgressRemovedFields(progressId);

            removalMethod.Should().Be(0);
            removalDate.Should().BeNull();

            transaction.Dispose();
        }

        [TestCase("course_not_started", 60, 12, 282619, false)]
        [TestCase("course_started_force_remove", 60, 9, 282560, true)]
        public async Task RemoveRelatedProgressRecordsForGroupCourse_should_remove_progress_for_case(
            string testCase,
            int groupId,
            int groupCustomisationId,
            int progressId,
            bool deleteStartedEnrolment
        )
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var progressRemoved = new DateTime(2021, 11, 6, 22, 23, 24, 567);

            // When
            groupsDataService.RemoveRelatedProgressRecordsForGroupCourse(
                groupId,
                groupCustomisationId,
                deleteStartedEnrolment,
                progressRemoved
            );

            // Then
            var (removalMethod, removalDate) = await connection.GetProgressRemovedFields(progressId);

            removalMethod.Should().Be(3);
            removalDate.Should().Be(progressRemoved);

            transaction.Dispose();
        }

        [Test]
        public async Task
            RemoveRelatedProgressRecordsForGroupCourse_should_not_remove_progress_when_course_in_additional_groups()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            const int progressRecordId = 285130;
            const int groupId = 8;
            const int groupCustomisationId = 1;

            AddDelegateToGroupWithSharedCourse();

            // When
            groupsDataService.RemoveRelatedProgressRecordsForGroupCourse(
                groupId,
                groupCustomisationId,
                false,
                DateTime.Now
            );

            // Then
            var (removalMethod, removalDate) = await connection.GetProgressRemovedFields(progressRecordId);

            removalMethod.Should().Be(0);
            removalDate.Should().BeNull();

            transaction.Dispose();
        }

        [Test]
        public void UpdateGroupDescription_updates_record()
        {
            // Given
            const int centerId = 101;
            const int groupId = 5;
            const string newDescription = "Test group description1";

            using var transaction = new TransactionScope();
            try
            {
                // When
                groupsDataService.UpdateGroupDescription(
                    groupId,
                    centerId,
                    newDescription
                );

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
            // Given
            const int incorrectCentreId = 107;
            const int groupId = 5;
            const string newDescription = "Test group description1";

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // When
                groupsDataService.UpdateGroupDescription(
                    groupId,
                    incorrectCentreId,
                    newDescription
                );

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

        [Test]
        public void GetGroupAtCentreByName_returns_expected_group()
        {
            // Given
            var expectedGroup = GroupTestHelper.GetDefaultGroup();
            var groupName = expectedGroup.GroupLabel;

            // When
            var result = groupsDataService.GetGroupAtCentreByName(groupName, 101);

            //Then
            result.Should().BeEquivalentTo(expectedGroup);
        }

        [Test]
        public void GetGroupAtCentreByName_returns_null_if_no_such_group_exists()
        {
            // Given
            var expectedGroup = GroupTestHelper.GetDefaultGroup();

            // When
            var result = groupsDataService.GetGroupAtCentreByName("Group name that does not exist", 101);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetGroupAtCentreByName_returns_null_with_incorrect_centreId()
        {
            // Given
            var expectedGroup = GroupTestHelper.GetDefaultGroup();
            const int incorrectCentreId = 1;

            // When
            var result = groupsDataService.GetGroupAtCentreByName(expectedGroup.GroupLabel, incorrectCentreId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void UpdateGroupName_updates_record()
        {
            // Given
            const int centerId = 101;
            const int groupId = 5;
            const string newGroupName = "Test group name";

            using var transaction = new TransactionScope();
            try
            {
                // When
                groupsDataService.UpdateGroupName(
                    groupId,
                    centerId,
                    newGroupName
                );

                // Then
                var result = GetGroupNameById(groupId);
                result.Should().Be(newGroupName);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void UpdateGroupName_with_incorrect_centreId_does_not_update_record()
        {
            // Given
            const int incorrectCentreId = 101;
            const int groupId = 59;
            const string expectedGroupName = "Nurses";

            using var transaction = new TransactionScope();
            try
            {
                // When
                groupsDataService.UpdateGroupName(
                    groupId,
                    incorrectCentreId,
                    "Test group name"
                );

                //Then
                var result = GetGroupNameById(groupId);
                result?.Should().Be(expectedGroupName);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void InsertGroupCustomisation_inserts_expected_record()
        {
            // Given
            var expectedDateTime = new DateTime(2019, 11, 15, 13, 53, 26, 510);
            var expectedGroupCourse = GroupTestHelper.GetDefaultGroupCourse(
                25,
                103,
                supervisorAdminId: 1,
                completeWithinMonths: 0,
                supervisorFirstName: "Kevin",
                supervisorLastName: "Whittaker (Developer)",
                addedToGroup: expectedDateTime
            );

            using var transaction = new TransactionScope();
            // When
            var insertedId = groupsDataService.InsertGroupCustomisation(
                expectedGroupCourse.GroupId,
                expectedGroupCourse.CustomisationId,
                expectedGroupCourse.CompleteWithinMonths,
                1,
                true,
                expectedGroupCourse.SupervisorAdminId
            );
            var result = groupsDataService.GetGroupCourseIfVisibleToCentre(insertedId, 101);

            // Then
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(
                    expectedGroupCourse,
                    options => options.Excluding(gc => gc.GroupCustomisationId).Excluding(gc => gc.AddedToGroup)
                );
            }
        }

        [Test]
        public void GetGroupCourseById_returns_expected_course()
        {
            // Given
            var expectedDateTime = new DateTime(2019, 11, 15, 13, 53, 26, 510);
            var expectedGroupCourse = GroupTestHelper.GetDefaultGroupCourse(
                25,
                103,
                supervisorAdminId: 1,
                completeWithinMonths: 0,
                supervisorFirstName: "Kevin",
                supervisorLastName: "Whittaker (Developer)",
                addedToGroup: expectedDateTime
            );

            // When
            var result = groupsDataService.GetGroupCourseById(25);

            // Then
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(expectedGroupCourse);
            }
        }

        [Test]
        public void AddDelegatesWithMatchingAnswersToGroup_adds_delegates_with_matching_answers_to_registration_prompt()
        {
            // Given
            const int delegateId = 254480;
            const int groupId = 100;

            using var transaction = new TransactionScope();
            try
            {
                // When
                groupsDataService.AddDelegatesWithMatchingAnswersToGroup(
                    100,
                    1,
                    101,
                    "Implementation and Business Change Manager",
                    null
                );
                var groupDelegates = groupsDataService.GetGroupDelegates(groupId).ToList();

                // Then
                groupDelegates.Count.Should().Be(1);
                groupDelegates.First().DelegateId.Should().Be(delegateId);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void AddDelegatesWithMatchingAnswersToGroup_adds_delegates_with_matching_job_group()
        {
            // Given
            const int delegateId = 254480;
            const int groupId = 100;

            using var transaction = new TransactionScope();
            try
            {
                // When
                groupsDataService.AddDelegatesWithMatchingAnswersToGroup(100, 7, 101, null, 9);
                var groupDelegates = groupsDataService.GetGroupDelegates(groupId).ToList();

                // Then
                groupDelegates.Count.Should().Be(33);
                groupDelegates.First().DelegateId.Should().Be(delegateId);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void
            AddDelegatesWithMatchingAnswersToGroup_does_not_add_delegates_to_group_if_delegate_has_matching_answer_to_different_prompt()
        {
            // Given
            const int groupId = 100;

            using var transaction = new TransactionScope();
            try
            {
                // When
                groupsDataService.AddDelegatesWithMatchingAnswersToGroup(
                    100,
                    2,
                    101,
                    "Implementation and Business Change Manager",
                    null
                );
                var groupDelegates = groupsDataService.GetGroupDelegates(groupId).ToList();

                // Then
                groupDelegates.Count.Should().Be(0);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void
            AddDelegatesWithMatchingAnswersToGroup_does_not_add_delegates_to_group_if_job_group_is_given_but_group_is_not_linked_to_job_group()
        {
            // Given
            const int groupId = 100;

            using var transaction = new TransactionScope();
            try
            {
                // When
                groupsDataService.AddDelegatesWithMatchingAnswersToGroup(100, 5, 101, null, 9);
                var groupDelegates = groupsDataService.GetGroupDelegates(groupId).ToList();

                // Then
                groupDelegates.Count.Should().Be(0);
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

        private string? GetGroupDescriptionById(int groupId)
        {
            return connection.Query<string?>(
                @"SELECT GroupDescription FROM Groups
                    WHERE GroupID = @groupId",
                new { groupId }
            ).FirstOrDefault();
        }

        private string? GetGroupNameById(int groupId)
        {
            return connection.Query<string?>(
                @"SELECT GroupLabel FROM Groups
                    WHERE GroupID = @groupId",
                new { groupId }
            ).FirstOrDefault();
        }
    }
}
