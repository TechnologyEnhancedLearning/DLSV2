﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningPortal
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public partial class LearningPortalControllerTests
    {
        [Test]
        public void SelfAssessment_action_should_return_view_result()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            var supervisors = new List<SelfAssessmentSupervisor>();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);
            var expectedModel = new SelfAssessmentDescriptionViewModel(selfAssessment, supervisors);

            // When
            var result = controller.SelfAssessment(SelfAssessmentId);

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/SelfAssessmentDescription")
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void SelfAssessment_action_should_update_last_accessed()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);

            // When
            controller.SelfAssessment(SelfAssessmentId);

            // Then
            A.CallTo(() => selfAssessmentService.UpdateLastAccessed(selfAssessment.Id, CandidateId)).MustHaveHappened();
        }
        [Test]
        public void SelfAssessment_action_should_increment_launch_count()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);

            // When
            controller.SelfAssessment(SelfAssessmentId);

            // Then
            A.CallTo(() => selfAssessmentService.IncrementLaunchCount(selfAssessment.Id, CandidateId)).MustHaveHappened();
        }

        [Test]
        public void SelfAssessment_action_without_self_assessment_should_return_403()
        {
            // Given
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(null);

            // When
            var result = controller.SelfAssessment(SelfAssessmentId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 403);
        }

        [Test]
        public void SelfAssessmentCompetency_action_should_return_view_result()
        {
            // Given
            const int competencyNumber = 1;
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            var competency = new Competency();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetNthCompetency(competencyNumber, selfAssessment.Id, CandidateId)).Returns(competency);
            var expectedModel = new SelfAssessmentCompetencyViewModel(selfAssessment, competency, competencyNumber, selfAssessment.NumberOfCompetencies);

            // When
            var result = controller.SelfAssessmentCompetency(SelfAssessmentId, competencyNumber);

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/SelfAssessmentCompetency")
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void SelfAssessmentCompetency_action_should_update_last_accessed()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);

            // When
            controller.SelfAssessmentCompetency(SelfAssessmentId, 1);

            // Then
            A.CallTo(() => selfAssessmentService.UpdateLastAccessed(selfAssessment.Id, CandidateId)).MustHaveHappened();
        }
        [Test]
        public void SelfAssessmentCompetency_action_should_update_user_bookmark()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);
            string destUrl = "/LearningPortal/SelfAssessment/" + selfAssessment.Id.ToString() + "/1";
            // When
            controller.SelfAssessmentCompetency(SelfAssessmentId, 1);

            // Then
            A.CallTo(() => selfAssessmentService.SetBookmark(selfAssessment.Id, CandidateId, destUrl)).MustHaveHappened();
        }

        [Test]
        public void SelfAssessmentCompetency_Redirects_To_Review_After_Last_Question()
        {
            // Given
            const int competencyNumber = 3;
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetNthCompetency(competencyNumber, selfAssessment.Id, CandidateId)).Returns(null);

            // When
            var result = controller.SelfAssessmentCompetency(SelfAssessmentId, competencyNumber);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("SelfAssessmentOverview");
        }

        [Test]
        public void SelfAssessmentCompetency_action_without_self_assessment_should_return_403()
        {
            // Given
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(null);

            // When
            var result = controller.SelfAssessmentCompetency(SelfAssessmentId, 1);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 403);
        }

        [Test]
        public void SelfAssessmentCompetency_Post_Should_Save_Answers()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            const int competencyNumber = 1;
            const int competencyId = 3;
            const int assessmentQuestionId = 2;
            const int assessmentQuestionResult = 4;
            const int minValue = 0;
            const int maxValue = 10;
            const int assessmentQuestionInputTypeID = 2;
            var assessmentQuestions = new Collection<AssessmentQuestion>()
            {
                new AssessmentQuestion()
                {
                    Id = assessmentQuestionId,
                    Result = assessmentQuestionResult,
                    MinValue = minValue,
                    MaxValue = maxValue,
                    AssessmentQuestionInputTypeID = assessmentQuestionInputTypeID
                }
            };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);

            // When
            controller.SelfAssessmentCompetency(SelfAssessmentId, assessmentQuestions, competencyNumber, competencyId);

            // Then
            A.CallTo(() => selfAssessmentService.SetResultForCompetency(
                competencyId,
                selfAssessment.Id,
                CandidateId,
                assessmentQuestionId,
                assessmentQuestionResult,
                null
            )).MustHaveHappened();
        }

        [Test]
        public void SelfAssessmentCompetency_Post_Redirects_To_Next_Question()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            const int competencyNumber = 1;
            const int competencyId = 3;
            const int assessmentQuestionId = 2;
            const int assessmentQuestionResult = 4;
            var assessmentQuestions = new Collection<AssessmentQuestion>()
            {
                new AssessmentQuestion()
                {
                    Id = assessmentQuestionId,
                    Result = assessmentQuestionResult
                }
            };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);

            // When
            var result = controller.SelfAssessmentCompetency(SelfAssessmentId, assessmentQuestions, competencyNumber, competencyId);

            // Then
            result.Should().BeRedirectToActionResult()
                .WithActionName("SelfAssessmentCompetency")
                .WithRouteValue("competencyNumber", competencyNumber + 1);
        }

        [Test]
        public void SelfAssessmentCompetency_Post_without_self_assessment_should_return_403()
        {
            // Given
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(null);

            // When
            var result = controller.SelfAssessmentCompetency(1, null, 1, 1);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 403);
        }

        [Test]
        public void SelfAssessmentOverview_Should_Return_View()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            var competencies = new List<Competency>()
            {
                new Competency() { CompetencyGroup = "A" },
                new Competency() { CompetencyGroup = "A" }
            };
            var expectedModel = new SelfAssessmentOverviewViewModel()
            {
                SelfAssessment = selfAssessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = 2
            };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetMostRecentResults(selfAssessment.Id, CandidateId)).Returns(competencies);

            // When
            var result = controller.SelfAssessmentOverview(SelfAssessmentId);

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/SelfAssessmentOverview")
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void SelfAssessmentOverview_action_should_update_last_accessed()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);

            // When
            controller.SelfAssessmentOverview(SelfAssessmentId);

            // Then
            A.CallTo(() => selfAssessmentService.UpdateLastAccessed(selfAssessment.Id, CandidateId)).MustHaveHappened();
        }
        [Test]
        public void SelfAssessmentOverview_action_should_update_user_bookmark()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);
            string destUrl = "/LearningPortal/SelfAssessment/" + selfAssessment.Id.ToString() + "/Overview";
            // When
            controller.SelfAssessmentOverview(SelfAssessmentId);

            // Then
            A.CallTo(() => selfAssessmentService.SetBookmark(selfAssessment.Id, CandidateId, destUrl)).MustHaveHappened();
        }
        [Test]
        public void SelfAssessmentOverview_Should_Have_Previous_Competency_Number_One_When_Empty()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            var competencies = new List<Competency>();
            var expectedModel = new SelfAssessmentOverviewViewModel()
            {
                SelfAssessment = selfAssessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = 1
            };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetMostRecentResults(selfAssessment.Id, CandidateId)).Returns(competencies);

            // When
            var result = controller.SelfAssessmentOverview(SelfAssessmentId);

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/SelfAssessmentOverview")
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void SelfAssessmentOverview_action_without_self_assessment_should_return_403()
        {
            // Given
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(null);

            // When
            var result = controller.SelfAssessmentOverview(SelfAssessmentId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 403);
        }

        [Test]
        public void SetSelfAssessmentCompleteByDate_post_action_valid_complete_by_date_should_call_the_course_service()
        {
            // Given
            const int selfAssessmentId = 1;
            const int newDay = 29;
            const int newMonth = 7;
            const int newYear = 3020;
            var newDate = new DateTime(newYear, newMonth, newDay);
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);

            // When
            controller.SetSelfAssessmentCompleteByDate(selfAssessmentId, newDay, newMonth, newYear);

            // Then
            A.CallTo(
                () => selfAssessmentService.SetCompleteByDate(selfAssessmentId, CandidateId, newDate)
            ).MustHaveHappened();
        }

        [Test]
        public void SetSelfAssessmentCompleteByDate_post_action_empty_complete_by_date_should_call_the_course_service()
        {
            // Given
            const int selfAssessmentId = 1;
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);

            // When
            controller.SetSelfAssessmentCompleteByDate(selfAssessmentId, 0, 0, 0);

            // Then
            A.CallTo(
                () => selfAssessmentService.SetCompleteByDate(selfAssessmentId, CandidateId, null)
            ).MustHaveHappened();
        }

        [Test]
        public void SetSelfAssessmentCompleteByDate_post_action_valid_complete_by_date_should_redirect_to_current_courses()
        {
            // Given
            const int selfAssessmentId = 1;
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);

            // When
            var result = (RedirectToActionResult)controller.SetSelfAssessmentCompleteByDate(selfAssessmentId, 29, 7, 3020);

            // Then
            result.ActionName.Should().Be("Current");
        }

        [Test]
        public void SetSelfAssessmentCompleteByDate_post_action_invalid_complete_by_date_should_not_call_the_course_service()
        {
            // Given
            const int selfAssessmentId = 1;
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);

            // When
            controller.SetSelfAssessmentCompleteByDate(31, 2, 2020, selfAssessmentId);

            // Then
            A.CallTo(
                () => selfAssessmentService.SetCompleteByDate(selfAssessmentId, CandidateId, A<DateTime>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void SetSelfAssessmentCompleteByDate_post_action_invalid_complete_by_date_should_redirect_with_an_error_message()
        {
            // Given
            const int selfAssessmentId = 1;
            const int day = 31;
            const int month = 2;
            const int year = 2020;
            var selfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(selfAssessment);

            // When
            var result = (RedirectToActionResult)controller.SetSelfAssessmentCompleteByDate(selfAssessmentId, day, month, year);

            // Then
            result.ActionName.Should().Be("SetSelfAssessmentCompleteByDate");
            result.RouteValues["day"].Should().Be(day);
            result.RouteValues["month"].Should().Be(month);
            result.RouteValues["year"].Should().Be(year);
        }

        [Test]
        public void SetSelfAssessmentCompleteByDate_get_action_without_self_assessment_should_return_403()
        {
            // Given
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)).Returns(null);

            // When
            var result = controller.SetSelfAssessmentCompleteByDate(2, 2, 2020, 999);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 403);
        }


    }
}
