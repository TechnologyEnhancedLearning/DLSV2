namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using System;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class GroupCourseViewModelTests
    {
        [Test]
        public void GroupCourseViewModel_populates_expected_values()
        {
            // Given
            var expectedDateTime = new DateTime(2018, 11, 02, 10, 53, 38, 920);
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(addedToGroup: expectedDateTime, supervisorActive: true);

            // When
            var result = new GroupCourseViewModel(groupCourse, ReturnPageQueryHelper.GetDefaultReturnPageQuery());

            // Then
            using (new AssertionScope())
            {
                result.GroupCustomisationId.Should().Be(1);
                result.Name.Should().BeEquivalentTo(groupCourse.CourseName);
                result.IsMandatory.Should().Be("Not mandatory");
                result.IsAssessed.Should().Be("Not assessed");
                result.AddedToGroup.Should().Be("02/11/2018");
                result.Supervisor.Should().BeNull();
                result.CompleteWithin.Should().Be("12 months");
                result.ValidFor.Should().BeNull();
            }
        }

        [Test]
        public void GroupCourseViewModel_populates_expected_values_with_supervisor_name()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(
                supervisorFirstName: "Test",
                supervisorLastName: "Name",
                supervisorActive: true
            );

            // When
            var result = new GroupCourseViewModel(groupCourse, ReturnPageQueryHelper.GetDefaultReturnPageQuery());

            // Then
            result.Supervisor.Should().Be("Test Name");
        }

        [Test]
        public void GroupCourseViewModel_populates_expected_values_for_mandatory_assessed_course()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(isMandatory: true, isAssessed: true);

            // When
            var result = new GroupCourseViewModel(groupCourse, ReturnPageQueryHelper.GetDefaultReturnPageQuery());

            // Then
            using (new AssertionScope())
            {
                result.IsMandatory.Should().Be("Mandatory");
                result.IsAssessed.Should().Be("Assessed");
            }
        }

        [Test]
        public void GroupCourseViewModel_populates_expected_complete_within_value_for_one_month()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(
                completeWithinMonths: 1
            );

            // When
            var result = new GroupCourseViewModel(groupCourse, ReturnPageQueryHelper.GetDefaultReturnPageQuery());

            // Then
            result.CompleteWithin.Should().Be("1 month");
        }
    }
}
