﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningPortal
{
    using System;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public partial class LearningPortalControllerTests
    {
        [Test]
        public void Current_action_should_return_view_result()
        {
            // Given
            var currentCourses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(),
                CurrentCourseHelper.CreateDefaultCurrentCourse()
            };
            var selfAssessments = new[]
            {
                SelfAssessmentHelper.CreateDefaultSelfAssessment(),
                SelfAssessmentHelper.CreateDefaultSelfAssessment(),
            };
            
            var bannerText = "bannerText";
            A.CallTo(() => courseService.GetCurrentCourses(CandidateId)).Returns(currentCourses);
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentsForCandidate(CandidateId)).Returns(selfAssessments);
            A.CallTo(() => centresDataService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var result = controller.Current();

            // Then
            var expectedModel = new CurrentPageViewModel(
                currentCourses,
                null,
                "Last Accessed Date",
                "Descending",
                selfAssessments,
                bannerText,
                1
            );
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
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
            var result = controller.SetCurrentCourseCompleteByDate(currentCourse.Id, null, null, null);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 403);
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
            var result = controller.SetCurrentCourseCompleteByDate(3, null, null, null);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
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
            controller.SetCurrentCourseCompleteByDate(1, newDay, newMonth, newYear, 1);

            // Then
            A.CallTo(() => courseService.SetCompleteByDate(progressId, CandidateId, newDate)).MustHaveHappened();
        }

        [Test]
        public void Setting_an_empty_complete_by_date_should_call_the_course_service_with_null()
        {
            // Given
            const int progressId = 1;

            // When
            controller.SetCurrentCourseCompleteByDate(1, 0, 0, 0, 1);

            // Then
            A.CallTo(() => courseService.SetCompleteByDate(progressId, CandidateId, null)).MustHaveHappened();
        }

        [Test]
        public void Setting_a_valid_complete_by_date_should_redirect_to_current_courses()
        {
            // When
            var result = (RedirectToActionResult)controller.SetCurrentCourseCompleteByDate(1, 29, 7, 3020, 1);

            // Then
            result.ActionName.Should().Be("Current");
        }

        [Test]
        public void Setting_an_invalid_complete_by_date_should_not_call_the_course_service()
        {
            // When
            controller.SetCurrentCourseCompleteByDate(1, 31, 2, 2020, 1);

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
            var result = (RedirectToActionResult)controller.SetCurrentCourseCompleteByDate(id, day, month, year, 1);

            // Then
            result.ActionName.Should().Be("SetCurrentCourseCompleteByDate");
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
            var expectedModel = new CurrentCourseViewModel(currentCourse);
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
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
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
            A.CallTo(() => notificationService.SendUnlockRequest(progressId)).MustHaveHappened();
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
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
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
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Current_action_should_have_banner_text()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresDataService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var currentViewModel = CurrentCourseHelper.CurrentPageViewModelFromController(controller);

            // Then
            currentViewModel.BannerText.Should().Be(bannerText);
        }
    }
}
