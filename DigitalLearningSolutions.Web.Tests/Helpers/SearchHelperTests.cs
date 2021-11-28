namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Linq;
    using ClosedXML;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;
    using SelfAssessmentHelper = DigitalLearningSolutions.Data.Tests.TestHelpers.SelfAssessmentHelper;

    public class GenericSearchHelperTests
    {
        private AvailableCourse[] availableCourses = null!;
        private CompletedCourse[] completedCourses = null!;
        private CurrentCourse[] currentCourses = null!;
        private CurrentLearningItem[] currentCoursesWithSelfAssessment = null!;

        [SetUp]
        public void SetUp()
        {
            currentCourses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(71, "b: course"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(72, "C: Course"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(73, "A: Course")
            };
            currentCoursesWithSelfAssessment = new CurrentLearningItem[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(71, "d: course"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(72, "C: Course"),
                SelfAssessmentHelper.CreateDefaultSelfAssessment(74, "a: self assessment"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(73, "A: Course")
            };
            completedCourses = new[]
            {
                CompletedCourseHelper.CreateDefaultCompletedCourse(71, "First course"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(72, "Course 20: the best course"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(73, "Last course 30105 and a lot of other text")
            };
            availableCourses = new[]
            {
                AvailableCourseHelper.CreateDefaultAvailableCourse(71, "One great course"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(72, "The course v1"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(73, "Course 1: some more title text")
            };
        }

        [TestCase("A:", new[] { 73 })]
        [TestCase(null, new[] { 71, 72, 73 })]
        [TestCase("course", new[] { 71, 72, 73 })]
        [TestCase("Course", new[] { 71, 72, 73 })]
        [TestCase("self", new int[] { })]
        public void Current_courses_should_be_filtered_correctly(
            string searchString,
            int[] expectedIds
        )
        {
            // When
            var result = GenericSearchHelper.SearchItems(currentCourses, searchString);
            var filteredIds = result.Select(course => course.Id);

            // Then
            filteredIds.Should().Equal(expectedIds);
        }

        [TestCase("A:", new[] { 74, 73 })]
        [TestCase(null, new[] { 71, 72, 74, 73 })]
        [TestCase("course", new[] { 71, 72, 73 })]
        [TestCase("Course", new[] { 71, 72, 73 })]
        [TestCase("self", new[] { 74 })]
        public void Current_courses_with_self_assessment_should_be_filtered_correctly(
            string searchString,
            int[] expectedIds
        )
        {
            // When
            var result = GenericSearchHelper.SearchItems(currentCoursesWithSelfAssessment, searchString);
            var filteredIds = result.Select(course => course.Id);

            // Then
            filteredIds.Should().Equal(expectedIds);
        }

        [TestCase(null, new[] { 71, 72, 73 })]
        [TestCase("2", new[] { 72 })]
        [TestCase("200", new int[] { })]
        [TestCase("40", new int[] { })]
        [TestCase("somecourse", new int[] { })]
        [TestCase("course 3010", new[] { 73 })]
        [TestCase("course 30105", new[] { 73 })]
        public void Completed_courses_should_be_filtered_correctly(
            string searchString,
            int[] expectedIds
        )
        {
            // When
            var result = GenericSearchHelper.SearchItems(completedCourses, searchString);
            var filteredIds = result.Select(course => course.Id);

            // Then
            filteredIds.Should().Equal(expectedIds);
        }

        [TestCase(null, new[] { 71, 72, 73 })]
        [TestCase("1", new[] { 72, 73 })]
        [TestCase("v1", new[] { 72 })]
        [TestCase("10", new int[] { })]
        [TestCase(":", new[] { 73 })]
        [TestCase("one great", new[] { 71 })]
        public void Available_courses_should_be_filtered_correctly(
            string searchString,
            int[] expectedIds
        )
        {
            // When
            var result = GenericSearchHelper.SearchItems(availableCourses, searchString);
            var filteredIds = result.Select(course => course.Id);

            // Then
            filteredIds.Should().Equal(expectedIds);
        }

        [Test]
        public void SearchItemsUsingTokeniseScorer_returns_expected_result_from_available_courses()
        {
            // Given
            var searchString = "title text";
            var expectedId = 73;

            // When
            var result = GenericSearchHelper.SearchItemsUsingTokeniseScorer(availableCourses, searchString);

            // Then
            result.Should().Contain(r => r.Id == expectedId);
        }
    }
}
