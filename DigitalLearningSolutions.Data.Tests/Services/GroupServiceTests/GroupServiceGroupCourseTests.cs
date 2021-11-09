namespace DigitalLearningSolutions.Data.Tests.Services.GroupServiceTests
{
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;
    using System;

    partial class GroupsServiceTests
    {
        [Test]
        public void GetGroupCourse_returns_null_when_ids_not_match()
        {
            // Given
            A.CallTo(() => groupsDataService.GetGroupCourse(A<int>._, A<int>._, A<int>._)).Returns(null!);

            // When
            var result = groupsService.GetGroupCourse(25, 103, 15);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetGroupCourse_returns_groupCourse_when_ids_match()
        {
            // Given
            A.CallTo(() => groupsDataService.GetGroupCourse(A<int>._, A<int>._, A<int>._)).Returns(new GroupCourse());

            // When
            var result = groupsService.GetGroupCourse(25, 103, 101);

            // Then
            result.Should().NotBeNull();
        }

        [Test]
        public void RemoveGroupCourseAndRelatedProgress_calls_expected_groupsDataServices()
        {
            // Given
            var groupCustomisationId = 25;
            var groupId = 103;
            var deleteStartedEnrolment = true;

            // When
            groupsService.RemoveGroupCourseAndRelatedProgress(groupCustomisationId, groupId, deleteStartedEnrolment);

            // Then
            A.CallTo(() => groupsDataService.RemoveRelatedProgressRecordsForCourse(groupId, groupCustomisationId, deleteStartedEnrolment, A<DateTime>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => groupsDataService.DeleteGroupCustomisation(groupCustomisationId))
                .MustHaveHappenedOnceExactly();
        }
    }
}
