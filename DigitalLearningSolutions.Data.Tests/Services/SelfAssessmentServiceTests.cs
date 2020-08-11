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
            var result = selfAssessmentService.GetSelfAssessmentForCandidate(254480);

            // Then
            var expectedSelfAssessment = new SelfAssessment()
            {
                Id = 1,
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
    }
}
