namespace DigitalLearningSolutions.Data.Tests.Services.GroupServiceTests
{
    using System;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using NUnit.Framework;

    partial class GroupsServiceTests
    {
        [Test]
        public void GetActiveGroupCourse_returns_null_when_data_service_returns_null()
        {
            // Given
            A.CallTo(() => groupsDataService.GetGroupCourseForCentre(A<int>._, A<int>._)).Returns(null);

            // When
            var result = groupsService.GetUsableGroupCourseForCentre(25, 103, 15);

            // Then
            result.Should().BeNull();
            A.CallTo(() => groupsDataService.GetGroupCourseForCentre(25, 15)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetActiveGroupCourse_returns_null_when_data_service_returns_inactive_course()
        {
            // Given
            var groupCourse = Builder<GroupCourse>.CreateNew()
                .With(g => g.Active = false)
                .Build();
            A.CallTo(() => groupsDataService.GetGroupCourseForCentre(A<int>._, A<int>._))
                .Returns(groupCourse);

            // When
            var result = groupsService.GetUsableGroupCourseForCentre(25, 103, 15);

            // Then
            result.Should().BeNull();
            A.CallTo(() => groupsDataService.GetGroupCourseForCentre(25, 15)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetActiveGroupCourse_returns_null_when_data_service_returns_archived_application()
        {
            // Given
            var groupCourse = Builder<GroupCourse>.CreateNew()
                .With(g => g.Active = true)
                .With(g => g.ApplicationArchivedDate = DateTime.Now)
                .Build();
            A.CallTo(() => groupsDataService.GetGroupCourseForCentre(A<int>._, A<int>._))
                .Returns(groupCourse);

            // When
            var result = groupsService.GetUsableGroupCourseForCentre(25, 103, 15);

            // Then
            result.Should().BeNull();
            A.CallTo(() => groupsDataService.GetGroupCourseForCentre(25, 15)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetActiveGroupCourse_returns_null_when_data_service_returns_course_with_inactive_date()
        {
            // Given
            var groupCourse = Builder<GroupCourse>.CreateNew()
                .With(g => g.Active = true)
                .With(g => g.InactivatedDate = DateTime.Now)
                .Build();
            A.CallTo(() => groupsDataService.GetGroupCourseForCentre(A<int>._, A<int>._))
                .Returns(groupCourse);

            // When
            var result = groupsService.GetUsableGroupCourseForCentre(25, 103, 15);

            // Then
            result.Should().BeNull();
            A.CallTo(() => groupsDataService.GetGroupCourseForCentre(25, 15)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetActiveGroupCourse_returns_groupCourse_from_data_service()
        {
            // Given
            var expectedGroupCourse = Builder<GroupCourse>.CreateNew()
                .With(g => g.GroupCustomisationId = 25)
                .With(g => g.GroupId = 103)
                .With(g => g.ApplicationName = "Test Application")
                .With(g => g.CustomisationName = "My Customisation")
                .With(g => g.Active = true)
                .With(g => g.InactivatedDate = null)
                .With(g => g.ApplicationArchivedDate = null)
                .Build();
            A.CallTo(() => groupsDataService.GetGroupCourseForCentre(A<int>._, A<int>._))
                .Returns(expectedGroupCourse);

            // When
            var result = groupsService.GetUsableGroupCourseForCentre(25, 103, 101);

            // Then
            result.Should().NotBeNull()
                .And.BeEquivalentTo(expectedGroupCourse);
            A.CallTo(() => groupsDataService.GetGroupCourseForCentre(25, 101)).MustHaveHappenedOnceExactly();
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
            A.CallTo(
                    () => groupsDataService.RemoveRelatedProgressRecordsForGroupCourse(
                        groupId,
                        groupCustomisationId,
                        deleteStartedEnrolment,
                        A<DateTime>._
                    )
                )
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => groupsDataService.DeleteGroupCustomisation(groupCustomisationId))
                .MustHaveHappenedOnceExactly();
        }
    }
}
