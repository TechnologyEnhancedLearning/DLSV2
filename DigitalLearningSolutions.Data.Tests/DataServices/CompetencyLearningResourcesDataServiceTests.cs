namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class CompetencyLearningResourcesDataServiceTests
    {
        private ICompetencyLearningResourcesDataService service = null!;
        private CompetencyLearningResourcesTestHelper testHelper = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            service = new CompetencyLearningResourcesDataService(connection);

            testHelper = new CompetencyLearningResourcesTestHelper(connection);
        }

        [Test]
        public void GetCompetencyLearningResourceById_returns_expected_resource()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                var expectedResource = new CompetencyLearningResource
                {
                    Id = 1,
                    CompetencyId = 1,
                    LearningHubResourceReferenceId = 2,
                    AdminId = 7,
                };
                InsertCompetencyLearningResources();

                // When
                var result = service.GetCompetencyLearningResourceById(1);

                // Then
                result.Should().BeEquivalentTo(expectedResource);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetCompetencyIdsByLearningHubResourceReference_returns_expected_ids()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                var expectedIds = new[] { 1, 2, 3, 5, 6 };
                InsertCompetencyLearningResources();

                // When
                var result = service.GetCompetencyIdsByLearningHubResourceReference(2);

                // Then
                result.Should().BeEquivalentTo(expectedIds);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        private void InsertCompetencyLearningResources()
        {
            var adminId = UserTestHelper.GetDefaultAdminUser().Id;
            testHelper.InsertCompetencyLearningResource(1, 1, 2, adminId);
            testHelper.InsertCompetencyLearningResource(2, 2, 2, adminId);
            testHelper.InsertCompetencyLearningResource(3, 3, 2, adminId);
            testHelper.InsertCompetencyLearningResource(4, 4, 3, adminId);
            testHelper.InsertCompetencyLearningResource(5, 5, 2, adminId);
            testHelper.InsertCompetencyLearningResource(6, 6, 2, adminId);
        }
    }
}
