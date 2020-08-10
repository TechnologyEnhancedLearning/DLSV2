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
        private ICentresService centresService;
        private IConfigService configService;
        private ICourseService courseService;
        private IUnlockService unlockService;
        private IConfiguration config;
        private const string BaseUrl = "https://www.dls.nhs.uk";
        private const int CandidateId = 254480;
        private const int CentreId = 2;

        [SetUp]
        public void SetUp()
        {
            centresService = A.Fake<ICentresService>();
            configService = A.Fake<IConfigService>();
            courseService = A.Fake<ICourseService>();
            unlockService = A.Fake<IUnlockService>();
            var logger = A.Fake<ILogger<LearningPortalController>>();
            config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("learnCandidateID", CandidateId.ToString()),
                new Claim("UserCentreID", CentreId.ToString())
            }, "mock"));
            controller = new LearningPortalController(
                centresService,
                configService,
                courseService,
                unlockService,
                logger,
                config
            )
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext { User = user } }
            };
        }

        [Test]
        public void Current_action_should_return_view_result()
        {
            // Given
            var currentCourses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(),
                CurrentCourseHelper.CreateDefaultCurrentCourse()
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var result = controller.Current();

            // Then
            var expectedModel = new CurrentViewModel(currentCourses, config, "Course Name", "Ascending", "");
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Current_course_should_be_overdue_when_complete_by_date_is_in_the_past()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(completeByDate: DateTime.Today - TimeSpan.FromDays(1));
            var currentCourses = new[]
            {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);

            // Then
            course.DateStyle().Should().Be("overdue");
        }

        [Test]
        public void Current_course_should__be_due_soon_when_complete_by_date_is_in_the_future()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(completeByDate: DateTime.Today + TimeSpan.FromDays(1));
            var currentCourses = new[]
            {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);

            // Then
            course.DateStyle().Should().Be("due-soon");
        }

        [Test]
        public void Current_course_should_have_no_date_style_when_due_far_in_the_future()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(completeByDate: DateTime.Today + TimeSpan.FromDays(100));
            var currentCourses = new[] {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var course = CurrentCourseHelper.CurrentCourseViewModelFromController(controller);


            // Then
            course.DateStyle().Should().Be("");
        }

        [Test]
        public void Current_course_should_not_have_diagnostic_score_without_diagnostic_assessment()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(hasDiagnostic: false);
            var currentCourses = new[]
            {
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
            var currentCourses = new[]
            {
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
            var currentCourses = new[]
            {
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
            var currentCourses = new[]
            {
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
            var currentCourses = new[]
            {
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
            var currentCourses = new[]
            {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var result = controller.SetCompleteByDate(currentCourse.CustomisationID, null, null, null);

            // Then
            result.Should().BeViewResult().WithViewName("Error/Forbidden");
            controller.Response.StatusCode.Should().Be(403);
        }

        [Test]
        public void Trying_to_edit_complete_by_date_for_non_existent_course_should_return_404()
        {
            // Given
            var currentCourses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(2)
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var result = controller.SetCompleteByDate(3, null, null, null);

            // Then
            result.Should().BeViewResult().WithViewName("Error/PageNotFound");
            controller.Response.StatusCode.Should().Be(404);
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
        public void Remove_confirmation_for_a_current_course_should_show_confirmation()
        {
            // Given
            const int customisationId = 2;
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(customisationId);
            var currentCourses = new[]
            {
                currentCourse
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var result = controller.RemoveCurrentCourseConfirmation(customisationId);

            // Then
            var expectedModel = new CurrentCourseViewModel(currentCourse, config);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Removing_non_existent_course_should_return_404()
        {
            // Given
            var currentCourses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(2)
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var result = controller.RemoveCurrentCourseConfirmation(3);

            // Then
            result.Should().BeViewResult().WithViewName("Error/PageNotFound");
            controller.Response.StatusCode.Should().Be(404);
        }

        [Test]
        public void Requesting_a_course_unlock_should_call_the_unlock_service()
        {
            // Given
            const int progressId = 1;
            var currentCourses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(progressId: progressId, locked: true)
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            controller.RequestUnlock(progressId);

            // Then
            A.CallTo(() => unlockService.SendUnlockRequest(progressId)).MustHaveHappened();
        }

        [Test]
        public void Requesting_unlock_for_non_existent_course_should_return_404()
        {
            // Given
            var currentCourses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(progressId: 2, locked: true)
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var result = controller.RequestUnlock(3);

            // Then
            result.Should().BeViewResult().WithViewName("Error/PageNotFound");
            controller.Response.StatusCode.Should().Be(404);
        }

        [Test]
        public void Requesting_unlock_for_unlocked_course_should_return_404()
        {
            // Given
            const int progressId = 2;
            var currentCourses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(progressId: progressId)
            };
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var result = controller.RequestUnlock(progressId);

            // Then
            result.Should().BeViewResult().WithViewName("Error/PageNotFound");
            controller.Response.StatusCode.Should().Be(404);
        }

        [Test]
        public void Current_action_should_have_banner_text()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var currentViewModel = CurrentCourseHelper.CurrentViewModelFromController(controller);

            // Then
            currentViewModel.BannerText.Should().Be(bannerText);
        }

        [Test]
        public void Completed_action_should_return_view_result()
        {
            // Given
            var completedCourses = new[]
            {
                new Course { Id = 1, Name = "Course 1" },
                new Course { Id = 2, Name = "Course 2" }
            };
            A.CallTo(() => courseService.GetCompletedCourses()).Returns(completedCourses);

            // When
            var result = controller.Completed();

            // Then
            var expectedModel = new CompletedViewModel(completedCourses, "");
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Completed_action_should_have_banner_text()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var completedViewModel = CompletedCourseHelper.CompletedViewModelFromController(controller);;

            // Then
            completedViewModel.BannerText.Should().Be(bannerText);
        }

        [Test]
        public void Available_action_should_return_view_result()
        {
            // Given
            var availableCourses = new[]
            {
                new Course { Id = 1, Name = "Course 1" },
                new Course { Id = 2, Name = "Course 2" }
            };
            A.CallTo(() => courseService.GetAvailableCourses()).Returns(availableCourses);

            // When
            var result = controller.Available();

            // Then
            var expectedModel = new AvailableViewModel(availableCourses, "");
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Available_action_should_have_banner_text()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var availableViewModel = AvailableCourseHelper.AvailableViewModelFromController(controller);;

            // Then
            availableViewModel.BannerText.Should().Be(bannerText);
        }

        [Test]
        public void Error_should_render_the_error_view()
        {
            // When
            var result = controller.Error();

            // Then
            result.Should().BeViewResult().WithViewName("Error/UnknownError");
            controller.Response.StatusCode.Should().Be(500);
        }

        [Test]
        public void Error_should_pass_the_banner_text()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var result = controller.Error();

            // Then
            var expectedModel = new ErrorViewModel(bannerText);
            result.Should().BeViewResult()
                .ModelAs<ErrorViewModel>().HelpText().Should().Be(expectedModel.HelpText());

        }

        [Test]
        public void StatusCode_should_render_not_found_view_when_code_is_404()
        {
            // When
            var result = controller.StatusCode(404);

            // Then
            result.Should().BeViewResult().WithViewName("Error/PageNotFound");
            controller.Response.StatusCode.Should().Be(404);
        }

        [Test]
        public void StatusCode_should_render_forbidden_view_when_code_is_403()
        {
            // When
            var result = controller.StatusCode(403);

            // Then
            result.Should().BeViewResult().WithViewName("Error/Forbidden");
            controller.Response.StatusCode.Should().Be(403);
        }

        [Test]
        public void StatusCode_should_render_unknown_error_view_when_code_is_500()
        {
            // When
            var result = controller.StatusCode(500);

            // Then
            result.Should().BeViewResult().WithViewName("Error/UnknownError");
            controller.Response.StatusCode.Should().Be(500);
        }

        [Test]
        public void StatusCode_should_set_banner_text_when_code_is_404()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var result = controller.StatusCode(404);

            // Then
            var expectedModel = new ErrorViewModel(bannerText);
            result.Should().BeViewResult()
                .ModelAs<ErrorViewModel>().HelpText().Should().Be(expectedModel.HelpText());
        }

        [Test]
        public void StatusCode_should_set_banner_text_when_code_is_403()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var result = controller.StatusCode(403);

            // Then
            var expectedModel = new ErrorViewModel(bannerText);
            result.Should().BeViewResult()
                .ModelAs<ErrorViewModel>().HelpText().Should().Be(expectedModel.HelpText());
        }

        [Test]
        public void StatusCode_should_set_banner_text_when_code_is_500()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var result = controller.StatusCode(500);

            // Then
            var expectedModel = new ErrorViewModel(bannerText);
            result.Should().BeViewResult()
                .ModelAs<ErrorViewModel>().HelpText().Should().Be(expectedModel.HelpText());
        }
    }
}
