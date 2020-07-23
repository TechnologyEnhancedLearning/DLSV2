namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using FluentAssertions;
    using NUnit.Framework;

    public class CurrentViewModelTests
    {
        private CurrentViewModel model;

        [SetUp]
        public void SetUp()
        {
            var currentCourses = new[] {
                new CurrentCourse {
                    CustomisationID = 1,
                    CourseName = "Course 1",
                    HasDiagnostic = true,
                    HasLearning = true,
                    IsAssessed = true,
                    StartedDate = new DateTime(2010, 1, 22),
                    LastAccessed = new DateTime(2010, 2, 22),
                    CompleteByDate = new DateTime(2010, 3, 22),
                    DiagnosticScore = 123,
                    Passes = 4,
                    Sections = 6,
                    SupervisorAdminId = 0,
                    GroupCustomisationId = 0,
                },
                new CurrentCourse {
                    CustomisationID = 2,
                    CourseName = "Course 2",
                    HasDiagnostic = false,
                    HasLearning = false,
                    IsAssessed = false,
                    StartedDate = new DateTime(2011, 1, 22),
                    LastAccessed = new DateTime(2011, 2, 22),
                    CompleteByDate = new DateTime(2011, 3, 22),
                    DiagnosticScore = 456,
                    Passes = 14,
                    Sections = 16,
                    SupervisorAdminId = 12,
                    GroupCustomisationId = 34,
                },
            };

            model = new CurrentViewModel(currentCourses);
        }

        [TestCase(
            0,
            1,
            "Course 1",
            true,
            true,
            true,
            "2010-1-22",
            "2010-2-22",
            "2010-3-22",
            123,
            4,
            6,
            false,
            false
           )]
        [TestCase(
            1,
            2,
            "Course 2",
            false,
            false,
            false,
            "2011-1-22",
            "2011-2-22",
            "2011-3-22",
            456,
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
            DateTime expectedCompleteBy,
            int expectedDiagnosticScore,
            int expectedPasses,
            int expectedSections,
            bool expectedIsSupervisor,
            bool expectedIsGroup )
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
        }
    }
}
