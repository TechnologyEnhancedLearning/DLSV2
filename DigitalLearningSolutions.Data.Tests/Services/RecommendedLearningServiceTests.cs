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
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
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
        private const int CompetencyLearningResourceId = 1;
        private const int CompetencyAssessmentQuestionId = 1;
        private const int RelevanceAssessmentQuestionId = 2;
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

        [Test]
        [TestCase(1, true, 100)]
        [TestCase(1, false, 30)]
        [TestCase(0, true, 0)]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_correct_recommendation_score_for_resource_with_essential_question_parameters_only(
                int numberOfQuestionParameters,
                bool essential,
                decimal expectedScore
            )
        {
            // Given
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi();
            GivenGetLearningLogItemsReturnsAnItem();

            var questionParameters =
                numberOfQuestionParameters == 0
                    ? new List<CompetencyResourceAssessmentQuestionParameter>()
                    : Builder<CompetencyResourceAssessmentQuestionParameter>
                        .CreateListOfSize(numberOfQuestionParameters).All()
                        .With(qp => qp.Essential = essential).Build();

            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyResourceAssessmentQuestionParameters(
                    A<IEnumerable<int>>._
                )
            ).Returns(questionParameters);

            var expectedResource = GetExpectedResource(true, false, LearningLogId, expectedScore);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 4)]
        [TestCase(2.4, 9.6)]
        [TestCase(5, 20)]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_correct_recommendation_score_for_resource_with_learning_hub_ratings_only(
                decimal learningHubRating,
                decimal expectedScore
            )
        {
            // Given
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi(learningHubRating);
            GivenGetLearningLogItemsReturnsAnItem();

            var expectedResource = GetExpectedResource(true, false, LearningLogId, expectedScore);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
        }

        [Test]
        [TestCase(0, 175)]
        [TestCase(1, 150)]
        [TestCase(2, 125)]
        [TestCase(5, 100)]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_correct_recommendation_score_for_resource_with_competency_role_requirements(
                int levelRag,
                decimal expectedScore
            )
        {
            // Given
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi();
            GivenGetLearningLogItemsReturnsAnItem();

            var questionParameters = Builder<CompetencyResourceAssessmentQuestionParameter>
                .CreateListOfSize(1).All()
                .With(qp => qp.Essential = true)
                .And(qp => qp.CompareToRoleRequirements = true).Build();
            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyResourceAssessmentQuestionParameters(
                    A<IEnumerable<int>>._
                )
            ).Returns(questionParameters);

            var roleRequirements = Builder<CompetencyAssessmentQuestionRoleRequirement>.CreateListOfSize(1).All()
                .With(rr => rr.LevelRag = levelRag).Build();
            A.CallTo(
                () => selfAssessmentDataService.GetCompetencyAssessmentQuestionRoleRequirements(
                    CompetencyId,
                    SelfAssessmentId
                )
            ).Returns(roleRequirements);

            var expectedResource = GetExpectedResource(true, false, LearningLogId, expectedScore);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
            A.CallTo(
                () => selfAssessmentDataService.GetCompetencyAssessmentQuestionRoleRequirements(
                    CompetencyId,
                    SelfAssessmentId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(() => selfAssessmentDataService.GetCompetencyAssessmentQuestions(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        [TestCase(10, 10, 100)]
        [TestCase(1, 10, 100)]
        [TestCase(10, 1, 190)]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_correct_recommendation_score_for_resource_with_self_assessment_results(
                int relevanceResult,
                int confidenceResult,
                decimal expectedScore
            )
        {
            // Given

            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi();
            GivenGetLearningLogItemsReturnsAnItem();
            GivenNotComparingToRoleRequirements();

            var assessmentQuestions = Builder<CompetencyAssessmentQuestion>.CreateListOfSize(2).All()
                .With(caq => caq.CompetencyId = CompetencyId)
                .TheFirst(1).With(caq => caq.AssessmentQuestionId = CompetencyAssessmentQuestionId)
                .TheRest().With(caq => caq.AssessmentQuestionId = RelevanceAssessmentQuestionId).Build();
            A.CallTo(
                () => selfAssessmentDataService.GetCompetencyAssessmentQuestions(
                    CompetencyId
                )
            ).Returns(assessmentQuestions);

            var assessmentResults = Builder<SelfAssessmentResult>.CreateListOfSize(2)
                .All()
                .With(r => r.SelfAssessmentId = SelfAssessmentId)
                .And(r => r.CandidateId = DelegateId)
                .And(r => r.CompetencyId = CompetencyId)
                .TheFirst(1)
                .With(r => r.AssessmentQuestionId = CompetencyAssessmentQuestionId)
                .And(r => r.Result = confidenceResult)
                .TheRest()
                .With(r => r.AssessmentQuestionId = RelevanceAssessmentQuestionId)
                .And(r => r.Result = relevanceResult)
                .Build();
            A.CallTo(
                () => selfAssessmentDataService.GetSelfAssessmentResultsForDelegateSelfAssessmentCompetency(
                    DelegateId,
                    SelfAssessmentId,
                    CompetencyId
                )
            ).Returns(assessmentResults);

            var expectedResource = GetExpectedResource(true, false, LearningLogId, expectedScore);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
            A.CallTo(
                () => selfAssessmentDataService.GetCompetencyAssessmentQuestionRoleRequirements(
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
            A.CallTo(() => selfAssessmentDataService.GetCompetencyAssessmentQuestions(CompetencyId))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_correct_recommendation_score_for_resource_with_missing_self_assessment_results()
        {
            // Given
            const int competencyAssessmentQuestionId = 1;
            const int relevanceAssessmentQuestionId = 2;
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi();
            GivenGetLearningLogItemsReturnsAnItem();
            GivenNotComparingToRoleRequirements();

            var assessmentQuestions = Builder<CompetencyAssessmentQuestion>.CreateListOfSize(2).All()
                .With(caq => caq.CompetencyId = CompetencyId)
                .TheFirst(1).With(caq => caq.AssessmentQuestionId = competencyAssessmentQuestionId)
                .TheRest().With(caq => caq.AssessmentQuestionId = relevanceAssessmentQuestionId).Build();
            A.CallTo(
                () => selfAssessmentDataService.GetCompetencyAssessmentQuestions(
                    CompetencyId
                )
            ).Returns(assessmentQuestions);

            A.CallTo(
                () => selfAssessmentDataService.GetSelfAssessmentResultsForDelegateSelfAssessmentCompetency(
                    DelegateId,
                    SelfAssessmentId,
                    CompetencyId
                )
            ).Returns(new List<SelfAssessmentResult>());

            var expectedResource = GetExpectedResource(true, false, LearningLogId, 100);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
            A.CallTo(
                () => selfAssessmentDataService.GetCompetencyAssessmentQuestionRoleRequirements(
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
            A.CallTo(() => selfAssessmentDataService.GetCompetencyAssessmentQuestions(CompetencyId))
                .MustHaveHappenedOnceExactly();
        }

        private void GivenResourceForSelfAssessmentIsReturnedByLearningHubApi(decimal rating = 0)
        {
            A.CallTo(() => selfAssessmentDataService.GetCompetencyIdsForSelfAssessment(SelfAssessmentId))
                .Returns(new[] { CompetencyId });

            var competencyLearningResource = new CompetencyLearningResource
            {
                Id = CompetencyLearningResourceId,
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
                        Rating = rating,
                        Link = ResourceLink,
                    },
                },
            };

            A.CallTo(() => learningHubApiClient.GetBulkResourcesByReferenceIds(A<IEnumerable<int>>._))
                .Returns(clientResponse);
        }

        private void GivenGetLearningLogItemsReturnsAnItem()
        {
            var learningLogItem = Builder<LearningLogItem>.CreateNew()
                .With(i => i.LearningHubResourceReferenceId = LearningHubResourceReferenceId)
                .And(i => i.CompletedDate = null)
                .And(i => i.LearningLogItemId = LearningLogId)
                .And(i => i.ArchivedDate = null).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(DelegateId))
                .Returns(new List<LearningLogItem> { learningLogItem });
        }

        private void GivenNotComparingToRoleRequirements()
        {
            var questionParameters = Builder<CompetencyResourceAssessmentQuestionParameter>
                .CreateListOfSize(1).All()
                .With(qp => qp.Essential = true)
                .And(qp => qp.CompareToRoleRequirements = false)
                .And(qp => qp.AssessmentQuestionId = CompetencyAssessmentQuestionId)
                .And(qp => qp.RelevanceAssessmentQuestionId = RelevanceAssessmentQuestionId).Build();
            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyResourceAssessmentQuestionParameters(
                    A<IEnumerable<int>>._
                )
            ).Returns(questionParameters);
        }

        private RecommendedResource GetExpectedResource(
            bool isInActionPlan,
            bool isCompleted,
            int? learningLogId,
            decimal recommendationScore = 0
        )
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
                RecommendationScore = recommendationScore,
            };
        }
    }
}
