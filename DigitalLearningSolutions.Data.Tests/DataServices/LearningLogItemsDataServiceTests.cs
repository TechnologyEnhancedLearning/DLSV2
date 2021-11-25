namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class LearningLogItemsDataServiceTests
    {
        private CompetencyLearningResourcesTestHelper competencyLearningResourcesTestHelper = null!;
        private LearningLogItemsTestHelper learningLogItemsTestHelper = null!;
        private ILearningLogItemsDataService service = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            service = new LearningLogItemsDataService(connection);

            learningLogItemsTestHelper = new LearningLogItemsTestHelper(connection);
            competencyLearningResourcesTestHelper = new CompetencyLearningResourcesTestHelper(connection);
        }

        [Test]
        public void InsertLearningLogItem_inserts_expected_record()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const int competencyLearningResourceId = 1;
                const int delegateId = 2;
                const string resourceName = "Activity";
                const string resourceLink = "www.test.com";
                var addedDate = new DateTime(2021, 11, 1);
                competencyLearningResourcesTestHelper.InsertCompetencyLearningResource(
                    1,
                    competencyLearningResourceId,
                    1,
                    7
                );

                // When
                service.InsertLearningLogItem(
                    delegateId,
                    addedDate,
                    resourceName,
                    resourceLink,
                    competencyLearningResourceId
                );
                var result = learningLogItemsTestHelper.SelectLearningLogItemWithResourceLink(resourceLink);

                // Then
                using (new AssertionScope())
                {
                    result.Should().NotBeNull();
                    result!.LoggedById.Should().Be(delegateId);
                    result.LoggedDate.Should().Be(addedDate);
                    result.LinkedCompetencyLearningResourceId.Should().Be(competencyLearningResourceId);
                    result.ExternalUri.Should().Be(resourceLink);
                    result.Activity.Should().Be(resourceName);
                    result.ActivityType.Should().Be("Learning Hub Resource");
                    result.LearningLogItemId.Should().NotBe(0);
                    result.IcsGuid.Should().NotBeNull();

                    result.DueDate.Should().BeNull();
                    result.CompletedDate.Should().BeNull();
                    result.DurationMins.Should().Be(0);
                    result.Outcomes.Should().BeNull();
                    result.LinkedCustomisationId.Should().BeNull();
                    result.VerifiedById.Should().BeNull();
                    result.VerifierComments.Should().BeNull();
                    result.ArchivedDate.Should().BeNull();
                    result.ArchivedById.Should().BeNull();
                    result.LoggedByAdminId.Should().BeNull();
                    result.SeqInt.Should().BeNull();
                    result.LastAccessedDate.Should().BeNull();
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void InsertCandidateAssessmentLearningLogItem_inserts_expected_record()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const int assessmentId = 1;
                const int learningLogId = 2;

                // When
                service.InsertCandidateAssessmentLearningLogItem(assessmentId, learningLogId);
                var result = learningLogItemsTestHelper.SelectCandidateAssessmentLearningLogItem();

                // Then
                using (new AssertionScope())
                {
                    result.Should().NotBeNull();
                    result!.CandidateAssessmentId.Should().Be(assessmentId);
                    result.LearningLogItemId.Should().Be(learningLogId);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void InsertLearningLogItemCompetencies_inserts_expected_record()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const int competencyId = 1;
                const int learningLogId = 2;
                var associatedDate = new DateTime(2021, 11, 1);

                // When
                service.InsertLearningLogItemCompetencies(learningLogId, competencyId, associatedDate);
                var result = learningLogItemsTestHelper.SelectLearningLogItemCompetency();

                // Then
                using (new AssertionScope())
                {
                    result.Should().NotBeNull();
                    result!.CompetencyId.Should().Be(competencyId);
                    result.LearningLogItemId.Should().Be(learningLogId);
                    result.AssociatedDate.Should().Be(associatedDate);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}
