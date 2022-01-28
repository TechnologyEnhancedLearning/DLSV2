namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class CompletedPageViewModelTests
    {
        private readonly IConfiguration config = A.Fake<IConfiguration>();
        private CompletedCourse[] completedCourses = null!;
        private CompletedActionPlanResource[] completedResources = null!;
        private CompletedPageViewModel model = null!;

        [SetUp]
        public void SetUp()
        {
            completedCourses = new[]
            {
                new CompletedCourse
                {
                    Id = 71,
                    Name = "B: Course",
                    HasDiagnostic = true,
                    HasLearning = true,
                    IsAssessed = true,
                    StartedDate = new DateTime(2010, 1, 31),
                    Completed = new DateTime(2010, 2, 22),
                    Evaluated = new DateTime(2010, 3, 22),
                    ArchivedDate = null,
                    DiagnosticScore = 123,
                    Passes = 4,
                    Sections = 6,
                },
                new CompletedCourse
                {
                    Id = 72,
                    Name = "C: Course",
                    HasDiagnostic = true,
                    HasLearning = true,
                    IsAssessed = false,
                    StartedDate = new DateTime(2010, 2, 1),
                    Completed = new DateTime(2011, 2, 22),
                    Evaluated = new DateTime(2011, 3, 22),
                    ArchivedDate = null,
                    DiagnosticScore = 0,
                    Passes = 14,
                    Sections = 16,
                },
                new CompletedCourse
                {
                    Id = 73,
                    Name = "A: Course",
                    HasDiagnostic = false,
                    HasLearning = true,
                    IsAssessed = true,
                    StartedDate = new DateTime(2001, 1, 22),
                    Completed = new DateTime(2011, 2, 23),
                    Evaluated = new DateTime(2011, 2, 23),
                    ArchivedDate = null,
                    DiagnosticScore = 0,
                    Passes = 0,
                    Sections = 6,
                },
            };
            completedResources = Builder<CompletedActionPlanResource>.CreateListOfSize(2).Build().ToArray();

            model = new CompletedPageViewModel(
                completedCourses,
                completedResources,
                false,
                false,
                config,
                null,
                "Name",
                "Ascending",
                null,
                1
            );
        }

        [TestCase(
            1,
            71,
            "B: Course",
            true,
            true,
            true,
            "2010-1-31",
            "2010-2-22",
            "2010-3-22",
            123,
            4,
            6
        )]
        [TestCase(
            2,
            72,
            "C: Course",
            true,
            true,
            false,
            "2010-2-1",
            "2011-2-22",
            "2011-3-22",
            0,
            14,
            16
        )]
        [TestCase(
            0,
            73,
            "A: Course",
            false,
            true,
            true,
            "2001-1-22",
            "2011-2-23",
            "2011-2-23",
            0,
            0,
            06
        )]
        public void Completed_courses_should_map_to_view_models_in_the_correct_order(
            int index,
            int expectedId,
            string expectedName,
            bool expectedDiagnostic,
            bool expectedLearning,
            bool expectedAssessmentAndCertification,
            DateTime expectedStart,
            DateTime expectedCompleted,
            DateTime expectedEvaluated,
            int expectedDiagnosticScore,
            int expectedPasses,
            int expectedSections
        )
        {
            var course = model.CompletedActivities.ElementAt(index);
            course.Id.Should().Be(expectedId);
            course.Name.Should().Be(expectedName);
            course.HasDiagnosticAssessment.Should().Be(expectedDiagnostic);
            course.HasLearningContent.Should().Be(expectedLearning);
            course.HasLearningAssessmentAndCertification.Should().Be(expectedAssessmentAndCertification);
            course.StartedDate.Should().Be(expectedStart);
            course.CompletedDate.Should().Be(expectedCompleted);
            course.EvaluatedDate.Should().Be(expectedEvaluated);
            course.DiagnosticScore.Should().Be(expectedDiagnosticScore);
            course.PassedSections.Should().Be(expectedPasses);
            course.Sections.Should().Be(expectedSections);
        }

        [Test]
        public void Completed_courses_should_default_to_returning_the_first_ten_courses()
        {
            var courses = new[]
            {
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "a course 1"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "b course 2"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "c course 3"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "d course 4"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "e course 5"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "f course 6"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "g course 7"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "h course 8"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "i course 9"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "j course 10"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "k course 11"),
            };

            model = new CompletedPageViewModel(
                courses,
                completedResources,
                false,
                false,
                config,
                null,
                "Name",
                "Ascending",
                null,
                1
            );

            model.CompletedActivities.Count().Should().Be(10);
            model.CompletedActivities.FirstOrDefault(course => course.Name == "k course 11").Should().BeNull();
        }

        [Test]
        public void Completed_courses_should_correctly_return_the_second_page_of_courses()
        {
            var courses = new[]
            {
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "a course 1"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "b course 2"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "c course 3"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "d course 4"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "e course 5"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "f course 6"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "g course 7"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "h course 8"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "i course 9"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "j course 10"),
                CompletedCourseHelper.CreateDefaultCompletedCourse(courseName: "k course 11"),
            };

            model = new CompletedPageViewModel(
                courses,
                completedResources,
                false,
                false,
                config,
                null,
                "Name",
                "Ascending",
                null,
                2
            );

            model.CompletedActivities.Count().Should().Be(3);
            model.CompletedActivities.First().Name.Should().Be("k course 11");
        }
    }
}
