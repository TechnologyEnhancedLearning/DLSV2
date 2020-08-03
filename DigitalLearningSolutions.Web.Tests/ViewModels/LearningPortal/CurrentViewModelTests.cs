namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class CurrentViewModelTests
    {
        private readonly IConfiguration config = A.Fake<IConfiguration>();
        private CurrentViewModel model;
        private CurrentCourse[] currentCourses;

        [SetUp]
        public void SetUp()
        {
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns("http://www.dls.nhs.uk");

            currentCourses = new[] {
                new CurrentCourse {
                    CustomisationID = 71,
                    CourseName = "B: Course",
                    HasDiagnostic = true,
                    HasLearning = true,
                    IsAssessed = false,
                    StartedDate = new DateTime(2010, 1, 31),
                    LastAccessed = new DateTime(2010, 2, 22),
                    CompleteByDate = new DateTime(2010, 3, 22),
                    DiagnosticScore = 123,
                    Passes = 4,
                    Sections = 6,
                    SupervisorAdminId = 0,
                    GroupCustomisationId = 0,
                },
                new CurrentCourse {
                    CustomisationID = 72,
                    CourseName = "C: Course",
                    HasDiagnostic = false,
                    HasLearning = false,
                    IsAssessed = false,
                    StartedDate = new DateTime(2010, 2, 1),
                    LastAccessed = new DateTime(2011, 2, 22),
                    CompleteByDate = new DateTime(2011, 3, 22),
                    DiagnosticScore = 456,
                    Passes = 14,
                    Sections = 16,
                    SupervisorAdminId = 12,
                    GroupCustomisationId = 34,
                },
                new CurrentCourse {
                    CustomisationID = 73,
                    CourseName = "A: Course",
                    HasDiagnostic = false,
                    HasLearning = true,
                    IsAssessed = false,
                    StartedDate = new DateTime(2001, 1, 22),
                    LastAccessed = new DateTime(2011, 2, 23),
                    CompleteByDate = null,
                    DiagnosticScore = 0,
                    Passes = 0,
                    Sections = 0,
                    SupervisorAdminId = 0,
                    GroupCustomisationId = 0,
                },
            };

            model = new CurrentViewModel(currentCourses, config, "Course Name", "Ascending");
        }

        [TestCase(
            0,
            73,
            "A: Course",
            false,
            true,
            false,
            "2001-1-22",
            "2011-2-23",
            null,
            0,
            0,
            0,
            false,
            false,
            "http://www.dls.nhs.uk/tracking/learn?CustomisationID=73&lp=1"
        )]
        [TestCase(
            1,
            71,
            "B: Course",
            true,
            true,
            false,
            "2010-1-31",
            "2010-2-22",
            "2010-3-22",
            123,
            4,
            6,
            false,
            false,
            "http://www.dls.nhs.uk/tracking/learn?CustomisationID=71&lp=1"
           )]
        [TestCase(
            2,
            72,
            "C: Course",
            false,
            false,
            false,
            "2010-2-1",
            "2011-2-22",
            "2011-3-22",
            456,
            14,
            16,
            true,
            true,
            "http://www.dls.nhs.uk/tracking/learn?CustomisationID=72&lp=1"
        )]
        public void Current_courses_should_map_to_view_models_in_the_correct_order(
            int index,
            int expectedId,
            string expectedName,
            bool expectedDiagnostic,
            bool expectedLearning,
            bool expectedAssessmentAndCertification,
            DateTime expectedStart,
            DateTime expectedLastAccessed,
            DateTime? expectedCompleteBy,
            int expectedDiagnosticScore,
            int expectedPasses,
            int expectedSections,
            bool expectedIsSupervisor,
            bool expectedIsGroup,
            string expectedLaunchUrl)
        {
            var course = model.CurrentCourses.ElementAt(index);
            course.Id.Should().Be(expectedId);
            course.Name.Should().Be(expectedName);
            course.HasDiagnosticAssessment.Should().Be(expectedDiagnostic);
            course.HasLearningContent.Should().Be(expectedLearning);
            course.HasLearningAssessmentAndCertification.Should().Be(expectedAssessmentAndCertification);
            course.StartedDate.Should().Be(expectedStart);
            course.LastAccessedDate.Should().Be(expectedLastAccessed);
            course.CompleteByDate.Should().Be(expectedCompleteBy);
            course.DiagnosticScore.Should().Be(expectedDiagnosticScore);
            course.PassedSections.Should().Be(expectedPasses);
            course.Sections.Should().Be(expectedSections);
            course.UserIsSupervisor.Should().Be(expectedIsSupervisor);
            course.IsEnrolledWithGroup.Should().Be(expectedIsGroup);
            course.LaunchUrl.Should().Be(expectedLaunchUrl);
        }

        [TestCase("Course Name", "Ascending", new int[] { 73, 71, 72 })]
        [TestCase("Course Name", "Descending", new int[] { 72, 71, 73 })]
        [TestCase("Has Diagnostic Assessment", "Ascending", new int[] { 72, 73, 71 })]
        [TestCase("Has Diagnostic Assessment", "Descending", new int[] { 71, 72, 73 })]
        [TestCase("Has Learning Assessment", "Ascending", new int[] { 72, 71, 73 })]
        [TestCase("Has Learning Assessment", "Descending", new int[] { 71, 73, 72 })]
        [TestCase("Enrolled Date", "Ascending", new int[] { 73, 71, 72 })]
        [TestCase("Enrolled Date", "Descending", new int[] { 72, 71, 73 })]
        [TestCase("Last Accessed Date", "Ascending", new int[] { 71, 72, 73 })]
        [TestCase("Last Accessed Date", "Descending", new int[] { 73, 72, 71 })]
        [TestCase("Complete By Date", "Ascending", new int[] { 73, 71, 72 })]
        [TestCase("Complete By Date", "Descending", new int[] { 72, 71, 73 })]
        public void Current_courses_should_be_sorted_correctly(
            string sortBy,
            string sortDirection,
            int[] expectedIdsOrder)
        {
            var sortedModel = new CurrentViewModel(currentCourses, config, sortBy, sortDirection);
            var sortedIds = sortedModel.CurrentCourses.Select(course => course.Id);
            sortedIds.Should().Equal(expectedIdsOrder);
        }
    }
}
