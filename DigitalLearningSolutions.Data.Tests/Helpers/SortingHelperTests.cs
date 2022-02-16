namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class SortingHelperTests
    {
        private IQueryable<AvailableCourse> availableCourses = null!;
        private IQueryable<CompletedCourse> completedCourses = null!;
        private IQueryable<CurrentCourse> currentCourses = null!;

        [SetUp]
        public void SetUp()
        {
            var config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns("http://www.dls.nhs.uk");

            currentCourses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(
                    71,
                    "B: Course",
                    true,
                    isAssessed: true,
                    startedDate: new DateTime(2010, 1, 31),
                    lastAccessed: new DateTime(2010, 2, 22),
                    completeByDate: new DateTime(2010, 3, 22),
                    passes: 4,
                    diagnosticScore: 123
                ),
                CurrentCourseHelper.CreateDefaultCurrentCourse(
                    72,
                    "C: Course",
                    true,
                    isAssessed: false,
                    startedDate: new DateTime(2010, 2, 1),
                    lastAccessed: new DateTime(2011, 2, 22),
                    completeByDate: new DateTime(2011, 3, 22),
                    passes: 14,
                    diagnosticScore: 0
                ),
                CurrentCourseHelper.CreateDefaultCurrentCourse(
                    73,
                    "A: Course",
                    false,
                    isAssessed: true,
                    startedDate: new DateTime(2001, 1, 22),
                    lastAccessed: new DateTime(2011, 2, 23),
                    completeByDate: null,
                    passes: 0,
                    diagnosticScore: 0
                ),
            }.AsQueryable();

            completedCourses = new[]
            {
                CompletedCourseHelper.CreateDefaultCompletedCourse(
                    71,
                    "B: Course",
                    true,
                    isAssessed: true,
                    startedDate: new DateTime(2010, 1, 31),
                    lastAccessed: new DateTime(2010, 2, 22),
                    completed: new DateTime(2010, 3, 22),
                    evaluated: new DateTime(2010, 5, 5),
                    diagnosticScore: 123,
                    passes: 4
                ),
                CompletedCourseHelper.CreateDefaultCompletedCourse(
                    72,
                    "C: Course",
                    true,
                    isAssessed: false,
                    startedDate: new DateTime(2010, 2, 1),
                    lastAccessed: new DateTime(2011, 2, 22),
                    completed: new DateTime(2011, 3, 22),
                    evaluated: new DateTime(2008, 5, 5),
                    diagnosticScore: 0,
                    passes: 14
                ),
                CompletedCourseHelper.CreateDefaultCompletedCourse(
                    73,
                    "A: Course",
                    false,
                    isAssessed: true,
                    startedDate: new DateTime(2001, 1, 22),
                    lastAccessed: new DateTime(2011, 2, 23),
                    completed: new DateTime(2011, 2, 23),
                    evaluated: new DateTime(2009, 5, 5),
                    diagnosticScore: 0,
                    passes: 0
                ),
            }.AsQueryable();

            availableCourses = new[]
            {
                AvailableCourseHelper.CreateDefaultAvailableCourse(
                    71,
                    "A: Course",
                    brand: "B: Brand",
                    category: "C: Category",
                    topic: "B: Topic"
                ),
                AvailableCourseHelper.CreateDefaultAvailableCourse(
                    72,
                    "B: Course",
                    brand: "A: Brand",
                    category: null,
                    topic: "A: Topic"
                ),
                AvailableCourseHelper.CreateDefaultAvailableCourse(
                    73,
                    "C: Course",
                    brand: "C: Brand",
                    category: "A: Category",
                    topic: null
                ),
            }.AsQueryable();
        }

        [TestCase("Name", "Ascending", new[] { 73, 71, 72 })]
        [TestCase("Name", "Descending", new[] { 72, 71, 73 })]
        [TestCase("HasDiagnostic,DiagnosticScore", "Ascending", new[] { 73, 72, 71 })]
        [TestCase("HasDiagnostic,DiagnosticScore", "Descending", new[] { 71, 72, 73 })]
        [TestCase("IsAssessed,Passes", "Ascending", new[] { 72, 73, 71 })]
        [TestCase("IsAssessed,Passes", "Descending", new[] { 71, 73, 72 })]
        [TestCase("StartedDate", "Ascending", new[] { 73, 71, 72 })]
        [TestCase("StartedDate", "Descending", new[] { 72, 71, 73 })]
        [TestCase("LastAccessed", "Ascending", new[] { 71, 72, 73 })]
        [TestCase("LastAccessed", "Descending", new[] { 73, 72, 71 })]
        [TestCase("CompleteByDate", "Ascending", new[] { 73, 71, 72 })]
        [TestCase("CompleteByDate", "Descending", new[] { 72, 71, 73 })]
        public void SortAllItems_should_sort_current_courses_correctly(
            string sortBy,
            string sortDirection,
            int[] expectedIdsOrder
        )
        {
            var sortedCurrentCourses = GenericSortingHelper.SortAllItems(currentCourses, sortBy, sortDirection);
            var sortedIds = sortedCurrentCourses.Select(course => course.Id);
            sortedIds.Should().Equal(expectedIdsOrder);
        }

        [TestCase("Name", "Ascending", new[] { 73, 71, 72 })]
        [TestCase("Name", "Descending", new[] { 72, 71, 73 })]
        [TestCase("StartedDate", "Ascending", new[] { 73, 71, 72 })]
        [TestCase("StartedDate", "Descending", new[] { 72, 71, 73 })]
        [TestCase("LastAccessed", "Ascending", new[] { 71, 72, 73 })]
        [TestCase("LastAccessed", "Descending", new[] { 73, 72, 71 })]
        [TestCase("Completed", "Ascending", new[] { 71, 73, 72 })]
        [TestCase("Completed", "Descending", new[] { 72, 73, 71 })]
        public void SortAllItems_should_sort_completed_courses_correctly(
            string sortBy,
            string sortDirection,
            int[] expectedIdsOrder
        )
        {
            var sortedCompletedCourses = GenericSortingHelper.SortAllItems(completedCourses, sortBy, sortDirection);
            var sortedIds = sortedCompletedCourses.Select(course => course.Id);
            sortedIds.Should().Equal(expectedIdsOrder);
        }

        [TestCase("Name", "Ascending", new[] { 71, 72, 73 })]
        [TestCase("Name", "Descending", new[] { 73, 72, 71 })]
        [TestCase("Brand", "Ascending", new[] { 72, 71, 73 })]
        [TestCase("Brand", "Descending", new[] { 73, 71, 72 })]
        [TestCase("Category", "Ascending", new[] { 72, 73, 71 })]
        [TestCase("Category", "Descending", new[] { 71, 73, 72 })]
        [TestCase("Topic", "Ascending", new[] { 73, 72, 71 })]
        [TestCase("Topic", "Descending", new[] { 71, 72, 73 })]
        public void SortAllItems_should_sort_available_courses_correctly(
            string sortBy,
            string sortDirection,
            int[] expectedIdsOrder
        )
        {
            var sortedCompletedCourses = GenericSortingHelper.SortAllItems(availableCourses, sortBy, sortDirection);
            var sortedIds = sortedCompletedCourses.Select(course => course.Id);
            sortedIds.Should().Equal(expectedIdsOrder);
        }
    }
}
