namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using NUnit.Framework;

    public class SelfAssessmentServiceTests
    {
        private ISelfAssessmentDataService selfAssessmentDataService = null!;
        private ISelfAssessmentService selfAssessmentService = null!;

        [SetUp]
        public void Setup()
        {
            selfAssessmentDataService = A.Fake<ISelfAssessmentDataService>();
            selfAssessmentService = new SelfAssessmentService(selfAssessmentDataService);
        }

        [Test]
        public void CanDelegateAccessSelfAssessment_returns_true_with_no_completed_or_removed_assessments()
        {
            // Given
            var candidateAssessments = Builder<CandidateAssessment>.CreateListOfSize(5).All()
                .With(ca => ca.CompletedDate = null)
                .With(ca => ca.RemovedDate = null)
                .Build().ToList();
            A.CallTo(() => selfAssessmentDataService.GetCandidateAssessments(A<int>._, A<int>._))
                .Returns(candidateAssessments);

            // When
            var result = selfAssessmentService.CanDelegateAccessSelfAssessment(1, 1);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void CanDelegateAccessSelfAssessment_returns_true_with_at_least_one_valid_assessment()
        {
            // Given
            var candidateAssessments = Builder<CandidateAssessment>.CreateListOfSize(5)
                .TheFirst(2)
                .With(ca => ca.RemovedDate = DateTime.UtcNow)
                .TheNext(2)
                .With(ca => ca.CompletedDate = DateTime.UtcNow)
                .TheRest()
                .With(ca => ca.CompletedDate = null)
                .With(ca => ca.RemovedDate = null)
                .Build().ToList();
            A.CallTo(() => selfAssessmentDataService.GetCandidateAssessments(A<int>._, A<int>._))
                .Returns(candidateAssessments);

            // When
            var result = selfAssessmentService.CanDelegateAccessSelfAssessment(1, 1);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void CanDelegateAccessSelfAssessment_returns_false_with_only_completed_or_removed_assessments()
        {
            // Given
            var candidateAssessments = Builder<CandidateAssessment>.CreateListOfSize(5)
                .TheFirst(3)
                .With(ca => ca.RemovedDate = DateTime.UtcNow)
                .TheRest()
                .With(ca => ca.CompletedDate = DateTime.UtcNow)
                .Build().ToList();
            A.CallTo(() => selfAssessmentDataService.GetCandidateAssessments(A<int>._, A<int>._))
                .Returns(candidateAssessments);

            // When
            var result = selfAssessmentService.CanDelegateAccessSelfAssessment(1, 1);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void CanDelegateAccessSelfAssessment_returns_false_no_assessments()
        {
            // Given
            A.CallTo(() => selfAssessmentDataService.GetCandidateAssessments(A<int>._, A<int>._))
                .Returns(new List<CandidateAssessment>());

            // When
            var result = selfAssessmentService.CanDelegateAccessSelfAssessment(1, 1);

            // Then
            result.Should().BeFalse();
        }
    }
}
