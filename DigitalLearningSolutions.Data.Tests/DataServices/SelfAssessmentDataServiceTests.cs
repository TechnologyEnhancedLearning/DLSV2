namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class SelfAssessmentDataServiceTests
    {
        private ISelfAssessmentDataService service = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<SelfAssessmentDataService>>();
            service = new SelfAssessmentDataService(connection, logger);
        }

        [Test]
        public void GetCompetencyIdsForSelfAssessment_returns_expected_ids()
        {
            // Given
            var expectedIds = Enumerable.Range(1, 32).ToList();

            // When
            var result = service.GetCompetencyIdsForSelfAssessment(1).ToList();

            // Then
            result.Should().BeEquivalentTo(expectedIds);
        }
    }
}
