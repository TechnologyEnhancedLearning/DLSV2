namespace DigitalLearningSolutions.Data.Tests.Services.GroupServiceTests
{
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;
    using System;
    using FizzWare.NBuilder;

    partial class GroupsServiceTests
    {
        [Test]
        public void GetGroupCourse_returns_null_when_data_service_returns_null()
        {
            // Given
            A.CallTo(() => groupsDataService.GetGroupCourse(A<int>._, A<int>._, A<int>._)).Returns(null);

            // When
            var result = groupsService.GetGroupCourse(25, 103, 15);

            // Then
            result.Should().BeNull();
            A.CallTo(() => groupsDataService.GetGroupCourse(25, 103, 15)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetGroupCourse_returns_groupCourse_from_data_service()
        {
            // Given
            var expectedGroupCourse = Builder<GroupCourse>.CreateNew()
                .With(g => g.GroupCustomisationId = 25)
                .With(g => g.GroupId = 103)
                .With(g => g.ApplicationName = "Test Application")
                .With(g => g.CustomisationName = "My Customisation")
                .Build();
            A.CallTo(() => groupsDataService.GetGroupCourse(A<int>._, A<int>._, A<int>._))
                .Returns(expectedGroupCourse);

            // When
            var result = groupsService.GetGroupCourse(25, 103, 101);

            // Then
            result.Should().NotBeNull()
                .And.BeEquivalentTo(expectedGroupCourse);
            A.CallTo(() => groupsDataService.GetGroupCourse(25, 103, 101)).MustHaveHappenedOnceExactly();
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
