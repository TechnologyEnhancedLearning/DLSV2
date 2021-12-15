namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class LearningLogItemsDataServiceTests
    {
        private const int GenericLearningResourceReferenceId = 1;
        private const int GenericDelegateId = 2;
        private const string GenericActivityName = "generic activity";
        private const string GenericResourceLink = "generic resource link";
        private CompetencyLearningResourcesTestHelper competencyLearningResourcesTestHelper = null!;
        private LearningLogItemsTestHelper learningLogItemsTestHelper = null!;
        private ILearningLogItemsDataService service = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<LearningLogItemsDataService>>();

            service = new LearningLogItemsDataService(connection, logger);

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
                const int learningResourceReferenceId = 1;
                const int learningHubResourceReferenceId = 2;
                const int delegateId = 2;
                const string resourceName = "Activity";
                const string resourceLink = "www.test.com";
                var addedDate = new DateTime(2021, 11, 1);
                competencyLearningResourcesTestHelper.InsertLearningResourceReference(
                    learningResourceReferenceId,
                    learningHubResourceReferenceId,
                    7,
                    "Resource"
                );
                competencyLearningResourcesTestHelper.InsertCompetencyLearningResource(
                    1,
                    1,
                    learningResourceReferenceId,
                    7
                );

                // When
                service.InsertLearningLogItem(
                    delegateId,
                    addedDate,
                    resourceName,
                    resourceLink,
                    learningResourceReferenceId
                );
                var result = learningLogItemsTestHelper.SelectLearningLogItemWithResourceLink(resourceLink);

                // Then
                using (new AssertionScope())
                {
                    result.Should().NotBeNull();
                    AssertLearningLogItemHasCorrectValuesForLearningHubResource(
                        result!,
                        delegateId,
                        addedDate,
                        learningResourceReferenceId,
                        resourceName,
                        resourceLink
                    );
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

        [Test]
        public void GetLearningLogItems_returns_all_learning_hub_resource_log_items_for_delegate()
        {
            // Given
            const int learningResourceReferenceId = 1;
            const int learningHubResourceReferenceId = 2;
            const int delegateId = 2;
            const int differentDelegateId = 3;
            const string firstActivityName = "activity 1";
            const string secondActivityName = "activity 2";
            const string firstResourceLink = "link 1";
            const string secondResourceLink = "link 2";
            var addedDate = new DateTime(2021, 11, 1);

            using (new TransactionScope())
            {
                competencyLearningResourcesTestHelper.InsertLearningResourceReference(
                    learningResourceReferenceId,
                    learningHubResourceReferenceId,
                    7,
                    "Resource"
                );
                competencyLearningResourcesTestHelper.InsertCompetencyLearningResource(
                    1,
                    1,
                    learningResourceReferenceId,
                    7
                );
                service.InsertLearningLogItem(
                    delegateId,
                    addedDate,
                    firstActivityName,
                    firstResourceLink,
                    learningResourceReferenceId
                );
                service.InsertLearningLogItem(
                    delegateId,
                    addedDate,
                    secondActivityName,
                    secondResourceLink,
                    learningResourceReferenceId
                );
                service.InsertLearningLogItem(
                    differentDelegateId,
                    addedDate,
                    "activity 3",
                    "resource link 3",
                    learningResourceReferenceId
                );

                // When
                var result = service.GetLearningLogItems(delegateId).ToList();

                // Then
                using (new AssertionScope())
                {
                    result.Count.Should().Be(2);
                    AssertLearningLogItemHasCorrectValuesForLearningHubResource(
                        result[0],
                        delegateId,
                        addedDate,
                        learningResourceReferenceId,
                        firstActivityName,
                        firstResourceLink
                    );
                    AssertLearningLogItemHasCorrectValuesForLearningHubResource(
                        result[1],
                        delegateId,
                        addedDate,
                        learningResourceReferenceId,
                        secondActivityName,
                        secondResourceLink
                    );
                }
            }
        }

        [Test]
        public void GetLearningLogItem_returns_null_for_non_learning_hub_resources()
        {
            // When
            var result = service.GetLearningLogItem(2);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetLearningLogItem_returns_learning_log_item()
        {
            // Given
            var addedDate = new DateTime(2021, 11, 1);

            using (new TransactionScope())
            {
                var itemId = InsertLearningLogItem(
                    GenericDelegateId,
                    addedDate,
                    GenericLearningResourceReferenceId
                );

                // When
                var result = service.GetLearningLogItem(itemId);

                // Then
                using (new AssertionScope())
                {
                    result.Should().NotBeNull();
                    AssertLearningLogItemHasCorrectValuesForLearningHubResource(
                        result!,
                        GenericDelegateId,
                        addedDate,
                        GenericLearningResourceReferenceId,
                        GenericActivityName,
                        GenericResourceLink
                    );
                }
            }
        }

        [Test]
        public void UpdateLearningLogItemLastAccessedDate_should_set_date_correctly()
        {
            // Given
            var addedDate = new DateTime(2021, 11, 1);
            var updatedDate = new DateTime(2021, 11, 1);

            using (new TransactionScope())
            {
                var itemId = InsertLearningLogItem(
                    GenericDelegateId,
                    addedDate,
                    GenericLearningResourceReferenceId
                );

                // When
                service.UpdateLearningLogItemLastAccessedDate(itemId, updatedDate);
                var result = service.GetLearningLogItem(itemId);

                // Then
                using (new AssertionScope())
                {
                    result.Should().NotBeNull();
                    result!.LastAccessedDate.Should().Be(updatedDate);
                }
            }
        }

        [Test]
        public void Set_completed_date_should_update_db()
        {
            // Given
            var addedDate = new DateTime(2021, 11, 1);
            var newCompletedDate = new DateTime(2020, 7, 29);

            using (new TransactionScope())
            {
                var itemId = InsertLearningLogItem(
                    GenericDelegateId,
                    addedDate,
                    GenericLearningResourceReferenceId
                );

                // When
                service.SetCompletionDate(itemId, newCompletedDate);
                var modifiedItem = service.GetLearningLogItem(itemId);

                // Then
                using (new AssertionScope())
                {
                    modifiedItem!.CompletedDate.Should().Be(newCompletedDate);
                }
            }
        }

        [Test]
        public void RemoveLearningLogItem_correctly_sets_removed_date_and_removed_by_id()
        {
            // Given
            var addedDate = new DateTime(2021, 11, 1);
            var removedDate = new DateTime(2021, 12, 6);

            using (new TransactionScope())
            {
                var itemId = InsertLearningLogItem(
                    GenericDelegateId,
                    addedDate,
                    GenericLearningResourceReferenceId
                );

                // When
                service.RemoveLearningLogItem(itemId, GenericDelegateId, removedDate);
                var result = service.GetLearningLogItem(itemId);

                // Then
                using (new AssertionScope())
                {
                    result.Should().NotBeNull();
                    result!.ArchivedDate.Should().Be(removedDate);
                    result.ArchivedById.Should().Be(GenericDelegateId);
                }
            }
        }

        private int InsertLearningLogItem(
            int delegateId,
            DateTime addedDate,
            int learningResourceReferenceId
        )
        {
            competencyLearningResourcesTestHelper.InsertLearningResourceReference(
                learningResourceReferenceId,
                1,
                7,
                "Resource"
            );
            competencyLearningResourcesTestHelper.InsertCompetencyLearningResource(
                1,
                learningResourceReferenceId,
                1,
                7
            );
            return service.InsertLearningLogItem(
                delegateId,
                addedDate,
                GenericActivityName,
                GenericResourceLink,
                learningResourceReferenceId
            );
        }

        private void AssertLearningLogItemHasCorrectValuesForLearningHubResource(
            LearningLogItem item,
            int delegateId,
            DateTime addedDate,
            int learningResourceReferenceId,
            string resourceName,
            string resourceLink
        )
        {
            item.LoggedById.Should().Be(delegateId);
            item.LoggedDate.Should().Be(addedDate);
            item.LearningResourceReferenceId.Should().Be(learningResourceReferenceId);
            item.ExternalUri.Should().Be(resourceLink);
            item.Activity.Should().Be(resourceName);
            item.ActivityType.Should().Be("Learning Hub Resource");
            item.LearningLogItemId.Should().NotBe(0);
            item.IcsGuid.Should().NotBeNull();

            item.DueDate.Should().BeNull();
            item.CompletedDate.Should().BeNull();
            item.DurationMins.Should().Be(0);
            item.Outcomes.Should().BeNull();
            item.LinkedCustomisationId.Should().BeNull();
            item.VerifiedById.Should().BeNull();
            item.VerifierComments.Should().BeNull();
            item.ArchivedDate.Should().BeNull();
            item.ArchivedById.Should().BeNull();
            item.LoggedByAdminId.Should().BeNull();
            item.SeqInt.Should().BeNull();
            item.LastAccessedDate.Should().BeNull();
        }
    }
}
