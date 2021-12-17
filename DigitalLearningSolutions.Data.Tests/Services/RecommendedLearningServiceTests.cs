namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using NUnit.Framework;

    public class RecommendedLearningServiceTests
    {
        private const int SelfAssessmentId = 1;
        private const int CompetencyId = 2;
        private const int LearningResourceReferenceId = 3;
        private const int LearningHubResourceReferenceId = 4;
        private const int DelegateId = 5;
        private const int LearningLogId = 6;
        private const string ResourceName = "Resource";
        private const string ResourceDescription = "Description";
        private const string ResourceCatalogue = "Catalogue";
        private const string ResourceType = "link";
        private const string ResourceLink = "www.test.com";

        private ICompetencyLearningResourcesDataService competencyLearningResourcesDataService = null!;
        private ILearningHubApiClient learningHubApiClient = null!;
        private ILearningLogItemsDataService learningLogItemsDataService = null!;
        private IRecommendedLearningService recommendedLearningService = null!;
        private ISelfAssessmentDataService selfAssessmentDataService = null!;

        [SetUp]
        public void Setup()
        {
            competencyLearningResourcesDataService = A.Fake<ICompetencyLearningResourcesDataService>();
            learningLogItemsDataService = A.Fake<ILearningLogItemsDataService>();
            learningHubApiClient = A.Fake<ILearningHubApiClient>();
            selfAssessmentDataService = A.Fake<ISelfAssessmentDataService>();

            recommendedLearningService = new RecommendedLearningService(
                selfAssessmentDataService,
                competencyLearningResourcesDataService,
                learningHubApiClient,
                learningLogItemsDataService
            );
        }

        [Test]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_correctly_populated_resource_when_learning_log_empty()
        {
            // Given
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(DelegateId))
                .Returns(new List<LearningLogItem>());

            var expectedResource = GetExpectedResource(false, false, null);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
        }

        [Test]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_correctly_populated_resource_when_not_in_learning_log()
        {
            // Given
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi();

            var learningLogItems = Builder<LearningLogItem>.CreateListOfSize(5).All()
                .With(i => i.LearningHubResourceReferenceId = LearningHubResourceReferenceId + 1)
                .And(i => i.ArchivedDate = null).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(DelegateId))
                .Returns(learningLogItems);

            var expectedResource = GetExpectedResource(false, false, null);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
        }

        [Test]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_correctly_populated_resource_when_learning_log_item_is_completed()
        {
            // Given
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi();

            var learningLogItem = Builder<LearningLogItem>.CreateNew()
                .With(i => i.LearningHubResourceReferenceId = LearningHubResourceReferenceId)
                .And(i => i.CompletedDate = DateTime.UtcNow)
                .And(i => i.LearningLogItemId = LearningLogId)
                .And(i => i.ArchivedDate = null).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(DelegateId))
                .Returns(new List<LearningLogItem> { learningLogItem });

            var expectedResource = GetExpectedResource(false, true, null);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
        }

        [Test]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_correctly_populated_resource_learning_log_item_is_incomplete()
        {
            // Given
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi();

            var learningLogItem = Builder<LearningLogItem>.CreateNew()
                .With(i => i.LearningHubResourceReferenceId = LearningHubResourceReferenceId)
                .And(i => i.CompletedDate = null)
                .And(i => i.LearningLogItemId = LearningLogId)
                .And(i => i.ArchivedDate = null).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(DelegateId))
                .Returns(new List<LearningLogItem> { learningLogItem });

            var expectedResource = GetExpectedResource(true, false, LearningLogId);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
        }

        [Test]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_correctly_populated_resource_with_incomplete_and_complete_learning_log_item()
        {
            // Given
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi();

            var completeLearningLogItem = Builder<LearningLogItem>.CreateNew()
                .With(i => i.LearningHubResourceReferenceId = LearningHubResourceReferenceId)
                .And(i => i.CompletedDate = DateTime.UtcNow)
                .And(i => i.LearningLogItemId = LearningLogId)
                .And(i => i.ArchivedDate = null).Build();
            var incompleteLearningLogItem = Builder<LearningLogItem>.CreateNew()
                .With(i => i.LearningHubResourceReferenceId = LearningHubResourceReferenceId)
                .And(i => i.CompletedDate = null)
                .And(i => i.LearningLogItemId = LearningLogId)
                .And(i => i.ArchivedDate = null).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(DelegateId))
                .Returns(new List<LearningLogItem> { completeLearningLogItem, incompleteLearningLogItem });

            var expectedResource = GetExpectedResource(true, true, LearningLogId);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
        }

        [Test]
        public async Task GetRecommendedLearningForSelfAssessment_calls_learning_hub_api_with_distinct_ids()
        {
            // Given
            A.CallTo(() => selfAssessmentDataService.GetCompetencyIdsForSelfAssessment(SelfAssessmentId))
                .Returns(new[] { 1, 2, 3, 4, 5 });

            var competencyLearningResources = Builder<CompetencyLearningResource>.CreateListOfSize(5).All()
                .With(clr => clr.LearningHubResourceReferenceId = LearningHubResourceReferenceId)
                .And(clr => clr.LearningResourceReferenceId = LearningResourceReferenceId).Build();
            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyLearningResourcesByCompetencyId(A<int>._)
            ).Returns(competencyLearningResources);

            var clientResponse = new BulkResourceReferences
            {
                ResourceReferences = new List<ResourceReferenceWithResourceDetails>
                {
                    new ResourceReferenceWithResourceDetails
                    {
                        ResourceId = 0,
                        RefId = LearningHubResourceReferenceId,
                        Title = ResourceName,
                        Description = ResourceDescription,
                        Catalogue = new Catalogue { Name = ResourceCatalogue },
                        ResourceType = ResourceType,
                        Rating = 0,
                        Link = ResourceLink,
                    },
                },
            };

            A.CallTo(() => learningHubApiClient.GetBulkResourcesByReferenceIds(A<IEnumerable<int>>._))
                .Returns(clientResponse);

            // When
            await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId);

            // Then
            A.CallTo(
                    () => learningHubApiClient.GetBulkResourcesByReferenceIds(
                        A<IEnumerable<int>>.That.Matches(i => i.Single() == LearningHubResourceReferenceId)
                    )
                )
                .MustHaveHappenedOnceExactly();
        }

        private void GivenResourceForSelfAssessmentIsReturnedByLearningHubApi()
        {
            A.CallTo(() => selfAssessmentDataService.GetCompetencyIdsForSelfAssessment(SelfAssessmentId))
                .Returns(new[] { CompetencyId });

            var competencyLearningResource = new CompetencyLearningResource
            {
                Id = 1,
                CompetencyId = CompetencyId,
                LearningResourceReferenceId = LearningResourceReferenceId,
                AdminId = 7,
                LearningHubResourceReferenceId = LearningHubResourceReferenceId,
            };

            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyLearningResourcesByCompetencyId(CompetencyId)
            ).Returns(new List<CompetencyLearningResource> { competencyLearningResource });

            var clientResponse = new BulkResourceReferences
            {
                ResourceReferences = new List<ResourceReferenceWithResourceDetails>
                {
                    new ResourceReferenceWithResourceDetails
                    {
                        ResourceId = 0,
                        RefId = LearningHubResourceReferenceId,
                        Title = ResourceName,
                        Description = ResourceDescription,
                        Catalogue = new Catalogue { Name = ResourceCatalogue },
                        ResourceType = ResourceType,
                        Rating = 0,
                        Link = ResourceLink,
                    },
                },
            };

            A.CallTo(() => learningHubApiClient.GetBulkResourcesByReferenceIds(A<IEnumerable<int>>._))
                .Returns(clientResponse);
        }

        private RecommendedResource GetExpectedResource(bool isInActionPlan, bool isCompleted, int? learningLogId)
        {
            return new RecommendedResource
            {
                LearningResourceReferenceId = LearningResourceReferenceId,
                LearningHubReferenceId = LearningHubResourceReferenceId,
                ResourceName = ResourceName,
                ResourceDescription = ResourceDescription,
                ResourceType = ResourceType,
                CatalogueName = ResourceCatalogue,
                ResourceLink = ResourceLink,
                IsInActionPlan = isInActionPlan,
                IsCompleted = isCompleted,
                LearningLogId = learningLogId,
                RecommendationScore = 0,
            };
        }
    }
}
