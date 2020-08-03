namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System;
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class LearningPortalControllerTests
    {
        private LearningPortalController controller;
        private ICourseService courseService;
        private IUnlockService unlockService;
        private IConfiguration config;
        private const string BaseUrl = "https://www.dls.nhs.uk";
        private const int CandidateId = 254480;

        [SetUp]
        public void SetUp()
        {
            courseService = A.Fake<ICourseService>();
            unlockService = A.Fake<IUnlockService>();
            var logger = A.Fake<ILogger<LearningPortalController>>();
            config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("learnCandidateID", CandidateId.ToString()),
            }, "mock"));
            controller = new LearningPortalController(courseService, unlockService, logger, config)
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext { User = user } }
            };
        }

        [Test]
        public void Current_action_should_return_view_result()
        {
            // Given
            var currentCourses = new[] {
                CurrentCourseHelper.CreateDefaultCurrentCourse(),
                CurrentCourseHelper.CreateDefaultCurrentCourse()
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var result = controller.Current();

            // Then
            var expectedModel = new CurrentViewModel(currentCourses, config, "Course Name", "Ascending");
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Current_course_should_be_overdue_when_complete_by_date_is_in_the_past()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(completeByDate: DateTime.Today - TimeSpan.FromDays(1));
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);

            // Then
            course.Overdue().Should().BeTrue();
        }

        [Test]
        public void Current_course_should_not_be_overdue_when_complete_by_date_is_in_the_future()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(completeByDate: DateTime.Today + TimeSpan.FromDays(1));
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);


            // Then
            course.Overdue().Should().BeFalse();
        }

        [Test]
        public void Current_course_should_not_have_diagnostic_score_without_diagnostic_assessment()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(hasDiagnostic: false);
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);


            // Then
            course.HasDiagnosticScore().Should().BeFalse();
        }

        [Test]
        public void Current_course_should_not_have_diagnostic_score_without_diagnostic_score_value()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(diagnosticScore: null);
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);


            // Then
            course.HasDiagnosticScore().Should().BeFalse();
        }

        [Test]
        public void Current_course_should_have_diagnostic_score_with_diagnostic_score_value_and_diagnostic_assessment()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse();
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);


            // Then
            course.HasDiagnosticScore().Should().BeTrue();
        }

        [Test]
        public void Current_course_should_not_have_passed_sections_without_learning_assessment()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(isAssessed: false);
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);


            // Then
            course.HasPassedSections().Should().BeFalse();
        }

        [Test]
        public void Current_course_should_have_passed_sections_with_learning_assessment()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse();
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);


            // Then
            course.HasPassedSections().Should().BeTrue();
        }

        [Test]
        public void Trying_to_edit_complete_by_date_when_not_self_enrolled_should_return_403()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(enrollmentMethodId: 0, completeByDate: new DateTime(2020, 1, 1));
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var result = controller.SetCompleteByDate(currentCourse.CustomisationID, null, null, null);

            // Then
            (result as ViewResult).ViewName.Should().Be("Error/Forbidden");
        }

        [Test]
        public void Setting_a_valid_complete_by_date_should_call_the_course_service()
        {
            // Given
            const int newDay = 29;
            const int newMonth = 7;
            const int newYear = 3020;
            var newDate = new DateTime(newYear, newMonth, newDay);
            const int progressId = 1;

            // When
            controller.SetCompleteByDate(1, newDay, newMonth, newYear, 1);

            // Then
            A.CallTo(() => courseService.SetCompleteByDate(progressId, CandidateId, newDate)).MustHaveHappened();
        }

        [Test]
        public void Setting_an_empty_complete_by_date_should_call_the_course_service_with_null()
        {
            // Given
            const int progressId = 1;

            // When
            controller.SetCompleteByDate(1, 0, 0, 0, 1);

            // Then
            A.CallTo(() => courseService.SetCompleteByDate(progressId, CandidateId, null)).MustHaveHappened();
        }

        [Test]
        public void Setting_a_valid_complete_by_date_should_redirect_to_current_courses()
        {
            // When
            var result = (RedirectToActionResult)controller.SetCompleteByDate(1, 29, 7, 3020, 1);

            // Then
            result.ActionName.Should().Be("Current");
        }

        [Test]
        public void Setting_an_invalid_complete_by_date_should_not_call_the_course_service()
        {
            // When
            controller.SetCompleteByDate(1, 31, 2, 2020, 1);

            // Then
            A.CallTo(() => courseService.SetCompleteByDate(1, CandidateId, A<DateTime>._)).MustNotHaveHappened();
        }

        [Test]
        public void Setting_an_invalid_complete_by_date_should_redirect_with_an_error_message()
        {
            // Given
            const int id = 1;
            const int day = 31;
            const int month = 2;
            const int year = 2020;

            // When
            var result = (RedirectToActionResult)controller.SetCompleteByDate(id, day, month, year, 1);

            // Then
            result.ActionName.Should().Be("SetCompleteByDate");
            result.RouteValues["id"].Should().Be(id);
            result.RouteValues["day"].Should().Be(day);
            result.RouteValues["month"].Should().Be(month);
            result.RouteValues["year"].Should().Be(year);
        }

        [Test]
        public void Removing_a_current_course_should_call_the_course_service()
        {
            // When
            controller.RemoveCurrentCourse(1);

            // Then
            A.CallTo(() => courseService.RemoveCurrentCourse(1, CandidateId)).MustHaveHappened();

        }

        [Test]
        public void Requesting_a_course_unlock_should_call_the_unlock_service()
        {
            // Given
            const int progressId = 1;

            // When
            controller.RequestUnlock(progressId);

            // Then
            A.CallTo(() => unlockService.SendUnlockRequest(progressId)).MustHaveHappened();
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
