namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class AvailableCourseViewModelTests
    {
        private IConfiguration config;

        [SetUp]
        public void SetUp()
        {
            config = A.Fake<IConfiguration>();
        }

        [TestCase("Valid category", "Valid category")]
        [TestCase("Undefined", null)]
        [TestCase("undefined", null)]
        public void Available_course_should_validate_category(
            string category,
            string? expectedValidatedCategory
        )
        {
            // Given
            var availableCourse = AvailableCourseHelper.CreateDefaultAvailableCourse(category: category);

            // When
            var availableCourseViewModel = new AvailableCourseViewModel(availableCourse, config);

            // Then
            availableCourseViewModel.Category.Should().Be(expectedValidatedCategory);
        }

        [TestCase("Valid topic", "Valid topic")]
        [TestCase("Undefined", null)]
        [TestCase("undefined", null)]
        public void Available_course_should_validate_topic(
            string topic,
            string? expectedValidatedTopic
        )
        {
            // Given
            var availableCourse = AvailableCourseHelper.CreateDefaultAvailableCourse(topic: topic);

            // When
            var availableCourseViewModel = new AvailableCourseViewModel(availableCourse, config);

            // Then
            availableCourseViewModel.Topic.Should().Be(expectedValidatedTopic);
        }

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
            var availableCourseViewModel = new AvailableCourseViewModel(availableCourse, config);

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
            var availableCourseViewModel = new AvailableCourseViewModel(availableCourse, config);

            // Then
            availableCourseViewModel.EnrolButtonText.Should().Be(expectedEnrolButtonText);
        }

        [TestCase(0, "Enrol on course")]
        [TestCase(1, "Re-enrol on course")]
        [TestCase(2, null)]
        [TestCase(3, null)]
        [TestCase(4, "Enrol on course")]
        public void Available_course_should_set_enrol_button_aria_label(
            int delegateStatus,
            string? expectedEnrolButtonAriaLabel
        )
        {
            // Given
            var availableCourse = AvailableCourseHelper.CreateDefaultAvailableCourse(delegateStatus: delegateStatus);

            // When
            var availableCourseViewModel = new AvailableCourseViewModel(availableCourse, config);

            // Then
            availableCourseViewModel.EnrolButtonAriaLabel.Should().Be(expectedEnrolButtonAriaLabel);
        }
    }
}
