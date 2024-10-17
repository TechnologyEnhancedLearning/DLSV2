namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningPortal
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current;
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
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            var supervisors = new List<SelfAssessmentSupervisor>();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);
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
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);

            // When
            controller.SelfAssessment(SelfAssessmentId);

            // Then
            A.CallTo(() => selfAssessmentService.UpdateLastAccessed(selfAssessment.Id, DelegateUserId)).MustHaveHappened();
        }

        [Test]
        public void SelfAssessment_action_should_increment_launch_count()
        {
            // Given
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);

            // When
            controller.SelfAssessment(SelfAssessmentId);

            // Then
            A.CallTo(() => selfAssessmentService.IncrementLaunchCount(selfAssessment.Id, DelegateUserId))
                .MustHaveHappened();
        }

        [Test]
        public void SelfAssessment_action_without_self_assessment_should_return_403()
        {
            // Given
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(null);

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
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            var competency = new Competency();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetNthCompetency(competencyNumber, selfAssessment.Id, DelegateUserId))
                .Returns(competency);
            A.CallTo(() => frameworkService.GetSelectedCompetencyFlagsByCompetecyId(competency.Id))
                .Returns(new List<Data.Models.Frameworks.CompetencyFlag>() { });
            var expectedModel = new SelfAssessmentCompetencyViewModel(
                selfAssessment,
                competency,
                competencyNumber,
                selfAssessment.NumberOfCompetencies
            );

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
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);

            // When
            controller.SelfAssessmentCompetency(SelfAssessmentId, 1);

            // Then
            A.CallTo(() => selfAssessmentService.UpdateLastAccessed(selfAssessment.Id, DelegateUserId)).MustHaveHappened();
        }

        [Test]
        public void SelfAssessmentCompetency_action_should_update_user_bookmark()
        {
            // Given
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);
            string destUrl = "/LearningPortal/SelfAssessment/" + selfAssessment.Id + "/1";
            // When
            controller.SelfAssessmentCompetency(SelfAssessmentId, 1);

            // Then
            A.CallTo(() => selfAssessmentService.SetBookmark(selfAssessment.Id, DelegateUserId, destUrl))
                .MustHaveHappened();
        }

        [Test]
        public void SelfAssessmentCompetency_Redirects_To_Review_After_Last_Question()
        {
            // Given
            const int competencyNumber = 3;
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetNthCompetency(competencyNumber, selfAssessment.Id, DelegateUserId))
                .Returns(null);

            // When
            var result = controller.SelfAssessmentCompetency(SelfAssessmentId, competencyNumber);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("SelfAssessmentOverview");
        }

        [Test]
        public void SelfAssessmentCompetency_action_without_self_assessment_should_return_403()
        {
            // Given
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(null);

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
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            const int competencyNumber = 1;
            const int competencyId = 3;
            const int competencyGroupId = 1;
            const int assessmentQuestionId = 2;
            const int assessmentQuestionResult = 4;
            const int minValue = 0;
            const int maxValue = 10;
            const int assessmentQuestionInputTypeID = 2;
            var assessmentQuestions = new Collection<AssessmentQuestion>
            {
                new AssessmentQuestion
                {
                    Id = assessmentQuestionId,
                    Result = assessmentQuestionResult,
                    MinValue = minValue,
                    MaxValue = maxValue,
                    AssessmentQuestionInputTypeID = assessmentQuestionInputTypeID,
                },
            };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);
            var competency = selfAssessmentService.GetNthCompetency(competencyNumber, selfAssessment.Id, DelegateUserId);
            if (competency != null && !competency.AssessmentQuestions.Any(x => x.SignedOff == true))
            {
                // When
                controller.SelfAssessmentCompetency(
                SelfAssessmentId,
                assessmentQuestions,
                competencyNumber,
                competencyId,
                competencyGroupId
            );

                // Then
                A.CallTo(
                    () => selfAssessmentService.SetResultForCompetency(
                        competencyId,
                        selfAssessment.Id,
                        DelegateUserId,
                        assessmentQuestionId,
                        assessmentQuestionResult,
                        null
                    )
                ).MustHaveHappened();
            }
        }

        [Test]
        public void SelfAssessmentCompetency_Post_Redirects_To_Next_Question()
        {
            // Given
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            const int competencyNumber = 1;
            const int competencyId = 3;
            const int competencyGroupId = 1;
            const int assessmentQuestionId = 2;
            const int assessmentQuestionResult = 4;
            var assessmentQuestions = new Collection<AssessmentQuestion>
            {
                new AssessmentQuestion
                {
                    Id = assessmentQuestionId,
                    Result = assessmentQuestionResult,
                },
            };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);

            // When
            var result = controller.SelfAssessmentCompetency(
                SelfAssessmentId,
                assessmentQuestions,
                competencyNumber,
                competencyId,
                competencyGroupId
            );

            // Then
            result.Should().BeRedirectToActionResult()
                .WithActionName("SelfAssessmentCompetency")
                .WithRouteValue("competencyNumber", competencyNumber + 1);
        }

        [Test]
        public void SelfAssessmentCompetency_Post_without_self_assessment_should_return_403()
        {
            var assessmentQuestion = new List<AssessmentQuestion>();
            // Given
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(null);

            // When
            var result = controller.SelfAssessmentCompetency(1, assessmentQuestion, 1, 1, 1);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 403);
        }

        [Test]
        public void SelfAssessmentCompetency_Post_result_overriding_signedoff_question_should_redirect_to_confirmation_route()
        {
            // Given
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();

            var existingAssessmentQuestions = new List<AssessmentQuestion>
            {
                new AssessmentQuestion
                {
                    Id = 1,
                    Result = 0,
                    SignedOff = true,
                },
            };
            var competency = new Competency();
            competency.AssessmentQuestions = existingAssessmentQuestions;

            var userUpdatedAssessmentQuestions = new Collection<AssessmentQuestion>
            {
                new AssessmentQuestion
                {
                    Id = 1,
                    Result = 1,
                    SignedOff = true,
                },
            };

            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);


            A.CallTo(() => selfAssessmentService.GetNthCompetency(A<int>._, A<int>._, A<int>._))
                .Returns(competency);

            // When
            var result = controller.SelfAssessmentCompetency(1, userUpdatedAssessmentQuestions, 1, 1, 1);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithActionName("ConfirmOverwriteSelfAssessment");
        }

        [Test]
        public void SelfAssessmentCompetency_Post_result_not_overriding_signedoff_question_should_redirect_to_confirmation_route()
        {
            // Given
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();

            var existingAssessmentQuestions = new List<AssessmentQuestion>
            {
                new AssessmentQuestion
                {
                    Id = 1,
                    Result = 1,
                    SignedOff = true,
                },
                new AssessmentQuestion
                {
                    Id = 2,
                    Result = 0,
                    SignedOff = false,
                },
            };
            var competency = new Competency();
            competency.AssessmentQuestions = existingAssessmentQuestions;

            var userUpdatedAssessmentQuestions = new Collection<AssessmentQuestion>
            {
                new AssessmentQuestion
                {
                    Id = 1,
                    Result = 1,
                },
                new AssessmentQuestion
                {
                    Id = 1,
                    Result = 1,
                },
            };

            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);


            A.CallTo(() => selfAssessmentService.GetNthCompetency(A<int>._, A<int>._, A<int>._))
                .Returns(competency);

            // When
            var result = controller.SelfAssessmentCompetency(1, userUpdatedAssessmentQuestions, 1, 1, 1);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithActionName("SelfAssessmentCompetency");
        }

        [Test]
        public void SelfAssessmentOverview_Should_Return_View()
        {
            // Given
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            var appliedFilterViewModel = new List<AppliedFilterViewModel>();
            var competencies = new List<Competency>
            {
                new Competency { CompetencyGroup = "A" },
                new Competency { CompetencyGroup = "A" },
            };
            var supervisorSignOffs = new List<SupervisorSignOff>();
            var expectedModel = new SelfAssessmentOverviewViewModel
            {
                SelfAssessment = selfAssessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = 2,
                SupervisorSignOffs = supervisorSignOffs,
                SearchViewModel = new SearchSelfAssessmentOverviewViewModel("", SelfAssessmentId, selfAssessment.Vocabulary!, false, false, appliedFilterViewModel),
                AllQuestionsVerifiedOrNotRequired = true
            };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetMostRecentResults(selfAssessment.Id, DelegateUserId))
                .Returns(competencies);

            // When
            var result = controller.SelfAssessmentOverview(SelfAssessmentId, selfAssessment.Vocabulary!);

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/SelfAssessmentOverview")
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void SelfAssessmentOverview_Should_Return_View_With_Optional_Competency()
        {
            // Given
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            var appliedFilterViewModel = new List<AppliedFilterViewModel>();
            var competencies = new List<Competency>
            {
                new Competency { CompetencyGroup = "A", Id = 1, CompetencyGroupID = 1,SelfAssessmentStructureId=1, Optional = true },
                new Competency { CompetencyGroup = "A", Id = 2, CompetencyGroupID = 1,SelfAssessmentStructureId=1, Optional = false },
            };
            var supervisorSignOffs = new List<SupervisorSignOff>();
            var expectedModel = new SelfAssessmentOverviewViewModel
            {
                SelfAssessment = selfAssessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = 2,
                SupervisorSignOffs = supervisorSignOffs,
                SearchViewModel = new SearchSelfAssessmentOverviewViewModel("", SelfAssessmentId, selfAssessment.Vocabulary!, false, false, appliedFilterViewModel),
                AllQuestionsVerifiedOrNotRequired = true
            };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetMostRecentResults(selfAssessment.Id, DelegateUserId))
                .Returns(competencies);

            // When
            var result = controller.SelfAssessmentOverview(SelfAssessmentId, selfAssessment.Vocabulary!);

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/SelfAssessmentOverview")
                .Model.Should().BeEquivalentTo(expectedModel);

            result.Should().BeViewResult().ModelAs<SelfAssessmentOverviewViewModel>().CompetencyGroups.ToList()[0].ToList()[0].Optional.Should().Be(true);
            result.Should().BeViewResult().ModelAs<SelfAssessmentOverviewViewModel>().CompetencyGroups.ToList()[0].ToList()[1].Optional.Should().Be(false);
        }

        [Test]
        public void SelfAssessmentOverview_action_should_update_last_accessed()
        {
            // Given
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetMostRecentResults(SelfAssessmentId, DelegateUserId))
                .Returns(new List<Competency>() { });

            // When
            controller.SelfAssessmentOverview(SelfAssessmentId, selfAssessment.Vocabulary!);

            // Then
            A.CallTo(() => selfAssessmentService.UpdateLastAccessed(selfAssessment.Id, DelegateUserId)).MustHaveHappened();
        }

        [Test]
        public void SelfAssessmentOverview_action_should_update_user_bookmark()
        {
            // Given
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);
            string destUrl = $"/LearningPortal/SelfAssessment/{selfAssessment.Id}/{selfAssessment.Vocabulary}";
            // When
            controller.SelfAssessmentOverview(SelfAssessmentId, selfAssessment.Vocabulary!);

            // Then
            A.CallTo(() => selfAssessmentService.SetBookmark(selfAssessment.Id, DelegateUserId, destUrl))
                .MustHaveHappened();
        }

        [Test]
        public void SelfAssessmentOverview_Should_Have_Previous_Competency_Number_One_When_Empty()
        {
            // Given
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            var competencies = new List<Competency>();
            var supervisorSignOffs = new List<SupervisorSignOff>();
            var expectedModel = new SelfAssessmentOverviewViewModel
            {
                SelfAssessment = selfAssessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = 1,
                SupervisorSignOffs = supervisorSignOffs,
                SearchViewModel = new SearchSelfAssessmentOverviewViewModel(null!, SelfAssessmentId, selfAssessment.Vocabulary!, false, false, null!),
                AllQuestionsVerifiedOrNotRequired = true
            };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetMostRecentResults(selfAssessment.Id, DelegateUserId))
                .Returns(competencies);

            // When
            var result = controller.SelfAssessmentOverview(SelfAssessmentId, selfAssessment.Vocabulary!);

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/SelfAssessmentOverview")
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void SelfAssessmentOverview_action_without_self_assessment_should_return_403()
        {
            // Given
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(null);

            // When
            var result = controller.SelfAssessmentOverview(SelfAssessmentId, Vocabulary);

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
            var formData = new EditCompleteByDateFormData { Day = newDay, Month = newMonth, Year = newYear };
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);

            // When
            controller.SetSelfAssessmentCompleteByDate(selfAssessmentId, formData);

            // Then
            A.CallTo(
                () => selfAssessmentService.SetCompleteByDate(selfAssessmentId, DelegateUserId, newDate)
            ).MustHaveHappened();
        }

        [Test]
        public void SetSelfAssessmentCompleteByDate_post_action_empty_complete_by_date_should_call_the_course_service()
        {
            // Given
            const int selfAssessmentId = 1;
            var formData = new EditCompleteByDateFormData { Day = null, Month = null, Year = null };
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);

            // When
            controller.SetSelfAssessmentCompleteByDate(selfAssessmentId, formData);

            // Then
            A.CallTo(
                () => selfAssessmentService.SetCompleteByDate(selfAssessmentId, DelegateUserId, null)
            ).MustHaveHappened();
        }

        [Test]
        public void
            SetSelfAssessmentCompleteByDate_post_action_valid_complete_by_date_should_redirect_to_current_courses()
        {
            // Given
            const int selfAssessmentId = 1;
            const int day = 29;
            const int month = 7;
            const int year = 3020;
            var formData = new EditCompleteByDateFormData { Day = day, Month = month, Year = year };
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);

            // When
            var result = (RedirectToActionResult)controller.SetSelfAssessmentCompleteByDate(selfAssessmentId, formData);

            // Then
            result.ActionName.Should().Be("Current");
        }

        [Test]
        public void
            SetSelfAssessmentCompleteByDate_post_action_invalid_complete_by_date_should_not_call_the_course_service()
        {
            // Given
            const int selfAssessmentId = 1;
            const int day = 1;
            const int month = 1;
            const int year = 2020;
            var formData = new EditCompleteByDateFormData { Day = day, Month = month, Year = year };
            controller.ModelState.AddModelError("year", "message");
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);

            // When
            controller.SetSelfAssessmentCompleteByDate(selfAssessmentId, formData);

            // Then
            A.CallTo(
                () => selfAssessmentService.SetCompleteByDate(selfAssessmentId, DelegateUserId, A<DateTime>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void SetSelfAssessmentCompleteByDate_get_action_without_self_assessment_should_return_403()
        {
            // Given
            const int day = 2;
            const int month = 2;
            const int year = 2020;
            var formData = new EditCompleteByDateFormData { Day = day, Month = month, Year = year };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(null);

            // When
            var result = controller.SetSelfAssessmentCompleteByDate(999, formData);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 403);
        }

        [Test]
        public void ResendSupervisorSignOffRequest_sends_email_and_navigates_to_confirmation_view()
        {
            // Given
            var expectedModel = new ResendSupervisorSignOffEmailViewModel
            {
                Id = 1,
                Vocabulary = "TestVocabulary",
                SupervisorName = "TestSupervisorName",
                SupervisorEmail = "testsupervisor@example.com",
            };

            // When
            var result = controller.ResendSupervisorSignOffRequest(1, 2, 3, "TestSupervisorName", "testsupervisor@example.com", "TestVocabulary");

            // Then
            A.CallTo(
                () => frameworkNotificationService.SendSignOffRequest(
                2,
                1,
                11,
                2
                )).MustHaveHappened();

            A.CallTo(
                () => selfAssessmentService.UpdateCandidateAssessmentSupervisorVerificationEmailSent(3)).MustHaveHappened();

            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/ResendSupervisorSignoffEmailConfirmation")
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void WithdrawSupervisorSignOffRequest_calls_remove_sign_off_and_reloads_sign_off_history_page()
        {
            // Given
            var expectedModel = new ResendSupervisorSignOffEmailViewModel
            {
                Id = 1,
                Vocabulary = "TestVocabulary",
                SupervisorName = "TestSupervisorName",
                SupervisorEmail = "testsupervisor@example.com",
            };

            // When
            var result = controller.WithdrawSupervisorSignOffRequest(1, 2, "TestVocabulary", "SignOffHistory");

            // Then
            A.CallTo(
                () => supervisorService.RemoveCandidateAssessmentSupervisorVerification(2)).MustHaveHappened();

            result.Should()
                .BeRedirectToActionResult()
                .WithActionName("SignOffHistory");
        }

        [Test]
        public void WithdrawSupervisorSignOffRequest_calls_remove_sign_off_and_defaults_route_to_self_assessment_overview_page()
        {
            // Given
            var expectedModel = new ResendSupervisorSignOffEmailViewModel
            {
                Id = 1,
                Vocabulary = "TestVocabulary",
                SupervisorName = "TestSupervisorName",
                SupervisorEmail = "testsupervisor@example.com",
            };

            // When
            var result = controller.WithdrawSupervisorSignOffRequest(1, 2, "TestVocabulary", "");

            // Then
            A.CallTo(
                () => supervisorService.RemoveCandidateAssessmentSupervisorVerification(2)).MustHaveHappened();

            result.Should()
                .BeRedirectToActionResult()
                .WithActionName("SelfAssessmentOverview");
        }

        [Test]
        public void ManageOptionalCompetencies_Should_Return_View_With_Flag()
        {
            // Given
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            var appliedFilterViewModel = new List<AppliedFilterViewModel>();
            var CompetencyFlags = new List<Data.Models.Frameworks.CompetencyFlag> { new Data.Models.Frameworks.CompetencyFlag { CompetencyId = 1, FlagId = 1, FlagGroup = "Purple", FlagName = "Supernumerary", FlagTagClass = "nhsuk-tag--purple", FrameworkId = 1, Selected = true }, };

            var optionalCompetencies = new List<Competency>
    {
        new Competency { CompetencyGroup = "A", Id = 1, CompetencyGroupID = 1,SelfAssessmentStructureId=1, Optional = true },
        new Competency { CompetencyGroup = "A", Id = 2, CompetencyGroupID = 1,SelfAssessmentStructureId=1, Optional = true },
    };
            var competencyIds = optionalCompetencies.Select(c => c.Id).ToArray();
            var includedSelfAssessmentStructureIds = new List<int> { 1, 2 };

            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetCandidateAssessmentOptionalCompetencies(selfAssessment.Id, DelegateUserId))
                .Returns(optionalCompetencies);
            A.CallTo(() => frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(A<int[]>.That.Matches(ids => ids.Length == 2 && ids[0] == 1 && ids[1] == 2)))
                .Returns(CompetencyFlags);
            A.CallTo(() => selfAssessmentService.GetCandidateAssessmentIncludedSelfAssessmentStructureIds(selfAssessment.Id, DelegateUserId))
                .Returns(includedSelfAssessmentStructureIds);

            var model = new ManageOptionalCompetenciesViewModel
            {
                SelfAssessment = selfAssessment,
                CompetencyGroups = optionalCompetencies.GroupBy(competency => competency.CompetencyGroup),
                IncludedSelfAssessmentStructureIds = includedSelfAssessmentStructureIds
            };

            // When
            var result = controller.ManageOptionalCompetencies(SelfAssessmentId);

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/ManageOptionalCompetencies")
                .Model.Should().BeEquivalentTo(model);

            result.Should().BeViewResult().ModelAs<ManageOptionalCompetenciesViewModel>().CompetencyGroups?.ToList()[0].ToList()[0].CompetencyFlags.Count().Should().Be(1);
            result.Should().BeViewResult().ModelAs<ManageOptionalCompetenciesViewModel>().CompetencyGroups?.ToList()[0].ToList()[0].CompetencyFlags.ToList()[0].FlagName.Should().Be("Supernumerary");

            result.Should().BeViewResult().ModelAs<ManageOptionalCompetenciesViewModel>().CompetencyGroups?.ToList()[0].ToList()[1].CompetencyFlags.Count().Should().Be(0);
        }


        [Test]
        public void SelfAssessmentOverview_Should_Return_View_With_All_Competencies()
        {
            // Given
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            var appliedFilterViewModel = new List<AppliedFilterViewModel>();
            var competencies = new List<Competency>
            {
                new Competency { CompetencyGroup = "A", Id = 1, CompetencyGroupID = 1,SelfAssessmentStructureId=1, Optional = true },
                new Competency { CompetencyGroup = "A", Id = 2, CompetencyGroupID = 1,SelfAssessmentStructureId=2, Optional = false },
                new Competency { CompetencyGroup = "A", Id = 3, CompetencyGroupID = 1,SelfAssessmentStructureId=3, Optional = false },
            };
            var supervisorSignOffs = new List<SupervisorSignOff>();
            var expectedModel = new SelfAssessmentOverviewViewModel
            {
                SelfAssessment = selfAssessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = 3,
                SupervisorSignOffs = supervisorSignOffs,
                SearchViewModel = new SearchSelfAssessmentOverviewViewModel("", SelfAssessmentId, selfAssessment.Vocabulary!, false, false, appliedFilterViewModel),
                AllQuestionsVerifiedOrNotRequired = true
            };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetMostRecentResults(selfAssessment.Id, DelegateUserId))
                .Returns(competencies);

            // When
            var result = controller.SelfAssessmentOverview(SelfAssessmentId, selfAssessment.Vocabulary!);

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/SelfAssessmentOverview")
                .Model.Should().BeEquivalentTo(expectedModel);

            result.Should().BeViewResult().ModelAs<SelfAssessmentOverviewViewModel>().CompetencyGroups.ToList()[0].Count().Should().Be(3);
        }

        [Test]
        public void SelfAssessmentOverview_Should_Return_View_With_Optional_Filter_Applied()
        {
            // Given
            var selfAssessment = SelfAssessmentTestHelper.CreateDefaultSelfAssessment();
            var appliedFilterViewModel = new List<AppliedFilterViewModel>
            {
                new AppliedFilterViewModel{DisplayText = "Optional", FilterCategory =  null!, FilterValue ="-4", TagClass = ""},
            };
            var search = new SearchSelfAssessmentOverviewViewModel
            {
                AppliedFilters = appliedFilterViewModel,
                SelfAssessmentId = SelfAssessmentId,
                Vocabulary = selfAssessment.Vocabulary!,
                SearchText = "",
                CompetencyFlags = null!,

            };
            var competencies = new List<Competency>
            {
                new Competency { CompetencyGroup = "A", Id = 1, CompetencyGroupID = 1,SelfAssessmentStructureId=1, Optional = true },
                new Competency { CompetencyGroup = "A", Id = 2, CompetencyGroupID = 1,SelfAssessmentStructureId=2, Optional = false },
                new Competency { CompetencyGroup = "A", Id = 3, CompetencyGroupID = 1,SelfAssessmentStructureId=3, Optional = false },
            };
            var optionalCompetencies = new List<Competency>
            {
                new Competency { CompetencyGroup = "A", Id = 1, CompetencyGroupID = 1,SelfAssessmentStructureId=1, Optional = true },
            };
            var supervisorSignOffs = new List<SupervisorSignOff>();
            var expectedModel = new SelfAssessmentOverviewViewModel
            {
                SelfAssessment = selfAssessment,
                CompetencyGroups = optionalCompetencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = 1,
                SupervisorSignOffs = supervisorSignOffs,
                SearchViewModel = search,
                AllQuestionsVerifiedOrNotRequired = true,
                NumberOfOptionalCompetencies = 1,
            };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetMostRecentResults(selfAssessment.Id, DelegateUserId))
                .Returns(competencies);
            A.CallTo(() => selfAssessmentService.GetCandidateAssessmentOptionalCompetencies(selfAssessment.Id, DelegateUserId))
               .Returns(optionalCompetencies);

            // When
            var result = controller.SelfAssessmentOverview(SelfAssessmentId, selfAssessment.Vocabulary!, searchModel: search);

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/SelfAssessmentOverview")
                .Model.Should().BeEquivalentTo(expectedModel);

            result.Should().BeViewResult().ModelAs<SelfAssessmentOverviewViewModel>().CompetencyGroups.ToList()[0].Count().Should().Be(1);
        }
    }
}
