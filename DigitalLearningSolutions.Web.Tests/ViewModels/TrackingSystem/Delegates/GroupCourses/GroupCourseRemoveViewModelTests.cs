namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses;
    using FluentAssertions;
    using NUnit.Framework;

    public class RemoveGroupCourseViewModelTests
    {
        [Test]
        public void RemoveGroupCourseViewModel_Constructor_CorrectlySetsProperties()
        {
            // Given
            var courseId = 1;
            var courseName = "Test Course";
            var groupName = "Test Group";

            // When
            var result = new RemoveGroupCourseViewModel(courseId, courseName, groupName);

            // Then
            result.GroupCourseId.Should().Be(1);
            result.CourseName.Should().Be("Test Course");
            result.GroupName.Should().Be("Test Group");
        }

        [Test]
        public void RemoveGroupCourseViewModel_Validate_returns_error_when_confirm_is_false()
        {
            // Given
            var model = new RemoveGroupCourseViewModel();

            // When
            var result = model.Validate(new ValidationContext(model)).ToList();

            // Then
            result.Should().NotBeEmpty();
            result.ElementAt(0).ErrorMessage.Should().Be("Confirm you wish to remove this course");
        }

        [Test]
        public void RemoveGroupCourseViewModel_Validate_does_not_return_error_when_confirm_is_true()
        {
            // Given
            var model = new RemoveGroupCourseViewModel
            {
                Confirm = true,
            };

            // When
            var result = model.Validate(new ValidationContext(model));

            // Then
            result.Should().BeEmpty();
        }
    }
}
