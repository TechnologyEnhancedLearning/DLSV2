﻿namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class ActionPlanServiceTests
    {
        private IActionPlanService actionPlanService = null!;
        private IClockService clockService = null!;
        private ICompetencyLearningResourcesDataService competencyLearningResourcesDataService = null!;
        private IConfiguration config = null!;
        private Catalogue genericCatalogue = null!;
        private ILearningHubApiClient learningHubApiClient = null!;
        private ILearningHubApiService learningHubApiService = null!;
        private ILearningLogItemsDataService learningLogItemsDataService = null!;
        private ISelfAssessmentDataService selfAssessmentDataService = null!;

        [SetUp]
        public void Setup()
        {
            genericCatalogue = Builder<Catalogue>.CreateNew().With(c => c.Name = "Generic catalogue").Build();
            clockService = A.Fake<IClockService>();
            competencyLearningResourcesDataService = A.Fake<ICompetencyLearningResourcesDataService>();
            learningLogItemsDataService = A.Fake<ILearningLogItemsDataService>();
            learningHubApiService = A.Fake<ILearningHubApiService>();
            learningHubApiClient = A.Fake<ILearningHubApiClient>();
            selfAssessmentDataService = A.Fake<ISelfAssessmentDataService>();
            config = A.Fake<IConfiguration>();

            actionPlanService = new ActionPlanService(
                competencyLearningResourcesDataService,
                learningLogItemsDataService,
                clockService,
                learningHubApiService,
                learningHubApiClient,
                selfAssessmentDataService,
                config
            );
        }

        [Test]
        public void AddResourceToActionPlan_calls_expected_insert_data_service_methods()
        {
            // Given
            const int competencyLearningResourceId = 1;
            const int delegateId = 2;
            const int selfAssessmentId = 3;
            const string resourceName = "Activity";
            const string resourceLink = "www.test.com";
            const int learningLogId = 4;
            const int learningHubResourceId = 6;

            var addedDate = new DateTime(2021, 11, 1);
            A.CallTo(() => clockService.UtcNow).Returns(addedDate);

            var competencyLearningResource = new CompetencyLearningResource
            {
                LearningHubResourceReferenceId = learningHubResourceId,
            };
            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyLearningResourceById(
                    competencyLearningResourceId
                )
            ).Returns(competencyLearningResource);

            A.CallTo(() => learningHubApiService.GetResourceNameAndLink(learningHubResourceId))
                .Returns((resourceName, resourceLink));

            var resourceCompetencies = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyIdsByLearningHubResourceReference(
                    learningHubResourceId
                )
            ).Returns(resourceCompetencies);

            var assessmentCompetencies = new[] { 2, 3, 5, 6, 8, 9, 10 };
            A.CallTo(() => selfAssessmentDataService.GetCompetencyIdsForSelfAssessment(selfAssessmentId))
                .Returns(assessmentCompetencies);

            A.CallTo(
                () => learningLogItemsDataService.InsertLearningLogItem(
                    A<int>._,
                    A<DateTime>._,
                    A<string>._,
                    A<string>._,
                    A<int>._
                )
            ).Returns(learningLogId);

            var expectedMatchingCompetencies = new[] { 2, 3, 5, 6, 8 };

            // When
            actionPlanService.AddResourceToActionPlan(competencyLearningResourceId, delegateId, selfAssessmentId);

            // Then
            A.CallTo(
                () => learningLogItemsDataService.InsertLearningLogItem(
                    delegateId,
                    addedDate,
                    resourceName,
                    resourceLink,
                    competencyLearningResourceId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => learningLogItemsDataService.InsertCandidateAssessmentLearningLogItem(
                    selfAssessmentId,
                    learningLogId
                )
            ).MustHaveHappenedOnceExactly();

            foreach (var competencyId in expectedMatchingCompetencies)
            {
                A.CallTo(
                    () => learningLogItemsDataService.InsertLearningLogItemCompetencies(
                        learningLogId,
                        competencyId,
                        addedDate
                    )
                ).MustHaveHappenedOnceExactly();
            }

            A.CallTo(
                () => learningLogItemsDataService.InsertLearningLogItemCompetencies(learningLogId, A<int>._, addedDate)
            ).MustHaveHappened(5, Times.Exactly);
        }

        [Test]
        public async Task GetIncompleteActionPlanItems_returns_empty_list_if_signposting_is_disabled()
        {
            // Given
            const int delegateId = 1;
            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("false");

            // When
            var result = await actionPlanService.GetIncompleteActionPlanItems(delegateId);

            // Then
            result.Should().BeEmpty();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(A<int>._))
                .MustNotHaveHappened();
            A.CallTo(() => learningHubApiClient.GetBulkResourcesByReferenceIds(A<IEnumerable<int>>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task GetIncompleteActionPlanItems_returns_empty_list_if_no_incomplete_learning_log_items_found()
        {
            // Given
            const int delegateId = 1;
            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("true");
            var invalidLearningLogItems = Builder<LearningLogItem>.CreateListOfSize(3)
                .All().With(i => i.CompletedDate = null).And(i => i.ArchivedDate = null)
                .And(i => i.LearningHubResourceReferenceId = 1)
                .TheFirst(1).With(i => i.Activity = "completed").And(i => i.CompletedDate = DateTime.UtcNow)
                .TheNext(1).With(i => i.Activity = "removed").And(i => i.ArchivedDate = DateTime.UtcNow)
                .TheNext(1).With(i => i.Activity = "no resource link").And(i => i.LearningHubResourceReferenceId = null)
                .Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(delegateId))
                .Returns(invalidLearningLogItems);

            // When
            var result = await actionPlanService.GetIncompleteActionPlanItems(delegateId);

            // Then
            result.Should().BeEmpty();
            A.CallTo(() => learningHubApiClient.GetBulkResourcesByReferenceIds(A<IEnumerable<int>>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task GetIncompleteActionPlanItems_returns_correctly_matched_action_plan_items()
        {
            // Given
            const int delegateId = 1;
            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("true");
            var learningLogIds = new List<int> { 4, 5, 6, 7, 8 };
            var learningResourceIds = new List<int> { 15, 21, 33, 48, 51 };
            var learningLogItems = Builder<LearningLogItem>.CreateListOfSize(5).All()
                .With(i => i.CompletedDate = null)
                .And(i => i.ArchivedDate = null)
                .And((i, index) => i.LearningHubResourceReferenceId = learningResourceIds[index])
                .And((i, index) => i.LearningLogItemId = learningLogIds[index])
                .Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(delegateId))
                .Returns(learningLogItems);
            var matchedResources = Builder<ResourceReferenceWithResourceDetails>.CreateListOfSize(3).All()
                .With((r, index) => r.RefId = learningResourceIds[index])
                .And((r, index) => r.Title = $"Title {learningResourceIds[index]}")
                .And(r => r.Catalogue = genericCatalogue).Build().ToList();
            var unmatchedResourceReferences = new List<int>
            {
                learningResourceIds[3],
                learningResourceIds[4],
            };
            var bulkReturnedItems = new BulkResourceReferences
            {
                ResourceReferences = matchedResources,
                UnmatchedResourceReferenceIds = unmatchedResourceReferences,
            };
            A.CallTo(() => learningHubApiClient.GetBulkResourcesByReferenceIds(A<IEnumerable<int>>._))
                .Returns(bulkReturnedItems);

            // When
            var result = await actionPlanService.GetIncompleteActionPlanItems(delegateId);

            // Then
            List<(int id, string title)> resultIdsAndTitles = result.Select(r => (r.Id, r.Name)).ToList();
            using (new AssertionScope())
            {
                resultIdsAndTitles.Count.Should().Be(3);
                resultIdsAndTitles[0].Should().Be((4, "Title 15"));
                resultIdsAndTitles[1].Should().Be((5, "Title 21"));
                resultIdsAndTitles[2].Should().Be((6, "Title 33"));
                A.CallTo(
                        () => learningHubApiClient.GetBulkResourcesByReferenceIds(
                            A<IEnumerable<int>>.That.IsSameSequenceAs(learningResourceIds)
                        )
                    )
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public async Task GetLearningResourceLinkAndUpdateLastAccessedDate_returns_null_if_signposting_is_disabled()
        {
            // Given
            const int learningLogItemId = 1;
            const int delegateId = 2;
            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("false");

            // When
            var result =
                await actionPlanService.GetLearningResourceLinkAndUpdateLastAccessedDate(learningLogItemId, delegateId);

            // Then
            result.Should().BeNull();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(A<int>._))
                .MustNotHaveHappened();
            A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task
            GetLearningResourceLinkAndUpdateLastAccessedDate_returns_null_if_delegate_does_not_have_active_action_plan_item_with_matching_id()
        {
            // Given
            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("true");
            const int learningLogItemId = 1;
            const int delegateId = 2;
            var learningLogIds = new List<int> { learningLogItemId, 5, 6, 7, learningLogItemId };
            var learningResourceIds = new List<int?> { 15, 21, 33, 48, null };
            var learningLogItems = Builder<LearningLogItem>.CreateListOfSize(5).All()
                .With(i => i.CompletedDate = null)
                .And(i => i.ArchivedDate = null)
                .And((i, index) => i.LearningHubResourceReferenceId = learningResourceIds[index])
                .And((i, index) => i.LearningLogItemId = learningLogIds[index])
                .TheFirst(1).With(i => i.ArchivedDate = DateTime.UtcNow)
                .Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(delegateId))
                .Returns(learningLogItems);

            // When
            var result =
                await actionPlanService.GetLearningResourceLinkAndUpdateLastAccessedDate(learningLogItemId, delegateId);

            // Then
            result.Should().BeNull();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(delegateId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task GetLearningResourceLinkAndUpdateLastAccessedDate_updates_last_accessed_returns_resource_link()
        {
            // Given
            var testDate = new DateTime(2021, 12, 2);
            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("true");
            A.CallTo(() => clockService.UtcNow).Returns(testDate);
            const int learningLogItemId = 1;
            const int delegateId = 2;
            const int resourceReferenceId = 3;
            const string resourceLink = "www.test.com";
            var learningLogItems = Builder<LearningLogItem>.CreateListOfSize(1).All()
                .With(i => i.CompletedDate = null)
                .And(i => i.ArchivedDate = null)
                .And(i => i.LearningHubResourceReferenceId = resourceReferenceId)
                .And(i => i.LearningLogItemId = learningLogItemId)
                .Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(delegateId))
                .Returns(learningLogItems);
            var matchedResource = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = resourceReferenceId)
                .And(r => r.Title = "Title")
                .And(r => r.Catalogue = genericCatalogue)
                .And(r => r.Link = resourceLink).Build();
            A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(resourceReferenceId))
                .Returns(matchedResource);

            // When
            var result =
                await actionPlanService.GetLearningResourceLinkAndUpdateLastAccessedDate(learningLogItemId, delegateId);

            // Then
            result.Should().Be(resourceLink);
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(delegateId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(
                    () => learningLogItemsDataService.UpdateLearningLogItemLastAccessedDate(learningLogItemId, testDate)
                )
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(resourceReferenceId))
                .MustHaveHappenedOnceExactly();
        }
    }
}
