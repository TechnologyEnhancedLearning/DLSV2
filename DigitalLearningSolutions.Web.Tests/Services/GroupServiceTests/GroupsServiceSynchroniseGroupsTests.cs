namespace DigitalLearningSolutions.Web.Tests.Services.GroupServiceTests
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.User;
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
            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(
                new List<Group> { nonSynchronisedGroup }
            );

            // When
            groupsService.UpdateDelegateGroupsBasedOnUserChanges(
                delegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                delegateDetails.GetRegistrationFieldAnswers(),
                null,
                new List<Group>()
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
            UpdateDelegateGroupsBasedOnUserChanges_does_nothing_if_synchronised_groups_are_not_for_changed_fields()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            var synchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "new answer",
                linkedToField: 2,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );

            // When
            groupsService.UpdateDelegateGroupsBasedOnUserChanges(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                UserTestHelper.GetDefaultRegistrationFieldAnswers(reusableDelegateDetails.CentreId),
                null,
                new List<Group> { synchronisedGroup }
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
            UpdateDelegateGroupsBasedOnUserChanges_does_nothing_if_synchronised_groups_for_changed_fields_have_different_values()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            var synchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "differentValue",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );

            // When
            groupsService.UpdateDelegateGroupsBasedOnUserChanges(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                UserTestHelper.GetDefaultRegistrationFieldAnswers(reusableDelegateDetails.CentreId),
                null,
                new List<Group> { synchronisedGroup }
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
        public void UpdateDelegateGroupsBasedOnUserChanges_removes_delegate_from_synchronised_old_answer_group()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockUtility.UtcNow).Returns(testDate);
            var synchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "old answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );

            // When
            groupsService.UpdateDelegateGroupsBasedOnUserChanges(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                UserTestHelper.GetDefaultRegistrationFieldAnswers(
                    reusableDelegateDetails.CentreId,
                    answer1: "old answer"
                ),
                null,
                new List<Group> { synchronisedGroup }
            );

            // Then
            DelegateMustHaveBeenRemovedFromGroups(new List<int> { synchronisedGroup.GroupId });
        }

        [Test]
        public void
            UpdateDelegateGroupsBasedOnUserChanges_removes_delegate_from_synchronised_old_answer_group_when_group_label_includes_prompt_name()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockUtility.UtcNow).Returns(testDate);
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

            // When
            groupsService.UpdateDelegateGroupsBasedOnUserChanges(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                UserTestHelper.GetDefaultRegistrationFieldAnswers(
                    reusableDelegateDetails.CentreId,
                    answer1: "old answer"
                ),
                null,
                new List<Group> { synchronisedGroup }
            );

            // Then
            DelegateMustHaveBeenRemovedFromGroups(new List<int> { synchronisedGroup.GroupId });
        }

        [Test]
        public void
            UpdateDelegateGroupsBasedOnUserChanges_removes_delegate_from_all_synchronised_old_answer_groups_if_multiple_exist()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockUtility.UtcNow).Returns(testDate);
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

            // When
            groupsService.UpdateDelegateGroupsBasedOnUserChanges(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                UserTestHelper.GetDefaultRegistrationFieldAnswers(
                    reusableDelegateDetails.CentreId,
                    answer1: "old answer"
                ),
                null,
                new List<Group> { synchronisedGroup1, synchronisedGroup2 }
            );

            // Then
            DelegateMustHaveBeenRemovedFromGroups(
                new List<int> { synchronisedGroup1.GroupId, synchronisedGroup2.GroupId }
            );
        }

        [Test]
        public void UpdateDelegateGroupsBasedOnUserChanges_adds_delegate_to_synchronised_new_answer_group()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockUtility.UtcNow).Returns(testDate);
            var synchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "new answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true
            );

            // When
            groupsService.UpdateDelegateGroupsBasedOnUserChanges(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                UserTestHelper.GetDefaultRegistrationFieldAnswers(reusableDelegateDetails.CentreId),
                null,
                new List<Group> { synchronisedGroup }
            );

            // Then
            DelegateMustHaveBeenAddedToGroups(new List<int> { synchronisedGroup.GroupId });
        }

        [Test]
        public void
            UpdateDelegateGroupsBasedOnUserChanges_adds_delegate_to_synchronised_new_answer_group_when_group_label_includes_prompt_name()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockUtility.UtcNow).Returns(testDate);
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

            // When
            groupsService.UpdateDelegateGroupsBasedOnUserChanges(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                UserTestHelper.GetDefaultRegistrationFieldAnswers(reusableDelegateDetails.CentreId),
                null,
                new List<Group> { synchronisedGroup }
            );

            // Then
            DelegateMustHaveBeenAddedToGroups(new List<int> { synchronisedGroup.GroupId });
        }

        [Test]
        public void
            UpdateDelegateGroupsBasedOnUserChanges_adds_delegate_to_all_synchronised_new_answer_groups_if_multiple_exist()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockUtility.UtcNow).Returns(testDate);
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

            // When
            groupsService.UpdateDelegateGroupsBasedOnUserChanges(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                UserTestHelper.GetDefaultRegistrationFieldAnswers(reusableDelegateDetails.CentreId),
                null,
                new List<Group> { synchronisedGroup1, synchronisedGroup2 }
            );

            // Then
            DelegateMustHaveBeenAddedToGroups(new List<int> { synchronisedGroup1.GroupId, synchronisedGroup2.GroupId });
        }

        [Test]
        public void
            UpdateDelegateGroupsBasedOnUserChanges_adds_delegate_to_synchronised_new_answer_groups_when_group_labels_differ_in_casing()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer");
            A.CallTo(() => clockUtility.UtcNow).Returns(testDate);
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

            // When
            groupsService.UpdateDelegateGroupsBasedOnUserChanges(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                UserTestHelper.GetDefaultRegistrationFieldAnswers(reusableDelegateDetails.CentreId),
                null,
                new List<Group> { synchronisedGroup1, synchronisedGroup2 }
            );

            // Then
            DelegateMustHaveBeenAddedToGroups(new List<int> { synchronisedGroup1.GroupId, synchronisedGroup2.GroupId });
        }

        [Test]
        public void UpdateSynchronisedDelegateGroupsBasedOnUserChanges_adds_delegate_to_appropriate_groups()
        {
            // Given
            var centreAnswersData = UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: "new answer", answer2: "new answer2");
            A.CallTo(() => clockUtility.UtcNow).Returns(testDate);
            var synchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "new answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: true,
                shouldAddNewRegistrantsToGroup: false
            );
            var unsynchronisedGroup = GroupTestHelper.GetDefaultGroup(
                6,
                "new answer2",
                linkedToField: 2,
                changesToRegistrationDetailsShouldChangeGroupMembership: false,
                shouldAddNewRegistrantsToGroup: true
            );
            A.CallTo(() => groupsDataService.GetGroupsForCentre(centreAnswersData.CentreId))
                .Returns(new List<Group> { synchronisedGroup, unsynchronisedGroup });

            // When
            groupsService.UpdateSynchronisedDelegateGroupsBasedOnUserChanges(
                reusableDelegateDetails.Id,
                reusableEditAccountDetailsData,
                centreAnswersData,
                UserTestHelper.GetDefaultRegistrationFieldAnswers(reusableDelegateDetails.CentreId),
                null
            );

            // Then
            DelegateMustHaveBeenAddedToGroups(new List<int> { synchronisedGroup.GroupId });
            A.CallTo(
                () => groupsDataService.AddDelegateToGroup(
                    reusableDelegateDetails.Id,
                    unsynchronisedGroup.GroupId,
                    A<DateTime>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void AddNewDelegateToAppropriateGroups_adds_delegate_to_appropriate_groups()
        {
            // Given
            var registrationModel = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                answer1: "new answer",
                answer2: "new answer2",
                centre: 1
            );
            A.CallTo(() => clockUtility.UtcNow).Returns(testDate);
            var synchronisedGroup = GroupTestHelper.GetDefaultGroup(
                5,
                "new answer",
                linkedToField: 1,
                changesToRegistrationDetailsShouldChangeGroupMembership: false,
                shouldAddNewRegistrantsToGroup: true
            );
            var unsynchronisedGroup = GroupTestHelper.GetDefaultGroup(
                6,
                "new answer2",
                linkedToField: 2,
                changesToRegistrationDetailsShouldChangeGroupMembership: true,
                shouldAddNewRegistrantsToGroup: false
            );
            A.CallTo(() => groupsDataService.GetGroupsForCentre(registrationModel.Centre))
                .Returns(new List<Group> { synchronisedGroup, unsynchronisedGroup });
            A.CallTo(() => jobGroupsDataService.GetJobGroupName(0)).Returns(null);
            A.CallTo(() => jobGroupsDataService.GetJobGroupName(1)).Returns(null);

            // When
            groupsService.AddNewDelegateToAppropriateGroups(
                reusableDelegateDetails.Id,
                registrationModel
            );

            // Then
            DelegateMustHaveBeenAddedToGroups(new List<int> { synchronisedGroup.GroupId });
            A.CallTo(
                () => groupsDataService.AddDelegateToGroup(
                    reusableDelegateDetails.Id,
                    unsynchronisedGroup.GroupId,
                    A<DateTime>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void SynchroniseJobGroupsOnOtherCentres_synchronises_correct_job_groups()
        {
            // Given
            A.CallTo(() => clockUtility.UtcNow).Returns(testDate);
            var originalDelegateId = 1;
            var userId = 4;
            var oldJobGroupId = 2;
            var newJobGroupId = 3;
            var accountDetailsData = new AccountDetailsData("test", "tester", "fake@email.com");
            var centreEmail = "centreEmail";

            var delegateAccount = UserTestHelper.GetDefaultDelegateAccount();
            var delegateAccounts = new List<DelegateAccount>{ delegateAccount };

            var oldJobGroupGroup = GroupTestHelper.GetDefaultGroup(1, linkedToField: 4, groupLabel: "old group");
            var newJobGroupGroup = GroupTestHelper.GetDefaultGroup(2, linkedToField: 4, groupLabel: "new group");
            var nonJobGroupGroup = GroupTestHelper.GetDefaultGroup(3, linkedToField: 3, groupLabel: "new group");
            var groups = new List<Group> { oldJobGroupGroup, newJobGroupGroup, nonJobGroupGroup };

            A.CallTo(() => userDataService.GetUserIdFromDelegateId(originalDelegateId)).Returns(userId);
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(userId)).Returns(delegateAccounts);
            A.CallTo(() => groupsDataService.GetGroupsForCentre(delegateAccount.CentreId))
                .Returns(groups);

            A.CallTo(() => jobGroupsDataService.GetJobGroupName(oldJobGroupId))
                .Returns(oldJobGroupGroup.GroupLabel);
            A.CallTo(() => jobGroupsDataService.GetJobGroupName(newJobGroupId))
                .Returns(newJobGroupGroup.GroupLabel);

            // When
            groupsService.SynchroniseJobGroupsOnOtherCentres(
                originalDelegateId,
                oldJobGroupId,
                newJobGroupId,
                accountDetailsData,
                centreEmail
            );

            // Then
            A.CallTo(
                () => groupsDataService.DeleteGroupDelegatesRecordForDelegate(
                    oldJobGroupGroup.GroupId,
                    delegateAccount.Id
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => groupsDataService.DeleteGroupDelegatesRecordForDelegate(
                    nonJobGroupGroup.GroupId,
                    A<int>._
                )
            ).MustNotHaveHappened();

            A.CallTo(
                () => groupsDataService.AddDelegateToGroup(delegateAccount.Id, newJobGroupGroup.GroupId, testDate, 1)
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => groupsDataService.AddDelegateToGroup(A<int>._, nonJobGroupGroup.GroupId, A<DateTime>._, A<int>._)
            ).MustNotHaveHappened();

            // no call to non-job group on other centre
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
