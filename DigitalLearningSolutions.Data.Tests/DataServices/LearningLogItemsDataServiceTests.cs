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
                    AssertLearningLogItemHasCorrectValuesForLearningHubResource(
                        result!,
                        delegateId,
                        addedDate,
                        competencyLearningResourceId,
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
            const int competencyLearningResourceId = 1;
            const int delegateId = 2;
            const int differentDelegateId = 3;
            const string firstActivityName = "activity 1";
            const string secondActivityName = "activity 2";
            const string firstResourceLink = "link 1";
            const string secondResourceLink = "link 2";
            var addedDate = new DateTime(2021, 11, 1);

            using var transaction = new TransactionScope();
            try
            {
                competencyLearningResourcesTestHelper.InsertCompetencyLearningResource(
                    1,
                    competencyLearningResourceId,
                    1,
                    7
                );
                service.InsertLearningLogItem(
                    delegateId,
                    addedDate,
                    firstActivityName,
                    firstResourceLink,
                    competencyLearningResourceId
                );
                service.InsertLearningLogItem(
                    delegateId,
                    addedDate,
                    secondActivityName,
                    secondResourceLink,
                    competencyLearningResourceId
                );
                service.InsertLearningLogItem(
                    differentDelegateId,
                    addedDate,
                    "activity 3",
                    "resource link 3",
                    competencyLearningResourceId
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
                        competencyLearningResourceId,
                        firstActivityName,
                        firstResourceLink
                    );
                    AssertLearningLogItemHasCorrectValuesForLearningHubResource(
                        result[1],
                        delegateId,
                        addedDate,
                        competencyLearningResourceId,
                        secondActivityName,
                        secondResourceLink
                    );
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void UpdateLearningLogItemLastAccessedDate_should_set_date_correctly()
        {
            // Given
            const int learningLogItemId = 2;
            var testDate = new DateTime(2021, 11, 1);

            using var transaction = new TransactionScope();
            try
            {
                // When
                service.UpdateLearningLogItemLastAccessedDate(learningLogItemId, testDate);
                var result = learningLogItemsTestHelper.SelectLearningLogItemById(learningLogItemId);

                // Then
                using (new AssertionScope())
                {
                    result.Should().NotBeNull();
                    result!.LastAccessedDate.Should().Be(testDate);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        private void AssertLearningLogItemHasCorrectValuesForLearningHubResource(
            LearningLogItem item,
            int delegateId,
            DateTime addedDate,
            int competencyLearningResourceId,
            string resourceName,
            string resourceLink
        )
        {
            item.LoggedById.Should().Be(delegateId);
            item.LoggedDate.Should().Be(addedDate);
            item.LinkedCompetencyLearningResourceId.Should().Be(competencyLearningResourceId);
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
