namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.CourseSetup
{
    using System;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    internal class CourseSummaryViewModelTests
    {
        [Test]
        public void Date_fields_should_display_in_day_month_year_order()
        {
            // Given
            var testDateTime = new DateTime(2012, 12, 21, 22, 22, 22);
            var courseDetails = CourseDetailsTestHelper.GetDefaultCourseDetails(
                createdDate: testDateTime,
                lastAccessed: testDateTime
            );

            // When
            var viewModel = new CourseSummaryViewModel(courseDetails);

            // Then
            using (new AssertionScope())
            {
                viewModel.CreatedDate.Should().Be("21/12/2012");
                viewModel.LastAccessed.Should().Be("21/12/2012");
            }
        }
    }
}
