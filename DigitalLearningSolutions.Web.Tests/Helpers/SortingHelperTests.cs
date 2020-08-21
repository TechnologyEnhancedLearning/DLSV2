namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class SortingHelperTests
    {
        private CurrentCourse[] currentCourses;
        private CompletedCourse[] completedCourses;

        [SetUp]
        public void SetUp()
        {
            var config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns("http://www.dls.nhs.uk");

            currentCourses = new[]
            {
                new CurrentCourse
                {
                    CustomisationID = 71,
                    CourseName = "B: Course",
                    HasDiagnostic = true,
                    HasLearning = true,
                    IsAssessed = true,
                    StartedDate = new DateTime(2010, 1, 31),
                    LastAccessed = new DateTime(2010, 2, 22),
                    CompleteByDate = new DateTime(2010, 3, 22),
                    DiagnosticScore = 123,
                    Passes = 4,
                    Sections = 6,
                    SupervisorAdminId = 0,
                    GroupCustomisationId = 0,
                },
                new CurrentCourse
                {
                    CustomisationID = 72,
                    CourseName = "C: Course",
                    HasDiagnostic = true,
                    HasLearning = true,
                    IsAssessed = false,
                    StartedDate = new DateTime(2010, 2, 1),
                    LastAccessed = new DateTime(2011, 2, 22),
                    CompleteByDate = new DateTime(2011, 3, 22),
                    DiagnosticScore = 0,
                    Passes = 14,
                    Sections = 16,
                    SupervisorAdminId = 12,
                    GroupCustomisationId = 34,
                },
                new CurrentCourse
                {
                    CustomisationID = 73,
                    CourseName = "A: Course",
                    HasDiagnostic = false,
                    HasLearning = true,
                    IsAssessed = true,
                    StartedDate = new DateTime(2001, 1, 22),
                    LastAccessed = new DateTime(2011, 2, 23),
                    CompleteByDate = null,
                    DiagnosticScore = 0,
                    Passes = 0,
                    Sections = 6,
                    SupervisorAdminId = 0,
                    GroupCustomisationId = 0,
                },
            };
            completedCourses = new[]
            {
                new CompletedCourse
                {
                    CustomisationID = 71,
                    CourseName = "B: Course",
                    HasDiagnostic = true,
                    HasLearning = true,
                    IsAssessed = true,
                    StartedDate = new DateTime(2010, 1, 31),
                    LastAccessed = new DateTime(2010, 2, 22),
                    Completed = new DateTime(2010, 3, 22),
                    Evaluated = new DateTime(2010, 5, 5),
                    DiagnosticScore = 123,
                    Passes = 4,
                    Sections = 6
                },
                new CompletedCourse
                {
                    CustomisationID = 72,
                    CourseName = "C: Course",
                    HasDiagnostic = true,
                    HasLearning = true,
                    IsAssessed = false,
                    StartedDate = new DateTime(2010, 2, 1),
                    LastAccessed = new DateTime(2011, 2, 22),
                    Completed = new DateTime(2011, 3, 22),
                    Evaluated = new DateTime(2008, 5, 5),
                    DiagnosticScore = 0,
                    Passes = 14,
                    Sections = 16
                },
                new CompletedCourse
                {
                    CustomisationID = 73,
                    CourseName = "A: Course",
                    HasDiagnostic = false,
                    HasLearning = true,
                    IsAssessed = true,
                    StartedDate = new DateTime(2001, 1, 22),
                    LastAccessed = new DateTime(2011, 2, 23),
                    Completed = new DateTime(2011, 2, 23),
                    Evaluated = new DateTime(2009, 5, 5),
                    DiagnosticScore = 0,
                    Passes = 0,
                    Sections = 6
                },
            };

        }

        [TestCase("Course Name", "Ascending", new int[] { 73, 71, 72 })]
        [TestCase("Course Name", "Descending", new int[] { 72, 71, 73 })]
        [TestCase("Diagnostic Score", "Ascending", new int[] { 73, 72, 71 })]
        [TestCase("Diagnostic Score", "Descending", new int[] { 71, 72, 73 })]
        [TestCase("Passed Sections", "Ascending", new int[] { 72, 73, 71 })]
        [TestCase("Passed Sections", "Descending", new int[] { 71, 73, 72 })]
        [TestCase("Enrolled Date", "Ascending", new int[] { 73, 71, 72 })]
        [TestCase("Enrolled Date", "Descending", new int[] { 72, 71, 73 })]
        [TestCase("Last Accessed Date", "Ascending", new int[] { 71, 72, 73 })]
        [TestCase("Last Accessed Date", "Descending", new int[] { 73, 72, 71 })]
        [TestCase("Complete By Date", "Ascending", new int[] { 73, 71, 72 })]
        [TestCase("Complete By Date", "Descending", new int[] { 72, 71, 73 })]
        public void SortAllItems_should_sort_current_courses_correctly(
            string sortBy,
            string sortDirection,
            int[] expectedIdsOrder)
        {
            var sortedCurrentCourses = SortingHelper.SortAllItems(currentCourses, null, sortBy, sortDirection);
            var sortedIds = sortedCurrentCourses.Select(course => (course as CurrentCourse).CustomisationID);
            sortedIds.Should().Equal(expectedIdsOrder);
        }

        [TestCase("Course Name", "Ascending", new int[] { 73, 71, 72 })]
        [TestCase("Course Name", "Descending", new int[] { 72, 71, 73 })]
        [TestCase("Enrolled Date", "Ascending", new int[] { 73, 71, 72 })]
        [TestCase("Enrolled Date", "Descending", new int[] { 72, 71, 73 })]
        [TestCase("Last Accessed Date", "Ascending", new int[] { 71, 72, 73 })]
        [TestCase("Last Accessed Date", "Descending", new int[] { 73, 72, 71 })]
        [TestCase("Completed Date", "Ascending", new int[] { 71, 73, 72 })]
        [TestCase("Completed Date", "Descending", new int[] { 72, 73, 71 })]
        public void SortAllItems_should_sort_completed_courses_correctly(
            string sortBy,
            string sortDirection,
            int[] expectedIdsOrder)
        {
            var sortedCompletedCourses = SortingHelper.SortAllItems(completedCourses, null, sortBy, sortDirection);
            var sortedIds = sortedCompletedCourses.Select(course => (course as CompletedCourse).CustomisationID);
            sortedIds.Should().Equal(expectedIdsOrder);
        }

    }
}
