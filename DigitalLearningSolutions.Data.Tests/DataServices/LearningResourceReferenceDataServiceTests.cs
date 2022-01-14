namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class LearningResourceReferenceDataServiceTests
    {
        private ILearningResourceReferenceDataService service = null!;
        private CompetencyLearningResourcesTestHelper testHelper = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            service = new LearningResourceReferenceDataService(connection);

            testHelper = new CompetencyLearningResourcesTestHelper(connection);
        }

        [Test]
        public void GetLearningHubResourceReferenceById_gets_expected_record()
        {
            using var transaction = new TransactionScope();

            // Given
            const int learningResourceReferenceId = 1;
            const int learningHubResourceRefId = 10;
            testHelper.InsertLearningResourceReference(
                learningResourceReferenceId,
                learningHubResourceRefId,
                7,
                "Resource"
            );

            // When
            var result = service.GetLearningHubResourceReferenceById(learningResourceReferenceId);

            // Then
            result.Should().Be(learningHubResourceRefId);
        }

        [Test]
        public void GetLearningHubResourceLinkById_gets_expected_record()
        {
            using var transaction = new TransactionScope();

            // Given
            const int learningResourceReferenceId = 1;
            const string learningHubResourceLink = "www.example.com";
            testHelper.InsertLearningResourceReference(
                learningResourceReferenceId,
                10,
                7,
                "Resource",
                learningHubResourceLink
            );

            // When
            var result = service.GetLearningHubResourceLinkById(learningResourceReferenceId);

            // Then
            result.Should().Be(learningHubResourceLink);
        }
    }
}
