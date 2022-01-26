namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
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
        public void GetCompetencyIdsLinkedToResource_returns_expected_ids_from_active_records_only()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                var expectedIds = new[] { 1, 2, 3, 5, 6 };
                InsertCompetencyLearningResources();
                testHelper.InsertCompetencyLearningResource(7, 7, 2, 7, DateTime.UtcNow, 7);

                // When
                var result = service.GetCompetencyIdsLinkedToResource(2);

                // Then
                result.Should().BeEquivalentTo(expectedIds);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetActiveCompetencyLearningResourcesByCompetencyId_returns_expected_active_records()
        {
            using var transaction = new TransactionScope();

            // Given
            InsertCompetencyLearningResources();
            testHelper.InsertCompetencyLearningResource(7, 1, 3, 7, DateTime.UtcNow, 7);

            var expectedItem = new CompetencyLearningResource
            {
                Id = 1,
                CompetencyId = 1,
                LearningResourceReferenceId = 2,
                AdminId = 7,
                LearningHubResourceReferenceId = 2,
            };

            // When
            var result = service.GetActiveCompetencyLearningResourcesByCompetencyId(1).ToList();

            // Then
            result.Should().HaveCount(1);
            result.Should().ContainEquivalentOf(expectedItem);
        }

        [Test]
        public void GetCompetencyResourceAssessmentQuestionParameters_returns_expected_results()
        {
            using (new TransactionScope())
            {
                var adminId = UserTestHelper.GetDefaultAdminUser().Id;

                testHelper.InsertLearningResourceReference(2, 2, adminId, "Resource 2");
                testHelper.InsertCompetencyLearningResource(1, 1, 2, adminId);
                testHelper.InsertCompetencyResourceAssessmentQuestionParameters(1, 1, true, 2, false, 1, 10);
                var expectedItem = new CompetencyResourceAssessmentQuestionParameter
                {
                    CompetencyLearningResourceId = 1,
                    AssessmentQuestionId = 1,
                    Essential = true,
                    RelevanceAssessmentQuestionId = 2,
                    CompareToRoleRequirements = false,
                    MinResultMatch = 1,
                    MaxResultMatch = 10,
                };

                // When
                var result = service.GetCompetencyResourceAssessmentQuestionParameters(new[] { 1 }).ToList();

                // Then
                result.Should().HaveCount(1);
                result.Should().ContainEquivalentOf(expectedItem);
            }
        }

        private void InsertCompetencyLearningResources()
        {
            var adminId = UserTestHelper.GetDefaultAdminUser().Id;

            testHelper.InsertLearningResourceReference(2, 2, adminId, "Resource 2");
            testHelper.InsertLearningResourceReference(3, 3, adminId, "Resource 3");

            testHelper.InsertCompetencyLearningResource(1, 1, 2, adminId);
            testHelper.InsertCompetencyLearningResource(2, 2, 2, adminId);
            testHelper.InsertCompetencyLearningResource(3, 3, 2, adminId);
            testHelper.InsertCompetencyLearningResource(4, 4, 3, adminId);
            testHelper.InsertCompetencyLearningResource(5, 5, 2, adminId);
            testHelper.InsertCompetencyLearningResource(6, 6, 2, adminId);
        }
    }
}
