namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class SortingHelperTests
    {
        private CurrentCourse[] currentCourses;
        private CompletedCourse[] completedCourses;
        private AvailableCourse[] availableCourses;

        [SetUp]
        public void SetUp()
        {
            var config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns("http://www.dls.nhs.uk");

            currentCourses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(
                    customisationId: 71,
                    courseName: "B: Course",
                    hasDiagnostic: true,
                    isAssessed: true,
                    startedDate: new DateTime(2010, 1, 31),
                    lastAccessed: new DateTime(2010, 2, 22),
                    completeByDate: new DateTime(2010, 3, 22),
                    passes: 4,
                    diagnosticScore: 123
                ),
                CurrentCourseHelper.CreateDefaultCurrentCourse(
                    customisationId: 72,
                    courseName: "C: Course",
                    hasDiagnostic: true,
                    isAssessed: false,
                    startedDate: new DateTime(2010, 2, 1),
                    lastAccessed: new DateTime(2011, 2, 22),
                    completeByDate: new DateTime(2011, 3, 22),
                    passes: 14,
                    diagnosticScore: 0
                ),
                CurrentCourseHelper.CreateDefaultCurrentCourse(
                    customisationId: 73,
                    courseName: "A: Course",
                    hasDiagnostic: false,
                    isAssessed: true,
                    startedDate: new DateTime(2001, 1, 22),
                    lastAccessed: new DateTime(2011, 2, 23),
                    completeByDate: null,
                    passes: 0,
                    diagnosticScore: 0
                )
            };

            completedCourses = new[]
            {
                CompletedCourseHelper.CreateDefaultCompletedCourse(
                    customisationId: 71,
                    courseName: "B: Course",
                    hasDiagnostic: true,
                    isAssessed: true,
                    startedDate: new DateTime(2010, 1, 31),
                    lastAccessed: new DateTime(2010, 2, 22),
                    completed: new DateTime(2010, 3, 22),
                    evaluated: new DateTime(2010, 5, 5),
                    diagnosticScore: 123,
                    passes: 4
                ),
                CompletedCourseHelper.CreateDefaultCompletedCourse(
                    customisationId: 72,
                    courseName: "C: Course",
                    hasDiagnostic: true,
                    isAssessed: false,
                    startedDate: new DateTime(2010, 2, 1),
                    lastAccessed: new DateTime(2011, 2, 22),
                    completed: new DateTime(2011, 3, 22),
                    evaluated: new DateTime(2008, 5, 5),
                    diagnosticScore: 0,
                    passes: 14
                ),
                CompletedCourseHelper.CreateDefaultCompletedCourse(
                    customisationId: 73,
                    courseName: "A: Course",
                    hasDiagnostic: false,
                    isAssessed: true,
                    startedDate: new DateTime(2001, 1, 22),
                    lastAccessed: new DateTime(2011, 2, 23),
                    completed: new DateTime(2011, 2, 23),
                    evaluated: new DateTime(2009, 5, 5),
                    diagnosticScore: 0,
                    passes: 0
                )
            };

            availableCourses = new[]
            {
                AvailableCourseHelper.CreateDefaultAvailableCourse(
                    customisationId: 71,
                    courseName: "A: Course",
                    brand: "B: Brand",
                    category: "C: Category",
                    topic: "B: Topic"
                ),
                AvailableCourseHelper.CreateDefaultAvailableCourse(
                    customisationId: 72,
                    courseName: "B: Course",
                    brand: "A: Brand",
                    category: null,
                    topic: "A: Topic"
                ),
                AvailableCourseHelper.CreateDefaultAvailableCourse(
                    customisationId: 73,
                    courseName: "C: Course",
                    brand: "C: Brand",
                    category: "A: Category",
                    topic: null
                )
            };
        }

        [TestCase("Course Name", "Ascending", new[] { 73, 71, 72 })]
        [TestCase("Course Name", "Descending", new[] { 72, 71, 73 })]
        [TestCase("Diagnostic Score", "Ascending", new[] { 73, 72, 71 })]
        [TestCase("Diagnostic Score", "Descending", new[] { 71, 72, 73 })]
        [TestCase("Passed Sections", "Ascending", new[] { 72, 73, 71 })]
        [TestCase("Passed Sections", "Descending", new[] { 71, 73, 72 })]
        [TestCase("Enrolled Date", "Ascending", new[] { 73, 71, 72 })]
        [TestCase("Enrolled Date", "Descending", new[] { 72, 71, 73 })]
        [TestCase("Last Accessed Date", "Ascending", new[] { 71, 72, 73 })]
        [TestCase("Last Accessed Date", "Descending", new[] { 73, 72, 71 })]
        [TestCase("Complete By Date", "Ascending", new[] { 73, 71, 72 })]
        [TestCase("Complete By Date", "Descending", new[] { 72, 71, 73 })]
        public void SortAllItems_should_sort_current_courses_correctly(
            string sortBy,
            string sortDirection,
            int[] expectedIdsOrder)
        {
            var sortedCurrentCourses = SortingHelper.SortAllItems(currentCourses, sortBy, sortDirection);
            var sortedIds = sortedCurrentCourses.Select(course => course.Id);
            sortedIds.Should().Equal(expectedIdsOrder);
        }

        [TestCase("Course Name", "Ascending", new[] { 73, 71, 72 })]
        [TestCase("Course Name", "Descending", new[] { 72, 71, 73 })]
        [TestCase("Enrolled Date", "Ascending", new[] { 73, 71, 72 })]
        [TestCase("Enrolled Date", "Descending", new[] { 72, 71, 73 })]
        [TestCase("Last Accessed Date", "Ascending", new[] { 71, 72, 73 })]
        [TestCase("Last Accessed Date", "Descending", new[] { 73, 72, 71 })]
        [TestCase("Completed Date", "Ascending", new[] { 71, 73, 72 })]
        [TestCase("Completed Date", "Descending", new[] { 72, 73, 71 })]
        public void SortAllItems_should_sort_completed_courses_correctly(
            string sortBy,
            string sortDirection,
            int[] expectedIdsOrder)
        {
            var sortedCompletedCourses = SortingHelper.SortAllItems(completedCourses, sortBy, sortDirection);
            var sortedIds = sortedCompletedCourses.Select(course => course.Id);
            sortedIds.Should().Equal(expectedIdsOrder);
        }

        [TestCase("Course Name", "Ascending", new[] { 71, 72, 73 })]
        [TestCase("Course Name", "Descending", new[] { 73, 72, 71 })]
        [TestCase("Brand", "Ascending", new[] { 72, 71, 73 })]
        [TestCase("Brand", "Descending", new[] { 73, 71, 72 })]
        [TestCase("Category", "Ascending", new[] { 72, 73, 71 })]
        [TestCase("Category", "Descending", new[] { 71, 73, 72 })]
        [TestCase("Topic", "Ascending", new[] { 73, 72, 71 })]
        [TestCase("Topic", "Descending", new[] { 71, 72, 73 })]
        public void SortAllItems_should_sort_available_courses_correctly(
            string sortBy,
            string sortDirection,
            int[] expectedIdsOrder)
        {
            var sortedCompletedCourses = SortingHelper.SortAllItems(availableCourses, sortBy, sortDirection);
            var sortedIds = sortedCompletedCourses.Select(course => course.Id);
            sortedIds.Should().Equal(expectedIdsOrder);
        }
    }
}
