﻿namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using NUnit.Framework;
    using FluentAssertions;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class GroupCourseRemoveViewModelTests
    {
        [Test]
        public void GroupCourseRemoveViewModel_Constructor_CorrectlySetsProperties()
        {
            // Given
            var courseId = 1;
            var courseName = "Test Course";
            var groupName = "Test Group";

            // When
            var result = new GroupCourseRemoveViewModel(courseId, courseName, groupName);

            // Then
            result.GroupCourseId.Should().Be(1);
            result.CourseName.Should().Be("Test Course");
            result.GroupName.Should().Be("Test Group");
        }

        [Test]
        public void GroupCourseRemoveViewModel_Validate_returns_error_when_confirm_is_false()
        {
            // Given
            var model = new GroupCourseRemoveViewModel();

            // When
            var result = model.Validate(new ValidationContext(model)).ToList();

            // Then
            result.Should().NotBeEmpty();
            result.ElementAt(0).ErrorMessage.Should().Be("Confirm you wish to remove this course");
        }

        [Test]
        public void GroupCourseRemoveViewModel_Validate_does_not_return_error_when_confirm_is_true()
        {
            // Given
            var model = new GroupCourseRemoveViewModel
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
