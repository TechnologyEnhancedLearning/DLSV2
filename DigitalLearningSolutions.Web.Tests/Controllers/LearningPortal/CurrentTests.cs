namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningPortal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;
    using CurrentCourseHelper = TestHelpers.CurrentCourseHelper;
    using SelfAssessmentHelper = TestHelpers.SelfAssessmentHelper;

    public partial class LearningPortalControllerTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public async Task Current_action_should_return_view_result_with_correct_api_accessibility_flag(
            bool apiIsAccessible
        )
        {
            // Given
            var currentCourses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(),
                CurrentCourseHelper.CreateDefaultCurrentCourse(),
            };
            var selfAssessments = new[]
            {
                SelfAssessmentHelper.CreateDefaultSelfAssessment(),
                SelfAssessmentHelper.CreateDefaultSelfAssessment(),
            };
            var actionPlanResources = Builder<ActionPlanResource>.CreateListOfSize(2).Build().ToArray();

            var bannerText = "bannerText";
            A.CallTo(() => courseDataService.GetCurrentCourses(CandidateId)).Returns(currentCourses);
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentsForCandidate(DelegateUserId)).Returns(selfAssessments);
            A.CallTo(() => actionPlanService.GetIncompleteActionPlanResources(CandidateId))
                .Returns((actionPlanResources, apiIsAccessible));
            A.CallTo(() => centresDataService.GetBannerText(CentreId)).Returns(bannerText);
            A.CallTo(() => config["FeatureManagement:UseSignposting"]).Returns("true");
            var allItems = currentCourses.Cast<CurrentLearningItem>().ToList();
            allItems.AddRange(selfAssessments);
            allItems.AddRange(actionPlanResources);
            SearchSortFilterAndPaginateTestHelper
                .GivenACallToSearchSortFilterPaginateServiceReturnsResult<CurrentLearningItem>(
                    searchSortFilterPaginateService
                );

            // When
            var result = await controller.Current();

            // Then
            var expectedModel = new CurrentPageViewModel(
                new SearchSortFilterPaginationResult<CurrentLearningItem>(
                    new PaginationResult<CurrentLearningItem>(allItems, 1, 1, 10, 6, true),
                    null,
                    "LastAccessed",
                    "Descending",
                    null
                ),
                apiIsAccessible,
                bannerText
            );
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public async Task Current_action_should_not_fetch_ActionPlanResources_if_signposting_disabled()
        {
            // Given
            GivenCurrentActivitiesAreEmptyLists();
            A.CallTo(() => config["FeatureManagement:UseSignposting"]).Returns("false");

            // When
            await controller.Current();

            // Then
            A.CallTo(() => actionPlanService.GetIncompleteActionPlanResources(CandidateId)).MustNotHaveHappened();
        }

        [Test]
        public async Task Current_action_should_fetch_ActionPlanResources_if_signposting_enabled()
        {
            // Given
            GivenCurrentActivitiesAreEmptyLists();
            A.CallTo(() => config["FeatureManagement:UseSignposting"]).Returns("true");

            // When
            await controller.Current();

            // Then
            A.CallTo(() => actionPlanService.GetIncompleteActionPlanResources(CandidateId))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task AllCurrentItems_action_should_not_fetch_ActionPlanResources_if_signposting_disabled()
        {
            // Given
            GivenCurrentActivitiesAreEmptyLists();
            A.CallTo(() => config["FeatureManagement:UseSignposting"]).Returns("false");

            // When
            await controller.AllCurrentItems();

            // Then
            A.CallTo(() => actionPlanService.GetIncompleteActionPlanResources(CandidateId)).MustNotHaveHappened();
        }

        [Test]
        public async Task AllCurrentItems_action_should_fetch_ActionPlanResources_if_signposting_enabled()
        {
            // Given
            GivenCurrentActivitiesAreEmptyLists();
            A.CallTo(() => config["FeatureManagement:UseSignposting"]).Returns("true");

            // When
            await controller.AllCurrentItems();

            // Then
            A.CallTo(() => actionPlanService.GetIncompleteActionPlanResources(CandidateId))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Trying_to_edit_complete_by_date_when_not_self_enrolled_should_return_403()
        {
            // Given
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(
                enrollmentMethodId: 0,
                completeByDate: new DateTime(2020, 1, 1)
            );
            var currentCourses = new[]
            {
                currentCourse,
            };
            A.CallTo(() => courseDataService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var result = controller.SetCurrentCourseCompleteByDate(currentCourse.Id, ReturnPageQueryHelper.GetDefaultReturnPageQuery());

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
                CurrentCourseHelper.CreateDefaultCurrentCourse(2),
            };
            A.CallTo(() => courseDataService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var result = controller.SetCurrentCourseCompleteByDate(3, ReturnPageQueryHelper.GetDefaultReturnPageQuery());

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Setting_a_valid_complete_by_date_for_course_should_call_the_course_service()
        {
            // Given
            const int id = 1;
            const int progressId = 1;
            const int newDay = 29;
            const int newMonth = 7;
            const int newYear = 3020;
            var formData = new EditCompleteByDateFormData { Day = newDay, Month = newMonth, Year = newYear };

            var newDate = new DateTime(newYear, newMonth, newDay);

            // When
            controller.SetCurrentCourseCompleteByDate(id, progressId, formData);

            // Then
            A.CallTo(() => courseDataService.SetCompleteByDate(progressId, CandidateId, newDate)).MustHaveHappened();
        }

        [Test]
        public void Setting_an_empty_complete_by_date_for_course_should_call_the_course_service_with_null()
        {
            // Given
            const int id = 1;
            const int progressId = 1;
            var formData = new EditCompleteByDateFormData { Day = null, Month = null, Year = null };

            // When
            controller.SetCurrentCourseCompleteByDate(id, progressId, formData);

            // Then
            A.CallTo(() => courseDataService.SetCompleteByDate(progressId, CandidateId, null)).MustHaveHappened();
        }

        [Test]
        public void Setting_a_valid_complete_by_date_should_redirect_to_current_courses()
        {
            // Given
            const int id = 1;
            const int progressId = 1;
            const int day = 29;
            const int month = 7;
            const int year = 3020;
            var formData = new EditCompleteByDateFormData { Day = day, Month = month, Year = year };

            // When
            var result = (RedirectToActionResult)controller.SetCurrentCourseCompleteByDate(id, progressId, formData);

            // Then
            result.ActionName.Should().Be("Current");
        }

        [Test]
        public void Setting_an_invalid_complete_by_date_for_course_should_not_call_the_course_service()
        {
            // Given
            const int id = 1;
            const int progressId = 1;
            const int day = 1;
            const int month = 1;
            const int year = 2020;
            var formData = new EditCompleteByDateFormData { Day = day, Month = month, Year = year };
            controller.ModelState.AddModelError("year", "message");

            // When
            controller.SetCurrentCourseCompleteByDate(id, progressId, formData);

            // Then
            A.CallTo(() => courseDataService.SetCompleteByDate(1, CandidateId, A<DateTime>._)).MustNotHaveHappened();
        }

        [Test]
        public void Removing_a_current_course_should_call_the_course_service()
        {
            // When
            controller.RemoveCurrentCourse(1);

            // Then
            A.CallTo(() => courseDataService.RemoveCurrentCourse(1, CandidateId, RemovalMethod.RemovedByDelegate))
                .MustHaveHappened();
        }

        [Test]
        public void Remove_confirmation_for_a_current_course_should_show_confirmation()
        {
            // Given
            const int customisationId = 2;
            var currentCourse = CurrentCourseHelper.CreateDefaultCurrentCourse(customisationId);
            var currentCourses = new[]
            {
                currentCourse,
            };
            A.CallTo(() => courseDataService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var result = controller.RemoveCurrentCourseConfirmation(customisationId, ReturnPageQueryHelper.GetDefaultReturnPageQuery());

            // Then
            var expectedModel = new CurrentCourseViewModel(currentCourse, ReturnPageQueryHelper.GetDefaultReturnPageQuery());
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Removing_non_existent_course_should_return_404()
        {
            // Given
            var currentCourses = new[]
            {
                CurrentCourseHelper.CreateDefaultCurrentCourse(2),
            };
            A.CallTo(() => courseDataService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

            // When
            var result = controller.RemoveCurrentCourseConfirmation(3, ReturnPageQueryHelper.GetDefaultReturnPageQuery());

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
                CurrentCourseHelper.CreateDefaultCurrentCourse(progressId: progressId, locked: true),
            };
            A.CallTo(() => courseDataService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

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
                CurrentCourseHelper.CreateDefaultCurrentCourse(progressId: 2, locked: true),
            };
            A.CallTo(() => courseDataService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

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
                CurrentCourseHelper.CreateDefaultCurrentCourse(progressId: progressId),
            };
            A.CallTo(() => courseDataService.GetCurrentCourses(CandidateId)).Returns(currentCourses);

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
        public async Task Current_action_should_have_banner_text()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresDataService.GetBannerText(CentreId)).Returns(bannerText);
            A.CallTo(() => config["FeatureManagement:UseSignposting"]).Returns("true");

            // When
            var currentViewModel = await CurrentCourseHelper.CurrentPageViewModelFromController(controller);

            // Then
            currentViewModel.BannerText.Should().Be(bannerText);
        }

        [Test]
        public void MarkActionPlanResourceAsComplete_calls_correct_service_method()
        {
            // Given
            const int learningLogItemId = 4;
            const int day = 1;
            const int month = 1;
            const int year = 2021;
            var formData = new MarkActionPlanResourceAsCompleteFormData { Day = day, Month = month, Year = year };
            var completedDate = new DateTime(year, month, day);
            A.CallTo(() => actionPlanService.SetCompletionDate(learningLogItemId, A<DateTime>._)).DoesNothing();

            // When
            var result = controller.MarkActionPlanResourceAsComplete(learningLogItemId, formData);

            // Then
            A.CallTo(() => actionPlanService.SetCompletionDate(learningLogItemId, completedDate))
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Current");
        }

        [Test]
        public void MarkActionPlanResourceAsComplete_does_not_call_service_with_invalid_model()
        {
            // Given
            const int learningLogItemId = 4;
            const int day = 1;
            const int month = 1;
            const int year = 4000;
            var formData = new MarkActionPlanResourceAsCompleteFormData { Day = day, Month = month, Year = year };
            var completedDate = new DateTime(year, month, day);
            controller.ModelState.AddModelError("year", "message");
            A.CallTo(() => actionPlanService.SetCompletionDate(learningLogItemId, A<DateTime>._)).DoesNothing();

            // When
            controller.MarkActionPlanResourceAsComplete(learningLogItemId, formData);

            // Then
            A.CallTo(() => actionPlanService.SetCompletionDate(learningLogItemId, completedDate))
                .MustNotHaveHappened();
        }

        private void GivenCurrentActivitiesAreEmptyLists()
        {
            A.CallTo(() => courseDataService.GetCurrentCourses(A<int>._)).Returns(new List<CurrentCourse>());
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentsForCandidate(A<int>._))
                .Returns(new List<CurrentSelfAssessment>());
            A.CallTo(() => actionPlanService.GetIncompleteActionPlanResources(A<int>._))
                .Returns((new List<ActionPlanResource>(), false));
            A.CallTo(() => centresDataService.GetBannerText(A<int>._)).Returns("bannerText");
        }
    }
}
