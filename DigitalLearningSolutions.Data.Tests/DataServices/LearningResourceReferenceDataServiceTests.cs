namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
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
        public void GetResourceReferenceDetailsByReferenceIds_gets_expected_records()
        {
                using var transaction = new TransactionScope();

            // Given
            var resourceReferenceIds = new[] { 1, 2, 3 };
            var expectedFirstResourceDetails = new LearningResourceReference
            {
                Id = 1,
                ResourceRefId = 2,
                AdminId = 7,
                OriginalResourceName = "Resource 1",
                ResourceLink = "www.resource1.com",
                OriginalDescription = "description 1",
                OriginalResourceType = "resource type 1",
                OriginalCatalogueName = "catalogue 1",
                OriginalRating = 1,
            };
            testHelper.InsertLearningResourceReference(
                expectedFirstResourceDetails.Id,
                expectedFirstResourceDetails.ResourceRefId,
                expectedFirstResourceDetails.AdminId,
                expectedFirstResourceDetails.OriginalResourceName,
                expectedFirstResourceDetails.ResourceLink,
                expectedFirstResourceDetails.OriginalDescription,
                expectedFirstResourceDetails.OriginalResourceType,
                expectedFirstResourceDetails.OriginalCatalogueName,
                expectedFirstResourceDetails.OriginalRating
            );
            testHelper.InsertLearningResourceReference(
                2,
                3,
                7,
                "Resource 2"
            );

            // When
            var result = service.GetResourceReferenceDetailsByReferenceIds(resourceReferenceIds).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().HaveCount(2);
                result.First().RefId.Should().Be(expectedFirstResourceDetails.ResourceRefId);
                result.First().Title.Should().Be(expectedFirstResourceDetails.OriginalResourceName);
                result.First().Link.Should().Be(expectedFirstResourceDetails.ResourceLink);
                result.First().Description.Should().Be(expectedFirstResourceDetails.OriginalDescription);
                result.First().ResourceType.Should().Be(expectedFirstResourceDetails.OriginalResourceType);
                result.First().Catalogue.Name.Should().Be(expectedFirstResourceDetails.OriginalCatalogueName);
                result.First().Rating.Should().Be(expectedFirstResourceDetails.OriginalRating);
                result[1].RefId.Should().Be(3);
            }
        }
    }
}
