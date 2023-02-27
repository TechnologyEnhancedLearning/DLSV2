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

        [Test]
        public void RefreshedToCourseName_should_be_from_refresh_course_application_if_RefreshToId_is_not_zero_or_CustomisationId_and_customisation_name_is_null()
        {
            // When
            var courseDetails = new CourseDetails
            {
                ApplicationName = "Original course",
                CustomisationName = "name",
                CustomisationId = 5,
                RefreshToCustomisationId = 10,
                RefreshToApplicationName = "Refreshed course",
                RefreshToCustomisationName = null
            };

            // Then
            courseDetails.RefreshToCourseName.Should().BeEquivalentTo("Refreshed course");
        }

        [Test]
        public void CourseName_should_be_ApplicationName_and_CustomisationName_if_CustomisationName_not_null()
        {
            // When
            var courseDetails = new CourseDetails
            {
                ApplicationName = "Original course",
                CustomisationName = "name",
            };

            // Then
            courseDetails.CourseName.Should().BeEquivalentTo("Original course - name");
        }


        [Test]
        public void CourseName_should_be_ApplicationName_if_CustomisationName_is_blank()
        {
            // When
            var courseDetails = new CourseDetails
            {
                ApplicationName = "Original course",
                CustomisationName = string.Empty
            };

            // Then
            courseDetails.CourseName.Should().BeEquivalentTo("Original course");
        }

        [Test]
        public void InProgressCount_should_be_TotalDelegates_minus_CompletedCount()
        {
            // When
            var courseDetails = new CourseDetails
            {
                DelegateCount = 123,
                CompletedCount = 81
            };

            // Then
            courseDetails.InProgressCount.Should().Be(42);
        }
    }

}
