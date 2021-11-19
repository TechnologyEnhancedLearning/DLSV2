namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class SelfAssessmentDataServiceTests
    {
        private ISelfAssessmentDataService service = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            service = new SelfAssessmentDataService(connection);
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
