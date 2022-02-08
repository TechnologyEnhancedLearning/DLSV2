namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
        private const int GenericLearningLogItemId = 1;
        private const int GenericDelegateId = 2;
        private const int GenericLearningHubResourceReferenceId = 3;
        private const int GenericLearningResourceReferenceId = 33;

        private IActionPlanService actionPlanService = null!;
        private IClockService clockService = null!;
        private ICompetencyLearningResourcesDataService competencyLearningResourcesDataService = null!;
        private IConfiguration config = null!;
        private Catalogue genericCatalogue = null!;
        private ILearningHubResourceService learningHubResourceService = null!;
        private ILearningLogItemsDataService learningLogItemsDataService = null!;
        private ILearningResourceReferenceDataService learningResourceReferenceDataService = null!;
        private ISelfAssessmentDataService selfAssessmentDataService = null!;

        [SetUp]
        public void Setup()
        {
            genericCatalogue = Builder<Catalogue>.CreateNew().With(c => c.Name = "Generic catalogue").Build();
            clockService = A.Fake<IClockService>();
            competencyLearningResourcesDataService = A.Fake<ICompetencyLearningResourcesDataService>();
            learningLogItemsDataService = A.Fake<ILearningLogItemsDataService>();
            learningHubResourceService = A.Fake<ILearningHubResourceService>();
            selfAssessmentDataService = A.Fake<ISelfAssessmentDataService>();
            learningResourceReferenceDataService = A.Fake<ILearningResourceReferenceDataService>();
            config = A.Fake<IConfiguration>();

            actionPlanService = new ActionPlanService(
                competencyLearningResourcesDataService,
                learningLogItemsDataService,
                clockService,
                learningHubResourceService,
                selfAssessmentDataService,
                config,
                learningResourceReferenceDataService
            );
        }

        [Test]
        public void AddResourceToActionPlan_calls_expected_insert_data_service_methods()
        {
            // Given
            const int learningResourceReferenceId = 1;
            const int delegateId = 2;
            const int selfAssessmentId = 3;
            const string resourceName = "Activity";
            const string resourceLink = "www.test.com";
            const int learningLogId = 4;
            const int learningHubResourceId = 6;

            var addedDate = new DateTime(2021, 11, 1);
            A.CallTo(() => clockService.UtcNow).Returns(addedDate);

            A.CallTo(
                () => learningResourceReferenceDataService.GetLearningHubResourceReferenceById(
                    learningResourceReferenceId
                )
            ).Returns(learningHubResourceId);

            var resource = new ResourceReferenceWithResourceDetails { Title = resourceName, Link = resourceLink };
            A.CallTo(() => learningHubResourceService.GetResourceByReferenceId(learningHubResourceId))
                .Returns((resource, true));

            var resourceCompetencies = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyIdsLinkedToResource(
                    learningResourceReferenceId
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
            actionPlanService.AddResourceToActionPlan(learningResourceReferenceId, delegateId, selfAssessmentId);

            // Then
            A.CallTo(
                () => learningLogItemsDataService.InsertLearningLogItem(
                    delegateId,
                    addedDate,
                    resourceName,
                    resourceLink,
                    learningResourceReferenceId
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
        public async Task
            GetIncompleteActionPlanResources_returns_empty_list_if_no_incomplete_learning_log_items_found()
        {
            // Given
            const int delegateId = 1;
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
            var result = await actionPlanService.GetIncompleteActionPlanResources(delegateId);

            // Then
            result.resources.Should().BeEmpty();
            result.apiIsAccessible.Should().BeTrue();
            A.CallTo(() => learningHubResourceService.GetBulkResourcesByReferenceIds(A<List<int>>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task GetIncompleteActionPlanResources_returns_correctly_matched_action_plan_items()
        {
            // Given
            const int delegateId = 1;
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
            GivenLearningHubResourceServiceBulkResponseReturnsExpectedResources(learningResourceIds);

            // When
            var result = await actionPlanService.GetIncompleteActionPlanResources(delegateId);

            // Then
            result.apiIsAccessible.Should().BeFalse();
            AssertThatActionPlanResourceIdsAndTitlesAreCorrect(result.resources, learningResourceIds);
        }

        [Test]
        public async Task GetCompletedActionPlanResources_returns_empty_list_if_no_completed_learning_log_items_found()
        {
            // Given
            const int delegateId = 1;
            var invalidLearningLogItems = Builder<LearningLogItem>.CreateListOfSize(3)
                .All().With(i => i.CompletedDate = DateTime.UtcNow).And(i => i.ArchivedDate = null)
                .And(i => i.LearningHubResourceReferenceId = 1)
                .TheFirst(1).With(i => i.Activity = "incomplete").And(i => i.CompletedDate = null)
                .TheNext(1).With(i => i.Activity = "removed").And(i => i.ArchivedDate = DateTime.UtcNow)
                .TheNext(1).With(i => i.Activity = "no resource link").And(i => i.LearningHubResourceReferenceId = null)
                .Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(delegateId))
                .Returns(invalidLearningLogItems);

            // When
            var result = await actionPlanService.GetCompletedActionPlanResources(delegateId);

            // Then
            result.resources.Should().BeEmpty();
            result.apiIsAccessible.Should().BeTrue();
            A.CallTo(() => learningHubResourceService.GetBulkResourcesByReferenceIds(A<List<int>>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task GetCompleteActionPlanResources_returns_correctly_matched_action_plan_items()
        {
            // Given
            const int delegateId = 1;
            var learningLogIds = new List<int> { 4, 5, 6, 7, 8 };
            var learningResourceIds = new List<int> { 15, 21, 33, 48, 51 };
            var learningLogItems = Builder<LearningLogItem>.CreateListOfSize(5).All()
                .With(i => i.CompletedDate = DateTime.UtcNow)
                .And(i => i.ArchivedDate = null)
                .And((i, index) => i.LearningHubResourceReferenceId = learningResourceIds[index])
                .And((i, index) => i.LearningLogItemId = learningLogIds[index])
                .Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(delegateId))
                .Returns(learningLogItems);
            GivenLearningHubResourceServiceBulkResponseReturnsExpectedResources(learningResourceIds);

            // When
            var result = await actionPlanService.GetCompletedActionPlanResources(delegateId);

            // Then
            result.apiIsAccessible.Should().BeFalse();
            AssertThatActionPlanResourceIdsAndTitlesAreCorrect(result.resources, learningResourceIds);
        }

        [Test]
        public async Task
            GetCompleteActionPlanResources_returns_correctly_matched_action_plan_items_with_repeated_resource()
        {
            // Given
            const int delegateId = 1;
            var learningLogIds = new List<int> { 4, 5, 6 };
            var learningResourceIds = new List<int> { 15, 26, 15 };
            var expectedLearningResourceIdsUsedInApiCall = new List<int> { 15, 26 };
            var learningLogItems = Builder<LearningLogItem>.CreateListOfSize(3).All()
                .With(i => i.CompletedDate = DateTime.UtcNow)
                .And(i => i.ArchivedDate = null)
                .And((i, index) => i.LearningHubResourceReferenceId = learningResourceIds[index])
                .And((i, index) => i.LearningLogItemId = learningLogIds[index])
                .Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(delegateId))
                .Returns(learningLogItems);
            var matchedResources = Builder<ResourceReferenceWithResourceDetails>.CreateListOfSize(2).All()
                .With((r, index) => r.RefId = learningResourceIds[index])
                .And((r, index) => r.Title = $"Title {learningResourceIds[index]}")
                .And(r => r.Catalogue = genericCatalogue).Build().ToList();
            var unmatchedResourceReferences = new List<int>();
            var bulkReturnedItems = new BulkResourceReferences
            {
                ResourceReferences = matchedResources,
                UnmatchedResourceReferenceIds = unmatchedResourceReferences,
            };
            A.CallTo(
                    () => learningHubResourceService
                        .GetBulkResourcesByReferenceIdsAndPopulateDeletedDetailsFromDatabase(A<List<int>>._)
                )
                .Returns((bulkReturnedItems, false));

            // When
            var result = await actionPlanService.GetCompletedActionPlanResources(delegateId);

            // Then
            List<(int id, string title)> resultIdsAndTitles = result.resources.Select(r => (r.Id, r.Name)).ToList();
            using (new AssertionScope())
            {
                resultIdsAndTitles.Count.Should().Be(3);
                resultIdsAndTitles[0].Should().Be((4, "Title 15"));
                resultIdsAndTitles[1].Should().Be((5, "Title 26"));
                resultIdsAndTitles[2].Should().Be((6, "Title 15"));
                A.CallTo(
                        () => learningHubResourceService
                            .GetBulkResourcesByReferenceIdsAndPopulateDeletedDetailsFromDatabase(
                                A<List<int>>.That.IsSameSequenceAs(expectedLearningResourceIdsUsedInApiCall)
                            )
                    )
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void
            UpdateActionPlanResourcesLastAccessedDateIfPresent_updates_last_accessed_date_of_appropriate_records()
        {
            // Given
            var testDate = new DateTime(2021, 12, 2);
            A.CallTo(() => clockService.UtcNow).Returns(testDate);
            const int delegateId = 2;
            const int resourceReferenceId = 3;
            var expectedLearningLogItemIdsToUpdate = new[] { 1, 4 };
            var learningLogItems = Builder<LearningLogItem>.CreateListOfSize(4).All()
                .With(i => i.CompletedDate = null)
                .And(i => i.ArchivedDate = null)
                .And(i => i.LearningHubResourceReferenceId = resourceReferenceId)
                .And((i, index) => i.LearningLogItemId = index + 1)
                .TheFirst(1).With(i => i.CompletedDate = DateTime.UtcNow)
                .TheNext(1).With(i => i.ArchivedDate = DateTime.UtcNow)
                .TheNext(1).With(i => i.LearningHubResourceReferenceId = resourceReferenceId + 100)
                .Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(delegateId))
                .Returns(learningLogItems);

            // When
            actionPlanService.UpdateActionPlanResourcesLastAccessedDateIfPresent(resourceReferenceId, delegateId);

            // Then
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(delegateId))
                .MustHaveHappenedOnceExactly();
            foreach (var id in expectedLearningLogItemIdsToUpdate)
            {
                A.CallTo(
                        () => learningLogItemsDataService.UpdateLearningLogItemLastAccessedDate(id, testDate)
                    )
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void SetCompletionDate_calls_data_service()
        {
            // Given
            const int learningLogItemId = 1;
            var completedDate = new DateTime(2021, 09, 01);

            // When
            learningLogItemsDataService.SetCompletionDate(learningLogItemId, completedDate);

            // Then
            A.CallTo(() => learningLogItemsDataService.SetCompletionDate(learningLogItemId, completedDate))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RemoveActionPlanResource_removes_item()
        {
            // Given
            var testDate = new DateTime(2021, 12, 6);
            A.CallTo(() => clockService.UtcNow).Returns(testDate);
            const int delegateId = 2;
            const int actionPlanId = 3;
            A.CallTo(() => learningLogItemsDataService.RemoveLearningLogItem(A<int>._, A<int>._, A<DateTime>._))
                .DoesNothing();

            // When
            actionPlanService.RemoveActionPlanResource(actionPlanId, delegateId);

            // Then
            A.CallTo(() => learningLogItemsDataService.RemoveLearningLogItem(actionPlanId, delegateId, testDate))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void VerifyDelegateCanAccessActionPlanResource_returns_null_if_signposting_is_deactivated()
        {
            // Given
            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("false");

            // When
            var result = actionPlanService.VerifyDelegateCanAccessActionPlanResource(
                GenericLearningLogItemId,
                GenericDelegateId
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().BeNull();
                A.CallTo(() => learningLogItemsDataService.GetLearningLogItem(A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void
            VerifyDelegateCanAccessActionPlanResource_returns_null_if_LearningLogItem_with_given_id_does_not_exist()
        {
            // Given
            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("true");
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItem(GenericLearningLogItemId)).Returns(null);

            // When
            var result = actionPlanService.VerifyDelegateCanAccessActionPlanResource(
                GenericLearningLogItemId,
                GenericDelegateId
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().BeNull();
                A.CallTo(() => learningLogItemsDataService.GetLearningLogItem(GenericLearningLogItemId))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void VerifyDelegateCanAccessActionPlanResource_returns_null_if_LearningLogItem_is_removed()
        {
            // Given
            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("true");
            var learningLogItem = Builder<LearningLogItem>.CreateNew()
                .With(i => i.LearningHubResourceReferenceId = GenericLearningHubResourceReferenceId)
                .And(i => i.ArchivedDate = DateTime.UtcNow)
                .And(i => i.LoggedById = GenericDelegateId).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItem(GenericLearningLogItemId))
                .Returns(learningLogItem);

            // When
            var result = actionPlanService.VerifyDelegateCanAccessActionPlanResource(
                GenericLearningLogItemId,
                GenericDelegateId
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().BeNull();
                A.CallTo(() => learningLogItemsDataService.GetLearningLogItem(GenericLearningLogItemId))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void VerifyDelegateCanAccessActionPlanResource_returns_null_if_LearningLogItem_has_no_linked_resource()
        {
            // Given
            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("true");
            var learningLogItem = Builder<LearningLogItem>.CreateNew()
                .With(i => i.LearningHubResourceReferenceId = null)
                .And(i => i.ArchivedDate = null)
                .And(i => i.LoggedById = GenericDelegateId).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItem(GenericLearningLogItemId))
                .Returns(learningLogItem);

            // When
            var result = actionPlanService.VerifyDelegateCanAccessActionPlanResource(
                GenericLearningLogItemId,
                GenericDelegateId
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().BeNull();
                A.CallTo(() => learningLogItemsDataService.GetLearningLogItem(GenericLearningLogItemId))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void
            VerifyDelegateCanAccessActionPlanResource_returns_false_if_LearningLogItem_is_for_different_delegate()
        {
            // Given
            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("true");
            var learningLogItem = Builder<LearningLogItem>.CreateNew()
                .With(i => i.LearningHubResourceReferenceId = GenericLearningHubResourceReferenceId)
                .And(i => i.ArchivedDate = null)
                .And(i => i.LoggedById = GenericDelegateId + 1000).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItem(GenericLearningLogItemId))
                .Returns(learningLogItem);

            // When
            var result = actionPlanService.VerifyDelegateCanAccessActionPlanResource(
                GenericLearningLogItemId,
                GenericDelegateId
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().BeFalse();
                A.CallTo(() => learningLogItemsDataService.GetLearningLogItem(GenericLearningLogItemId))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void VerifyDelegateCanAccessActionPlanResource_returns_true_if_all_conditions_met()
        {
            // Given
            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("true");
            var learningLogItem = Builder<LearningLogItem>.CreateNew()
                .With(i => i.LearningHubResourceReferenceId = GenericLearningHubResourceReferenceId)
                .And(i => i.ArchivedDate = null)
                .And(i => i.LoggedById = GenericDelegateId).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItem(GenericLearningLogItemId))
                .Returns(learningLogItem);

            // When
            var result = actionPlanService.VerifyDelegateCanAccessActionPlanResource(
                GenericLearningLogItemId,
                GenericDelegateId
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                A.CallTo(() => learningLogItemsDataService.GetLearningLogItem(GenericLearningLogItemId))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void ResourceCanBeAddedToActionPlan_returns_true_with_no_learning_log_records()
        {
            // Given
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(GenericDelegateId))
                .Returns(new List<LearningLogItem>());

            // When
            var result = actionPlanService.ResourceCanBeAddedToActionPlan(1, GenericDelegateId);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void ResourceCanBeAddedToActionPlan_returns_true_with_no_incomplete_learning_log_records()
        {
            // Given
            var learningLogItems = Builder<LearningLogItem>.CreateListOfSize(5)
                .All()
                .With(i => i.LearningHubResourceReferenceId = GenericLearningHubResourceReferenceId)
                .And(i => i.LearningResourceReferenceId = GenericLearningResourceReferenceId)
                .And(i => i.ArchivedDate = null)
                .And(i => i.CompletedDate = DateTime.UtcNow).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(GenericDelegateId))
                .Returns(learningLogItems);

            // When
            var result = actionPlanService.ResourceCanBeAddedToActionPlan(
                GenericLearningResourceReferenceId,
                GenericDelegateId
            );

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void ResourceCanBeAddedToActionPlan_returns_false_with_an_incomplete_learning_log_record()
        {
            // Given
            var learningLogItems = Builder<LearningLogItem>.CreateListOfSize(5)
                .TheFirst(1)
                .With(i => i.LearningHubResourceReferenceId = GenericLearningHubResourceReferenceId)
                .And(i => i.LearningResourceReferenceId = GenericLearningResourceReferenceId)
                .And(i => i.ArchivedDate = null)
                .And(i => i.CompletedDate = null)
                .TheRest()
                .With(i => i.LearningHubResourceReferenceId = GenericLearningHubResourceReferenceId)
                .And(i => i.LearningResourceReferenceId = GenericLearningResourceReferenceId)
                .And(i => i.ArchivedDate = null)
                .And(i => i.CompletedDate = DateTime.UtcNow).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(GenericDelegateId))
                .Returns(learningLogItems);

            // When
            var result = actionPlanService.ResourceCanBeAddedToActionPlan(
                GenericLearningResourceReferenceId,
                GenericDelegateId
            );

            // Then
            result.Should().BeFalse();
        }

        private void GivenLearningHubResourceServiceBulkResponseReturnsExpectedResources(IList<int> learningResourceIds)
        {
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
            A.CallTo(
                    () => learningHubResourceService
                        .GetBulkResourcesByReferenceIdsAndPopulateDeletedDetailsFromDatabase(A<List<int>>._)
                )
                .Returns((bulkReturnedItems, false));
        }

        private void AssertThatActionPlanResourceIdsAndTitlesAreCorrect(
            IEnumerable<ActionPlanResource> actionPlanResources,
            IList<int> learningResourceIds
        )
        {
            List<(int id, string title)> resultIdsAndTitles = actionPlanResources.Select(r => (r.Id, r.Name)).ToList();
            using (new AssertionScope())
            {
                resultIdsAndTitles.Count.Should().Be(3);
                resultIdsAndTitles[0].Should().Be((4, "Title 15"));
                resultIdsAndTitles[1].Should().Be((5, "Title 21"));
                resultIdsAndTitles[2].Should().Be((6, "Title 33"));
                A.CallTo(
                        () => learningHubResourceService
                            .GetBulkResourcesByReferenceIdsAndPopulateDeletedDetailsFromDatabase(
                                A<List<int>>.That.IsSameSequenceAs(learningResourceIds)
                            )
                    )
                    .MustHaveHappenedOnceExactly();
            }
        }
    }
}
