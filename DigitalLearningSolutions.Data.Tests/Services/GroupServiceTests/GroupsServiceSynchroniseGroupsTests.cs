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
            var centreAnswersData = UserTestHelper.GetDefaultCentreAnswersData();
            var nonSynchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "new answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: false
            );
            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(
                new List<Group> { nonSynchronisedGroup }
            );

            // When
            groupsService.SynchroniseUserChangesWithGroups(
                delegateDetails,
                reusableMyAccountDetailsData,
                centreAnswersData
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
            var centreAnswersData = UserTestHelper.GetDefaultCentreAnswersData(answer1: "new answer");
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
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                centreAnswersData
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
            var centreAnswersData = UserTestHelper.GetDefaultCentreAnswersData(answer1: "new answer");
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
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                centreAnswersData
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
            var centreAnswersData = UserTestHelper.GetDefaultCentreAnswersData(answer1: "new answer");
            const bool removeStartedEnrolments = false;
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
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                centreAnswersData
            );

            // Then
            A.CallTo(
                () => groupsDataService.DeleteGroupDelegatesRecordForDelegate(
                    synchronisedGroup.GroupId,
                    reusableDelegateDetails.Id
                )
            ).MustHaveHappened();
            A.CallTo(
                () => groupsDataService.RemoveRelatedProgressRecordsForGroup(
                    synchronisedGroup.GroupId,
                    reusableDelegateDetails.Id,
                    removeStartedEnrolments,
                    testDate
                )
            ).MustHaveHappened();
        }

        [Test]
        public void
            SynchroniseUserChangesWithGroups_removes_delegate_from_synchronised_old_answer_group_when_group_label_includes_prompt_name()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultCentreAnswersData(answer1: "new answer");
            const bool removeStartedEnrolments = false;
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
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                centreAnswersData
            );

            // Then
            A.CallTo(
                () => groupsDataService.DeleteGroupDelegatesRecordForDelegate(
                    synchronisedGroup.GroupId,
                    reusableDelegateDetails.Id
                )
            ).MustHaveHappened();
            A.CallTo(
                () => groupsDataService.RemoveRelatedProgressRecordsForGroup(
                    synchronisedGroup.GroupId,
                    reusableDelegateDetails.Id,
                    removeStartedEnrolments,
                    testDate
                )
            ).MustHaveHappened();
        }

        [Test]
        public void SynchroniseUserChangesWithGroups_adds_delegate_to_synchronised_new_answer_group()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultCentreAnswersData(answer1: "new answer");
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
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                centreAnswersData
            );

            // Then
            A.CallTo(
                () => groupsDataService.AddDelegateToGroup(
                    reusableDelegateDetails.Id,
                    synchronisedGroup.GroupId,
                    testDate,
                    1
                )
            ).MustHaveHappened();
        }

        [Test]
        public void
            SynchroniseUserChangesWithGroups_adds_delegate_to_synchronised_new_answer_group_when_group_label_includes_prompt_name()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultCentreAnswersData(answer1: "new answer");
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
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                centreAnswersData
            );

            // Then
            A.CallTo(
                () => groupsDataService.AddDelegateToGroup(
                    reusableDelegateDetails.Id,
                    synchronisedGroup.GroupId,
                    testDate,
                    1
                )
            ).MustHaveHappened();
        }
    }
}
