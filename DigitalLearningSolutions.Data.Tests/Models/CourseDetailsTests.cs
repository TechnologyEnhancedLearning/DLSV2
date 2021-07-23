namespace DigitalLearningSolutions.Data.Tests.Models
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using FluentAssertions;
    using NUnit.Framework;

    public class CourseDetailsTests
    {
        [Test]
        public void RefreshedToCourseName_should_be_Same_Course_if_RefreshToId_is_Zero()
        {
            // When
            var courseDetails = new CourseDetails
            {
                ApplicationName = "Original course",
                CustomisationName = "name",
                RefreshToCustomisationId = 0
            };

            // Then
            courseDetails.RefreshToCourseName.Should().BeEquivalentTo("Same course");
        }

        [Test]
        public void RefreshedToCourseName_should_be_CourseName_if_RefreshToId_is_CustomisationId()
        {
            // When
            var courseDetails = new CourseDetails
            {
                ApplicationName = "Original course",
                CustomisationName = "name",
                CustomisationId = 5,
                RefreshToCustomisationId = 5
            };

            // Then
            courseDetails.RefreshToCourseName.Should().BeEquivalentTo("Same course");
        }

        
        [Test]
        public void RefreshedToCourseName_should_be_from_refresh_course_if_RefreshToId_is_not_zero_or_CustomisationId()
        {
            // When
            var courseDetails = new CourseDetails
            {
                ApplicationName = "Original course",
                CustomisationName = "name",
                CustomisationId = 5,
                RefreshToCustomisationId = 10,
                RefreshToApplicationName = "Refreshed course",
                RefreshToCustomisationName = "refreshing"
            };

            // Then
            courseDetails.RefreshToCourseName.Should().BeEquivalentTo("Refreshed course - refreshing");
        }
    }
}
