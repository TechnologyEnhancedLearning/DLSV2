namespace DigitalLearningSolutions.Data.Tests.Services.GroupServiceTests
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public partial class GroupsServiceTests
    {
        [Test]
        public void SynchroniseUserChangesWithGroups_does_nothing_if_no_groups_need_synchronising()
        {
            // Given
            var delegateDetails = UserTestHelper.GetDefaultDelegateUser();
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers();
            var nonSynchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "new answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: false
            );
            A.CallTo(() => userDataService.GetDelegateUserById(delegateDetails.Id)).Returns(delegateDetails);
            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(
                new List<Group> { nonSynchronisedGroup }
            );

            // When
            groupsService.SynchroniseUserChangesWithGroups(
                delegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                null,
                delegateAccountWithOldDetails.GetRegistrationFieldAnswers()
            );

            // Then
            using (new AssertionScope())
            {
                DelegateMustNotHaveBeenRemovedFromAGroup();
                DelegateMustNotHaveBeenAddedToAGroup();
                DelegateProgressRecordMustNotHaveBeenUpdated();
                NewDelegateProgressRecordMustNotHaveBeenAdded();
                NoEnrolmentEmailsMustHaveBeenSent();
            }
        }

        [Test]
        public void SynchroniseUserChangesWithGroups_does_nothing_if_synchronised_groups_are_not_for_changed_fields()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            var synchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "new answer",
                linkedToField: 2,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );
            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(
                new List<Group> { synchronisedGroup }
            );

            // When
            groupsService.SynchroniseUserChangesWithGroups(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                null,
                delegateAccountWithOldDetails.GetRegistrationFieldAnswers()
            );

            // Then
            using (new AssertionScope())
            {
                DelegateMustNotHaveBeenRemovedFromAGroup();
                DelegateMustNotHaveBeenAddedToAGroup();
                DelegateProgressRecordMustNotHaveBeenUpdated();
                NewDelegateProgressRecordMustNotHaveBeenAdded();
                NoEnrolmentEmailsMustHaveBeenSent();
            }
        }

        [Test]
        public void
            SynchroniseUserChangesWithGroups_does_nothing_if_synchronised_groups_for_changed_fields_have_different_values()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            var synchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "differentValue",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );
            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(
                new List<Group> { synchronisedGroup }
            );

            // When
            groupsService.SynchroniseUserChangesWithGroups(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                null,
                delegateAccountWithOldDetails.GetRegistrationFieldAnswers()
            );

            // Then
            using (new AssertionScope())
            {
                DelegateMustNotHaveBeenRemovedFromAGroup();
                DelegateMustNotHaveBeenAddedToAGroup();
                DelegateProgressRecordMustNotHaveBeenUpdated();
                NewDelegateProgressRecordMustNotHaveBeenAdded();
                NoEnrolmentEmailsMustHaveBeenSent();
            }
        }

        [Test]
        public void SynchroniseUserChangesWithGroups_removes_delegate_from_synchronised_old_answer_group()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockService.UtcNow).Returns(testDate);
            var synchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "old answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );
            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(
                new List<Group> { synchronisedGroup }
            );

            // When
            groupsService.SynchroniseUserChangesWithGroups(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                null,
                delegateAccountWithOldDetails.GetRegistrationFieldAnswers()
            );

            // Then
            DelegateMustHaveBeenRemovedFromGroups(new List<int> { synchronisedGroup.GroupId });
        }

        [Test]
        public void
            SynchroniseUserChangesWithGroups_removes_delegate_from_synchronised_old_answer_group_when_group_label_includes_prompt_name()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockService.UtcNow).Returns(testDate);
            A.CallTo(
                () => centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(
                    reusableDelegateDetails.CentreId,
                    1
                )
            ).Returns("Prompt Name");
            var synchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "Prompt Name - old answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );
            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(
                new List<Group> { synchronisedGroup }
            );

            // When
            groupsService.SynchroniseUserChangesWithGroups(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                null,
                delegateAccountWithOldDetails.GetRegistrationFieldAnswers()
            );

            // Then
            DelegateMustHaveBeenRemovedFromGroups(new List<int> { synchronisedGroup.GroupId });
        }

        [Test]
        public void
            SynchroniseUserChangesWithGroups_removes_delegate_from_all_synchronised_old_answer_groups_if_multiple_exist()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockService.UtcNow).Returns(testDate);
            A.CallTo(
                () => centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(
                    reusableDelegateDetails.CentreId,
                    1
                )
            ).Returns("Prompt Name");
            var synchronisedGroup1 = GroupTestHelper.GetDefaultGroup(
                5,
                "old answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );
            var synchronisedGroup2 = GroupTestHelper.GetDefaultGroup(
                6,
                "Prompt Name - old answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );
            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(
                new List<Group> { synchronisedGroup1, synchronisedGroup2 }
            );

            // When
            groupsService.SynchroniseUserChangesWithGroups(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                null,
                delegateAccountWithOldDetails.GetRegistrationFieldAnswers()
            );

            // Then
            DelegateMustHaveBeenRemovedFromGroups(
                new List<int> { synchronisedGroup1.GroupId, synchronisedGroup2.GroupId }
            );
        }

        [Test]
        public void SynchroniseUserChangesWithGroups_adds_delegate_to_synchronised_new_answer_group()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockService.UtcNow).Returns(testDate);
            var synchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "new answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );
            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(
                new List<Group> { synchronisedGroup }
            );

            // When
            groupsService.SynchroniseUserChangesWithGroups(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                null,
                delegateAccountWithOldDetails.GetRegistrationFieldAnswers()
            );

            // Then
            DelegateMustHaveBeenAddedToGroups(new List<int> { synchronisedGroup.GroupId });
        }

        [Test]
        public void
            SynchroniseUserChangesWithGroups_adds_delegate_to_synchronised_new_answer_group_when_group_label_includes_prompt_name()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockService.UtcNow).Returns(testDate);
            A.CallTo(
                () => centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(
                    reusableDelegateDetails.CentreId,
                    1
                )
            ).Returns("Prompt Name");

            var synchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "Prompt Name - new answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );
            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(
                new List<Group> { synchronisedGroup }
            );

            // When
            groupsService.SynchroniseUserChangesWithGroups(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                null,
                delegateAccountWithOldDetails.GetRegistrationFieldAnswers()
            );

            // Then
            DelegateMustHaveBeenAddedToGroups(new List<int> { synchronisedGroup.GroupId });
        }

        [Test]
        public void
            SynchroniseUserChangesWithGroups_adds_delegate_to_all_synchronised_new_answer_groups_if_multiple_exist()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockService.UtcNow).Returns(testDate);
            A.CallTo(
                () => centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(
                    reusableDelegateDetails.CentreId,
                    1
                )
            ).Returns("Prompt Name");
            var synchronisedGroup1 = GroupTestHelper.GetDefaultGroup(
                5,
                "new answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );
            var synchronisedGroup2 = GroupTestHelper.GetDefaultGroup(
                6,
                "Prompt Name - new answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );
            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(
                new List<Group> { synchronisedGroup1, synchronisedGroup2 }
            );

            // When
            groupsService.SynchroniseUserChangesWithGroups(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                null,
                delegateAccountWithOldDetails.GetRegistrationFieldAnswers()
            );

            // Then
            DelegateMustHaveBeenAddedToGroups(new List<int> { synchronisedGroup1.GroupId, synchronisedGroup2.GroupId });
        }

        [Test]
        public void
            SynchroniseUserChangesWithGroups_adds_delegate_to_synchronised_new_answer_groups_when_group_labels_differ_in_casing()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockService.UtcNow).Returns(testDate);
            A.CallTo(
                () => centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(
                    reusableDelegateDetails.CentreId,
                    1
                )
            ).Returns("Prompt name");

            var synchronisedGroup1 = GroupTestHelper.GetDefaultGroup(
                5,
                "NEW ANSWER",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );
            var synchronisedGroup2 = GroupTestHelper.GetDefaultGroup(
                6,
                "PROMPT NAME - NEW ANSWER",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );
            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(
                new List<Group> { synchronisedGroup1, synchronisedGroup2 }
            );

            // When
            groupsService.SynchroniseUserChangesWithGroups(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                null,
                delegateAccountWithOldDetails.GetRegistrationFieldAnswers()
            );

            // Then
            DelegateMustHaveBeenAddedToGroups(new List<int> { synchronisedGroup1.GroupId, synchronisedGroup2.GroupId });
        }

        private void DelegateMustHaveBeenRemovedFromGroups(IEnumerable<int> groupIds)
        {
            foreach (var groupId in groupIds)
            {
                A.CallTo(
                    () => groupsDataService.DeleteGroupDelegatesRecordForDelegate(
                        groupId,
                        reusableDelegateDetails.Id
                    )
                ).MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => groupsDataService.RemoveRelatedProgressRecordsForGroup(
                        groupId,
                        reusableDelegateDetails.Id,
                        false,
                        testDate
                    )
                ).MustHaveHappenedOnceExactly();
            }
        }

        private void DelegateMustHaveBeenAddedToGroups(IEnumerable<int> groupIds)
        {
            foreach (var groupId in groupIds)
            {
                A.CallTo(
                    () => groupsDataService.AddDelegateToGroup(
                        reusableDelegateDetails.Id,
                        groupId,
                        testDate,
                        1
                    )
                ).MustHaveHappenedOnceExactly();
            }
        }
    }
}
