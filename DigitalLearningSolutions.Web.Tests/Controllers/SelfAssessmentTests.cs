namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public partial class LearningPortalControllerTests
    {
        [Test]
        public void SelfAssessment_action_should_return_view_result()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.SelfAssessment();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidate(CandidateId)).Returns(selfAssessment);
            var expectedModel = new SelfAssessmentDescriptionViewModel(selfAssessment);

            // When
            var result = controller.SelfAssessment();

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/SelfAssessmentDescription")
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void SelfAssessment_action_without_self_assessment_should_return_403()
        {
            // Given
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidate(CandidateId)).Returns(null);

            // When
            var result = controller.SelfAssessment();

            // Then
            result.Should().BeViewResult().WithViewName("Error/Forbidden");
            controller.Response.StatusCode.Should().Be(403);
        }

        [Test]
        public void SelfAssessmentCompetency_action_should_return_view_result()
        {
            // Given
            const int competencyNumber = 1;
            var selfAssessment = SelfAssessmentHelper.SelfAssessment();
            var competency = new Competency();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidate(CandidateId)).Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetNthCompetency(competencyNumber, selfAssessment.Id, CandidateId)).Returns(competency);
            var expectedModel = new SelfAssessmentCompetencyViewModel(selfAssessment, competency, competencyNumber);

            // When
            var result = controller.SelfAssessmentCompetency(competencyNumber);

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/SelfAssessmentCompetency")
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void SelfAssessmentCompetency_Redirects_To_Review_After_Last_Question()
        {
            // Given
            const int competencyNumber = 3;
            var selfAssessment = SelfAssessmentHelper.SelfAssessment();
            var competency = new Competency();
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidate(CandidateId)).Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetNthCompetency(competencyNumber, selfAssessment.Id, CandidateId)).Returns(null);

            // When
            var result = controller.SelfAssessmentCompetency(competencyNumber);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("SelfAssessmentReview");
        }

        [Test]
        public void SelfAssessmentCompetency_action_without_self_assessment_should_return_403()
        {
            // Given
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidate(CandidateId)).Returns(null);

            // When
            var result = controller.SelfAssessmentCompetency(1);

            // Then
            result.Should().BeViewResult().WithViewName("Error/Forbidden");
            controller.Response.StatusCode.Should().Be(403);
        }

        [Test]
        public void SelfAssessmentCompetency_Post_Should_Save_Answers()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.SelfAssessment();
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
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidate(CandidateId)).Returns(selfAssessment);

            // When
            controller.SelfAssessmentCompetency(assessmentQuestions, competencyNumber, competencyId);

            // Then
            A.CallTo(() => selfAssessmentService.SetResultForCompetency(
                competencyId,
                selfAssessment.Id,
                CandidateId,
                assessmentQuestionId,
                assessmentQuestionResult
            )).MustHaveHappened();
        }

        [Test]
        public void SelfAssessmentCompetency_Post_Redirects_To_Next_Question()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.SelfAssessment();
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
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidate(CandidateId)).Returns(selfAssessment);

            // When
            var result = controller.SelfAssessmentCompetency(assessmentQuestions, competencyNumber, competencyId);

            // Then
            result.Should().BeRedirectToActionResult()
                .WithActionName("SelfAssessmentCompetency")
                .WithRouteValue("competencyNumber", competencyNumber + 1);
        }

        [Test]
        public void SelfAssessmentCompetency_Post_without_self_assessment_should_return_403()
        {
            // Given
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidate(CandidateId)).Returns(null);

            // When
            var result = controller.SelfAssessmentCompetency(null, 1, 1);

            // Then
            result.Should().BeViewResult().WithViewName("Error/Forbidden");
            controller.Response.StatusCode.Should().Be(403);
        }

        [Test]
        public void SelfAssessmentReview_Should_Return_View()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.SelfAssessment();
            var competencies = new List<Competency>()
            {
                new Competency() { CompetencyGroup = "A" },
                new Competency() { CompetencyGroup = "A" }
            };
            var expectedModel = new SelfAssessmentReviewViewModel()
            {
                SelfAssessment = selfAssessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = 2
            };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidate(CandidateId)).Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetMostRecentResults(selfAssessment.Id, CandidateId)).Returns(competencies);

            // When
            var result = controller.SelfAssessmentReview();

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/SelfAssessmentReview")
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void SelfAssessmentReview_Should_Have_Previous_Competency_Number_One_When_Empty()
        {
            // Given
            var selfAssessment = SelfAssessmentHelper.SelfAssessment();
            var competencies = new List<Competency>();
            var expectedModel = new SelfAssessmentReviewViewModel()
            {
                SelfAssessment = selfAssessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = 1
            };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidate(CandidateId)).Returns(selfAssessment);
            A.CallTo(() => selfAssessmentService.GetMostRecentResults(selfAssessment.Id, CandidateId)).Returns(competencies);

            // When
            var result = controller.SelfAssessmentReview();

            // Then
            result.Should().BeViewResult()
                .WithViewName("SelfAssessments/SelfAssessmentReview")
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void SelfAssessmentReview_action_without_self_assessment_should_return_403()
        {
            // Given
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidate(CandidateId)).Returns(null);

            // When
            var result = controller.SelfAssessmentReview();

            // Then
            result.Should().BeViewResult().WithViewName("Error/Forbidden");
            controller.Response.StatusCode.Should().Be(403);
        }
    }
}
