namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
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
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(addedToGroup: expectedDateTime);

            // When
            var result = new GroupCourseViewModel(groupCourse);

            // Then
            using (new AssertionScope())
            {
                result.GroupCustomisationId.Should().Be(1);
                result.Name.Should().BeEquivalentTo(groupCourse.CourseName);
                result.IsMandatory.Should().Be("Not mandatory");
                result.IsAssessed.Should().Be("Not assessed");
                result.AddedToGroup.Should().Be(expectedDateTime);
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
                supervisorLastName: "Name"
            );

            // When
            var result = new GroupCourseViewModel(groupCourse);

            // Then
            result.Supervisor.Should().Be("Test Name");
        }

        [Test]
        public void GroupCourseViewModel_populates_expected_values_for_mandatory_assessed_course()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(isMandatory: true, isAssessed: true);

            // When
            var result = new GroupCourseViewModel(groupCourse);

            // Then
            using (new AssertionScope())
            {
                result.IsMandatory.Should().Be("Mandatory");
                result.IsAssessed.Should().Be("Assessed");
            }
        }
    }
}
