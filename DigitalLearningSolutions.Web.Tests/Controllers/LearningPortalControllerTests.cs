namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class LearningPortalControllerTests
    {
        private LearningPortalController controller;

        private ICourseService courseService;

        [SetUp]
        public void SetUp()
        {
            courseService = A.Fake<ICourseService>();
            var logger = A.Fake<ILogger<LearningPortalController>>();
            controller = new LearningPortalController(courseService, logger);
        }

        [Test]
        public void Current_action_should_return_view_result()
        {
            // Given
            const int candidateID = 1;
            var currentCourses = new[] {
                CurrentCourseHelper.CreateDefaultCurrentCourse(),
                CurrentCourseHelper.CreateDefaultCurrentCourse()
            };
            A.CallTo(() => courseService.GetCurrentCourses(candidateID)).Returns(currentCourses);

            // When
            var result = controller.Current();

            // Then
            var expectedModel = new CurrentViewModel(currentCourses);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Current_course_should_be_overdue_when_complete_by_date_is_in_the_past()
        {
            // Given
            const int candidateID = 1;
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(completeByDate: DateTime.Today - TimeSpan.FromDays(1));
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(candidateID)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);

            // Then
            course.Overdue().Should().BeTrue();
        }

        [Test]
        public void Current_course_should_not_be_overdue_when_complete_by_date_is_in_the_future()
        {
            // Given
            const int candidateID = 1;
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(completeByDate: DateTime.Today + TimeSpan.FromDays(1));
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(candidateID)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);


            // Then
            course.Overdue().Should().BeFalse();
        }

        [Test]
        public void Current_course_should_not_have_diagnostic_score_without_diagnostic_assessment()
        {
            // Given
            const int candidateID = 1;
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(hasDiagnostic: false);
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(candidateID)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);


            // Then
            course.HasDiagnosticScore().Should().BeFalse();
        }

        [Test]
        public void Current_course_should_not_have_diagnostic_score_without_diagnostic_score_value()
        {
            // Given
            const int candidateID = 1;
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(diagnosticScore: null);
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(candidateID)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);


            // Then
            course.HasDiagnosticScore().Should().BeFalse();
        }

        [Test]
        public void Current_course_should_have_diagnostic_score_with_diagnostic_score_value_and_diagnostic_assessment()
        {
            // Given
            const int candidateID = 1;
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse();
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(candidateID)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);


            // Then
            course.HasDiagnosticScore().Should().BeTrue();
        }

        [Test]
        public void Current_course_should_not_have_passed_sections_without_learning_assessment()
        {
            // Given
            const int candidateID = 1;
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(isAssessed: false);
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(candidateID)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);


            // Then
            course.HasPassedSections().Should().BeFalse();
        }

        [Test]
        public void Current_course_should_have_passed_sections_with_learning_assessment()
        {
            // Given
            const int candidateID = 1;
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse();
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(candidateID)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);


            // Then
            course.HasPassedSections().Should().BeTrue();
        }

        [Test]
        public void Completed_action_should_return_view_result()
        {
            // Given
            var completedCourses = new[] {
                new Course { Id = 1, Name = "Course 1" },
                new Course { Id = 2, Name = "Course 2" }
            };
            A.CallTo(() => courseService.GetCompletedCourses()).Returns(completedCourses);

            // When
            var result = controller.Completed();

            // Then
            var expectedModel = new CompletedViewModel(completedCourses);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Available_action_should_return_view_result()
        {
            // Given
            var availableCourses = new[] {
                new Course { Id = 1, Name = "Course 1" },
                new Course { Id = 2, Name = "Course 2" }
            };
            A.CallTo(() => courseService.GetAvailableCourses()).Returns(availableCourses);

            // When
            var result = controller.Available();

            // Then
            var expectedModel = new AvailableViewModel(availableCourses);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
