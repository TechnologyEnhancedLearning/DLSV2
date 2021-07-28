namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    internal class LearningPathwayDefaultsViewModelTests
    {
        [Test]
        public void Month_fields_should_display_singular_month_when_value_is_one_month()
        {
            // Given
            var courseDetails = CourseDetailsTestHelper.GetDefaultCourseDetails(
                autoRefreshMonths: 1,
                completeWithinMonths: 1,
                validityMonths: 1
            );

            // When
            var viewModel = new LearningPathwayDefaultsViewModel(courseDetails);

            // Then
            using (new AssertionScope())
            {
                viewModel.AutoRefreshMonths.Should().Be("1 month < expiry");
                viewModel.CompleteWithinMonths.Should().Be("1 month");
                viewModel.CompletionValidFor.Should().Be("1 month");
            }
        }

        [Test]
        public void Month_fields_should_display_plural_months_when_value_is_zero_months()
        {
            // Given
            var courseDetails = CourseDetailsTestHelper.GetDefaultCourseDetails(
                autoRefreshMonths: 0,
                completeWithinMonths: 0,
                validityMonths: 0
            );

            // When
            var viewModel = new LearningPathwayDefaultsViewModel(courseDetails);

            // Then
            using (new AssertionScope())
            {
                viewModel.AutoRefreshMonths.Should().Be("0 months < expiry");
                viewModel.CompleteWithinMonths.Should().Be("0 months");
                viewModel.CompletionValidFor.Should().Be("0 months");
            }
        }

        [Test]
        public void Month_fields_should_display_plural_months_when_value_is_multiple_months()
        {
            // Given
            var courseDetails = CourseDetailsTestHelper.GetDefaultCourseDetails(
                autoRefreshMonths: 2,
                completeWithinMonths: 2,
                validityMonths: 2
            );

            // When
            var viewModel = new LearningPathwayDefaultsViewModel(courseDetails);

            // Then
            using (new AssertionScope())
            {
                viewModel.AutoRefreshMonths.Should().Be("2 months < expiry");
                viewModel.CompleteWithinMonths.Should().Be("2 months");
                viewModel.CompletionValidFor.Should().Be("2 months");
            }
        }
    }
}
