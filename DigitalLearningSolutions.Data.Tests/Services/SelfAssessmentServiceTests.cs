namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using NUnit.Framework;
    using FluentAssertions;

    [Parallelizable(ParallelScope.Fixtures)]
    public class SelfAssessmentServiceTests
    {
        private SelfAssessmentService selfAssessmentService;
        private const int SelfAssessmentId = 1;
        private const int CandidateId = 254480;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            selfAssessmentService = new SelfAssessmentService(connection);
        }

        [Test]
        public void GetSelfAssessmentForCandidate_should_return_a_self_assessment()
        {
            // When
            var result = selfAssessmentService.GetSelfAssessmentForCandidate(CandidateId);

            // Then
            var expectedSelfAssessment = new SelfAssessment()
            {
                Id = SelfAssessmentId,
                Name = "Digital Capability Self Assessment",
                Description = "When thinking about your current role, for each of the following statements rate your current confidence level (Where are you now) and where your confidence leve ought to be to undertake your role successfully (Where do you need to be). Once you have submitted your ratings they will be used to recommend useful learning resources. We will also collect data anonymously to build up a picture of digital capability across the workforce to help with service design and learning provision."
            };

            result.Should().BeEquivalentTo(expectedSelfAssessment);
        }

        [Test]
        public void GetSelfAssessmentForCandidate_should_return_null_when_there_are_no_matching_assessments()
        {
            // When
            var result = selfAssessmentService.GetSelfAssessmentForCandidate(2);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_first_competency()
        {
            // Given
            var expectedCompetency = new Competency
            {
                Id = 1,
                CompetencyGroup = "Data, information and content",
                Description = "I can find, use and store information that exists in different digital locations e.g. on a PC, shared drives, via the internet",
                AssessmentQuestions =
                {
                    new AssessmentQuestion { Id = 1, MaxValueDescription = "Very confident", MinValueDescription = "Beginner", Question = "Where are you now" },
                    new AssessmentQuestion { Id = 2, MaxValueDescription = "Very confident", MinValueDescription = "Beginner", Question = "Where do you need to be" }
                }
            };

            // When
            var result = selfAssessmentService.GetNthCompetency(1, SelfAssessmentId, CandidateId);

            // Then
            result.Should().BeEquivalentTo(expectedCompetency);
        }

        [Test]
        public void GetNthCompetency_returns_last_competency()
        {
            // Given
            var expectedCompetency = new Competency
            {
                Id = 32,
                CompetencyGroup = "General questions",
                Description = "Taking an active role in my own learning is the most important thing that affects my digital literacy skills development",
                AssessmentQuestions =
                {
                    new AssessmentQuestion { Id = 3, MaxValueDescription = "Strongly agree", MinValueDescription = "Strongly disagree", Question = "To what extent to you agree" }
                }
            };

            // When
            var result = selfAssessmentService.GetNthCompetency(32, SelfAssessmentId, CandidateId);

            // Then
            result.Should().BeEquivalentTo(expectedCompetency);
        }

        [Test]
        public void GetNthCompetency_returns_null_when_reached_end_of_assessment()
        {
            // When
            var result = selfAssessmentService.GetNthCompetency(33, SelfAssessmentId, CandidateId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_null_when_n_zero()
        {
            // When
            var result = selfAssessmentService.GetNthCompetency(0, SelfAssessmentId, CandidateId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_null_when_n_negative()
        {
            // When
            var result = selfAssessmentService.GetNthCompetency(-1, SelfAssessmentId, CandidateId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_null_when_invalid_candidate()
        {
            // When
            var result = selfAssessmentService.GetNthCompetency(1, SelfAssessmentId, 1);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_null_when_invalid_assessment()
        {
            // When
            var result = selfAssessmentService.GetNthCompetency(1, 2, CandidateId);

            // Then
            result.Should().BeNull();
        }
    }
}
