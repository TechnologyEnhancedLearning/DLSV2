namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available;
    using FluentAssertions;
    using NUnit.Framework;

    public class AvailableCourseViewModelTests
    {
        [TestCase(0, DelegateStatus.NotEnrolled)]
        [TestCase(1, DelegateStatus.Expired)]
        [TestCase(2, DelegateStatus.Completed)]
        [TestCase(3, DelegateStatus.Current)]
        [TestCase(4, DelegateStatus.Removed)]
        public void Available_course_should_map_delegate_status(
            int delegateStatus,
            DelegateStatus expectedMappedDelegateStatus
        )
        {
            // Given
            var availableCourse = AvailableCourseHelper.CreateDefaultAvailableCourse(delegateStatus: delegateStatus);

            // When
            var availableCourseViewModel = new AvailableCourseViewModel(availableCourse);

            // Then
            availableCourseViewModel.DelegateStatus.Should().Be(expectedMappedDelegateStatus);
        }

        [TestCase(0, "Enrol")]
        [TestCase(1, "Re-enrol")]
        [TestCase(2, null)]
        [TestCase(3, null)]
        [TestCase(4, "Enrol")]
        public void Available_course_should_set_enrol_button_text(
            int delegateStatus,
            string? expectedEnrolButtonText
        )
        {
            // Given
            var availableCourse = AvailableCourseHelper.CreateDefaultAvailableCourse(delegateStatus: delegateStatus);

            // When
            var availableCourseViewModel = new AvailableCourseViewModel(availableCourse);

            // Then
            availableCourseViewModel.EnrolButtonText.Should().Be(expectedEnrolButtonText);
        }

        [TestCase(0, "Enrol on activity")]
        [TestCase(1, "Re-enrol on activity")]
        [TestCase(2, null)]
        [TestCase(3, null)]
        [TestCase(4, "Enrol on activity")]
        public void Available_course_should_set_enrol_button_aria_label(
            int delegateStatus,
            string? expectedEnrolButtonAriaLabel
        )
        {
            // Given
            var availableCourse = AvailableCourseHelper.CreateDefaultAvailableCourse(delegateStatus: delegateStatus);

            // When
            var availableCourseViewModel = new AvailableCourseViewModel(availableCourse);

            // Then
            availableCourseViewModel.EnrolButtonAriaLabel.Should().Be(expectedEnrolButtonAriaLabel);
        }
    }
}
