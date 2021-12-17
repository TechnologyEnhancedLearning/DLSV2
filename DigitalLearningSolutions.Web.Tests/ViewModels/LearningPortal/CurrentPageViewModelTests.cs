namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using NUnit.Framework;

    public class CurrentPageViewModelTests
    {
        private CurrentPageViewModel model = null!;
        private CurrentCourse[] currentCourses = null!;
        private SelfAssessment[] selfAssessments = null!;
        private ActionPlanResource[] actionPlanResources = null!;

        [SetUp]
        public void SetUp()
        {
            currentCourses = new[]
            {
                new CurrentCourse
                {
                    Id = 71,
                    Name = "B: Course",
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
                    Id = 72,
                    Name = "C: Course",
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
                    Id = 73,
                    Name = "A: Course",
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
            selfAssessments = new[]
            {
                new SelfAssessment
                {
                    Id = 1,
                    Name = "Self Assessment 1",
                    Description = "Self Assessment 1 Description"
                },
                new SelfAssessment
                {
                    Id = 1,
                    Name = "Self Assessment 2",
                    Description = "Self Assessment 2 Description"
                },
            };
            actionPlanResources = Builder<ActionPlanResource>.CreateListOfSize(2).Build().ToArray();

            model = new CurrentPageViewModel(
                currentCourses,
                null,
                "Name",
                "Ascending",
                selfAssessments,
                actionPlanResources,
                null,
                1
            );
        }

        [TestCase(
            0,
            73,
            "A: Course",
            false,
            true,
            true,
            "2001-1-22",
            "2011-2-23",
            null,
            0,
            0,
            06,
            false,
            false
        )]
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
            6,
            false,
            false
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
            16,
            true,
            true
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
            bool expectedIsGroup)
        {
            var course = model.CurrentActivities.ElementAt(index) as CurrentCourseViewModel;
            course!.Id.Should().Be(expectedId);
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
        }

        [Test]
        public void Current_courses_should_default_to_returning_the_first_ten_courses()
        {
            var courses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "a course 1"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "b course 2"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "c course 3"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "d course 4"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "e course 5"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "f course 6"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "g course 7"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "h course 8"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "i course 9"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "j course 10"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "k course 11"),
            };
            selfAssessments = new SelfAssessment[]
            {
                SelfAssessmentHelper.CreateDefaultSelfAssessment(),
                SelfAssessmentHelper.CreateDefaultSelfAssessment(),
            };

            model = new CurrentPageViewModel(
                courses,
                null,
                "Name",
                "Ascending",
                selfAssessments,
                actionPlanResources,
                null,
                1
            );

            model.CurrentActivities.Count().Should().Be(10);
            model.CurrentActivities.FirstOrDefault(course => course.Name == "k course 11").Should().BeNull();
        }

        [Test]
        public void Current_courses_should_correctly_return_the_second_page_of_courses()
        {
            var courses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "a course 1"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "b course 2"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "c course 3"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "d course 4"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "e course 5"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "f course 6"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "g course 7"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "h course 8"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "i course 9"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "j course 10"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "k course 11"),
            };
            selfAssessments = new SelfAssessment[]
            {
                SelfAssessmentHelper.CreateDefaultSelfAssessment(),
                SelfAssessmentHelper.CreateDefaultSelfAssessment(),
            };
            actionPlanResources = Builder<ActionPlanResource>.CreateListOfSize(2).Build().ToArray();

            model = new CurrentPageViewModel(
                courses,
                null,
                "Name",
                "Ascending",
                selfAssessments,
                actionPlanResources,
                null,
                2
            );

            model.CurrentActivities.Count().Should().Be(5);
            model.CurrentActivities.First().Name.Should().Be("k course 11");
        }

        [Test]
        public void Current_courses_should_return_correct_number_of_search_results()
        {
            var courses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "a course 1"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "b course 2"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "c course 3"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "d course 4"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "e course 5"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "f course 6"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "g course 7"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "h course 8"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "i course 9"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "j course 10"),
                CurrentCourseHelper.CreateDefaultCurrentCourse(courseName: "k course 11"),
            };
            selfAssessments = new SelfAssessment[]
            {
                SelfAssessmentHelper.CreateDefaultSelfAssessment(),
                SelfAssessmentHelper.CreateDefaultSelfAssessment(),
            };
            model = new CurrentPageViewModel(
                courses,
                "Course",
                "Name",
                "Ascending",
                selfAssessments,
                actionPlanResources,
                null,
                1
            );

            model.MatchingSearchResults.Should().Be(11);
        }
    }
}
