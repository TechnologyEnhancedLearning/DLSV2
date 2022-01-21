namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
        private const int SecondCompetencyId = 3;
        private const int LearningResourceReferenceId = 3;
        private const int LearningHubResourceReferenceId = 4;
        private const int DelegateId = 5;
        private const int LearningLogId = 6;
        private const int CompetencyLearningResourceId = 1;
        private const int SecondCompetencyLearningResourceId = 2;
        private const int CompetencyAssessmentQuestionId = 1;
        private const int RelevanceAssessmentQuestionId = 2;
        private const string ResourceName = "Resource";
        private const string ResourceDescription = "Description";
        private const string ResourceCatalogue = "Catalogue";
        private const string ResourceType = "link";
        private const string ResourceLink = "www.test.com";

        private ICompetencyLearningResourcesDataService competencyLearningResourcesDataService = null!;
        private ILearningHubResourceService learningHubResourceService = null!;
        private ILearningLogItemsDataService learningLogItemsDataService = null!;
        private IRecommendedLearningService recommendedLearningService = null!;
        private ISelfAssessmentDataService selfAssessmentDataService = null!;

        [SetUp]
        public void Setup()
        {
            competencyLearningResourcesDataService = A.Fake<ICompetencyLearningResourcesDataService>();
            learningLogItemsDataService = A.Fake<ILearningLogItemsDataService>();
            learningHubResourceService = A.Fake<ILearningHubResourceService>();
            selfAssessmentDataService = A.Fake<ISelfAssessmentDataService>();

            recommendedLearningService = new RecommendedLearningService(
                selfAssessmentDataService,
                competencyLearningResourcesDataService,
                learningHubResourceService,
                learningLogItemsDataService
            );
        }

        [Test]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_correctly_populated_resource_when_learning_log_empty()
        {
            // Given
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi();
            GivenQuestionParametersAreReturned(true, true, 1, 10);
            GivenSelfAssessmentHasResultsForFirstCompetency(5, 5);

            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(DelegateId))
                .Returns(new List<LearningLogItem>());

            var expectedResource = GetExpectedResource(false, false, null, 175);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .recommendedResources
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
            GivenQuestionParametersAreReturned(true, true, 1, 10);
            GivenSelfAssessmentHasResultsForFirstCompetency(5, 5);

            var learningLogItems = Builder<LearningLogItem>.CreateListOfSize(5).All()
                .With(i => i.LearningHubResourceReferenceId = LearningHubResourceReferenceId + 1)
                .And(i => i.ArchivedDate = null).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(DelegateId))
                .Returns(learningLogItems);

            var expectedResource = GetExpectedResource(false, false, null, 175);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .recommendedResources.ToList();

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
            GivenQuestionParametersAreReturned(true, true, 1, 10);
            GivenSelfAssessmentHasResultsForFirstCompetency(5, 5);

            var learningLogItem = Builder<LearningLogItem>.CreateNew()
                .With(i => i.LearningHubResourceReferenceId = LearningHubResourceReferenceId)
                .And(i => i.CompletedDate = DateTime.UtcNow)
                .And(i => i.LearningLogItemId = LearningLogId)
                .And(i => i.ArchivedDate = null).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(DelegateId))
                .Returns(new List<LearningLogItem> { learningLogItem });

            var expectedResource = GetExpectedResource(false, true, null, 175);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .recommendedResources.ToList();

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
            GivenQuestionParametersAreReturned(true, true, 1, 10);
            GivenSelfAssessmentHasResultsForFirstCompetency(5, 5);

            var learningLogItem = Builder<LearningLogItem>.CreateNew()
                .With(i => i.LearningHubResourceReferenceId = LearningHubResourceReferenceId)
                .And(i => i.CompletedDate = null)
                .And(i => i.LearningLogItemId = LearningLogId)
                .And(i => i.ArchivedDate = null).Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(DelegateId))
                .Returns(new List<LearningLogItem> { learningLogItem });

            var expectedResource = GetExpectedResource(true, false, LearningLogId, 175);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .recommendedResources.ToList();

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
            GivenQuestionParametersAreReturned(true, true, 1, 10);
            GivenSelfAssessmentHasResultsForFirstCompetency(5, 5);

            var learningLogItems = Builder<LearningLogItem>.CreateListOfSize(2)
                .All()
                .With(i => i.LearningHubResourceReferenceId = LearningHubResourceReferenceId)
                .And(i => i.LearningLogItemId = LearningLogId)
                .And(i => i.ArchivedDate = null)
                .TheFirst(1)
                .With(i => i.CompletedDate = DateTime.UtcNow)
                .TheRest()
                .With(i => i.CompletedDate = null)
                .Build();
            A.CallTo(() => learningLogItemsDataService.GetLearningLogItems(DelegateId))
                .Returns(learningLogItems);

            var expectedResource = GetExpectedResource(true, true, LearningLogId, 175);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .recommendedResources.ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
        }

        [Test]
        public async Task
            GetRecommendedLearningForSelfAssessment_calls_learning_hub_resource_service_with_distinct_ids()
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

            GivenLearningHubApiReturnsResources(0);

            // When
            await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId);

            // Then
            A.CallTo(
                    () => learningHubResourceService.GetBulkResourcesByReferenceIds(
                        A<List<int>>.That.Matches(i => i.Single() == LearningHubResourceReferenceId)
                    )
                )
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        [TestCase(true, 175)]
        [TestCase(false, 105)]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_correct_recommendation_score_for_resource_with_essential_question_parameters(
                bool essential,
                decimal expectedScore
            )
        {
            // Given
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi();
            GivenGetLearningLogItemsReturnsAnItem();
            GivenQuestionParametersAreReturned(essential, true, 1, 10);
            GivenSelfAssessmentHasResultsForFirstCompetency(5, 5);

            var expectedResource = GetExpectedResource(true, false, LearningLogId, expectedScore);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .recommendedResources.ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
        }

        [Test]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_correct_recommendation_score_for_resource_with_multiple_competencies_with_essential_question_parameters_of_different_value()
        {
            // Given
            GivenResourceHasTwoCompetencies();
            GivenLearningHubApiReturnsResources(0);
            GivenGetLearningLogItemsReturnsAnItem();
            GivenSelfAssessmentHasResultsForFirstCompetency(5, 5);

            var questionParameters = Builder<CompetencyResourceAssessmentQuestionParameter>
                .CreateListOfSize(2)
                .All()
                .With(qp => qp.MinResultMatch = 1)
                .And(qp => qp.MaxResultMatch = 10)
                .TheFirst(1)
                .With(qp => qp.Essential = true)
                .And(qp => qp.CompetencyLearningResourceId = CompetencyLearningResourceId)
                .TheRest()
                .With(qp => qp.Essential = false)
                .And(qp => qp.CompetencyLearningResourceId = 2)
                .Build();

            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyResourceAssessmentQuestionParameters(
                    A<IEnumerable<int>>._
                )
            ).Returns(questionParameters);

            var expectedResource = GetExpectedResource(true, false, LearningLogId, 175);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .recommendedResources.ToList();

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
            GetRecommendedLearningForSelfAssessment_returns_correct_recommendation_score_for_resource_with_learning_hub_ratings_and_no_question_parameters(
                decimal learningHubRating,
                decimal expectedScore
            )
        {
            // Given
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi(learningHubRating);
            GivenGetLearningLogItemsReturnsAnItem();
            GivenSelfAssessmentHasResultsForFirstCompetency(5, 5);

            var expectedResource = GetExpectedResource(true, false, LearningLogId, expectedScore);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .recommendedResources.ToList();

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
            GivenQuestionParametersAreReturned(true, true, 1, 10);
            GivenSelfAssessmentHasResultsForFirstCompetency(5, 5);

            var roleRequirement = Builder<CompetencyAssessmentQuestionRoleRequirement>.CreateNew()
                .With(rr => rr.LevelRag = levelRag).Build();
            A.CallTo(
                () => selfAssessmentDataService.GetCompetencyAssessmentQuestionRoleRequirements(
                    CompetencyId,
                    SelfAssessmentId
                )
            ).Returns(roleRequirement);

            var expectedResource = GetExpectedResource(true, false, LearningLogId, expectedScore);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .recommendedResources.ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
            A.CallTo(
                () => selfAssessmentDataService.GetCompetencyAssessmentQuestionRoleRequirements(
                    CompetencyId,
                    SelfAssessmentId
                )
            ).MustHaveHappenedOnceExactly();
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
            GivenSelfAssessmentHasResultsForFirstCompetency(relevanceResult, confidenceResult);

            var expectedResource = GetExpectedResource(true, false, LearningLogId, expectedScore);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .recommendedResources.ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
            A.CallTo(
                () => selfAssessmentDataService.GetCompetencyAssessmentQuestionRoleRequirements(
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public async Task
            GetRecommendedLearningForSelfAssessment_does_not_return_resources_relating_to_unanswered_optional_competencies()
        {
            // Given
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi();
            GivenGetLearningLogItemsReturnsAnItem();
            GivenNotComparingToRoleRequirements();

            A.CallTo(
                () => selfAssessmentDataService.GetSelfAssessmentResultsForDelegateSelfAssessmentCompetency(
                    DelegateId,
                    SelfAssessmentId,
                    CompetencyId
                )
            ).Returns(new List<SelfAssessmentResult>());

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .recommendedResources.ToList();

            // Then
            result.Should().BeEmpty();
            A.CallTo(
                () => selfAssessmentDataService.GetCompetencyAssessmentQuestionRoleRequirements(
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public async Task
            GetRecommendedLearningForSelfAssessment_does_return_resources_with_some_unanswered_optional_competencies()
        {
            // Given
            GivenResourceHasTwoCompetencies();
            GivenLearningHubApiReturnsResources(0);
            GivenGetLearningLogItemsReturnsAnItem();
            GivenSelfAssessmentHasResultsForFirstCompetency(5, 5);

            var questionParameters = Builder<CompetencyResourceAssessmentQuestionParameter>
                .CreateListOfSize(2)
                .All()
                .With(qp => qp.MinResultMatch = 1)
                .And(qp => qp.MaxResultMatch = 10)
                .TheFirst(1)
                .With(qp => qp.Essential = true)
                .And(qp => qp.CompetencyLearningResourceId = CompetencyLearningResourceId)
                .TheRest()
                .With(qp => qp.Essential = false)
                .And(qp => qp.CompetencyLearningResourceId = SecondCompetencyLearningResourceId)
                .Build();

            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyResourceAssessmentQuestionParameters(
                    A<IEnumerable<int>>._
                )
            ).Returns(questionParameters);

            var expectedResource = GetExpectedResource(true, false, LearningLogId, 175);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .recommendedResources.ToList();

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(expectedResource);
        }

        [Test]
        [TestCase(1, 10, 5, 1)]
        [TestCase(5, 10, 4, 0)]
        [TestCase(1, 5, 6, 0)]
        [TestCase(3, 7, 3, 1)]
        [TestCase(3, 7, 7, 1)]
        public async Task
            GetRecommendedLearningForSelfAssessment_returns_expected_number_of_resources_for_answers_in_and_out_of_range(
                int minScore,
                int maxScore,
                int confidenceResult,
                int expectedResultCount
            )
        {
            // Given
            GivenResourceForSelfAssessmentIsReturnedByLearningHubApi();
            GivenGetLearningLogItemsReturnsAnItem();
            GivenQuestionParametersAreReturned(true, true, minScore, maxScore);
            GivenSelfAssessmentHasResultsForFirstCompetency(5, confidenceResult);

            // When
            var result =
                (await recommendedLearningService.GetRecommendedLearningForSelfAssessment(SelfAssessmentId, DelegateId))
                .recommendedResources.ToList();

            // Then
            result.Should().HaveCount(expectedResultCount);
        }

        private void GivenResourceForSelfAssessmentIsReturnedByLearningHubApi(decimal rating = 0)
        {
            GivenSingleCompetencyExistsForResource();
            GivenLearningHubApiReturnsResources(rating);
        }

        private void GivenSingleCompetencyExistsForResource()
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
        }

        private void GivenResourceHasTwoCompetencies()
        {
            A.CallTo(() => selfAssessmentDataService.GetCompetencyIdsForSelfAssessment(SelfAssessmentId))
                .Returns(new[] { CompetencyId, SecondCompetencyId });

            var competencyLearningResources = Builder<CompetencyLearningResource>.CreateListOfSize(2)
                .All()
                .With(clr => clr.LearningResourceReferenceId = LearningResourceReferenceId)
                .And(clr => clr.LearningHubResourceReferenceId = LearningHubResourceReferenceId)
                .And(clr => clr.AdminId = 7)
                .TheFirst(1)
                .With(clr => clr.Id = CompetencyLearningResourceId)
                .And(clr => clr.CompetencyId = CompetencyId)
                .TheRest()
                .With(clr => clr.Id = SecondCompetencyLearningResourceId)
                .And(clr => clr.CompetencyId = SecondCompetencyId)
                .Build();

            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyLearningResourcesByCompetencyId(CompetencyId)
            ).Returns(competencyLearningResources);
        }

        private void GivenLearningHubApiReturnsResources(decimal rating)
        {
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

            A.CallTo(() => learningHubResourceService.GetBulkResourcesByReferenceIds(A<List<int>>._))
                .Returns((clientResponse, false));
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
                .And(qp => qp.CompetencyLearningResourceId = CompetencyLearningResourceId)
                .And(qp => qp.AssessmentQuestionId = CompetencyAssessmentQuestionId)
                .And(qp => qp.RelevanceAssessmentQuestionId = RelevanceAssessmentQuestionId)
                .And(qp => qp.MinResultMatch = 1)
                .And(qp => qp.MaxResultMatch = 10)
                .Build();
            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyResourceAssessmentQuestionParameters(
                    A<IEnumerable<int>>._
                )
            ).Returns(questionParameters);
        }

        private void GivenQuestionParametersAreReturned(
            bool essential,
            bool compareToRoleRequirements,
            int minMatch,
            int maxMatch
        )
        {
            var questionParameters = Builder<CompetencyResourceAssessmentQuestionParameter>
                .CreateListOfSize(1).All()
                .With(qp => qp.Essential = essential)
                .And(qp => qp.CompareToRoleRequirements = compareToRoleRequirements)
                .And(qp => qp.MinResultMatch = minMatch)
                .And(qp => qp.MaxResultMatch = maxMatch)
                .And(qp => qp.CompetencyLearningResourceId = CompetencyLearningResourceId)
                .Build();
            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyResourceAssessmentQuestionParameters(
                    A<IEnumerable<int>>._
                )
            ).Returns(questionParameters);
        }

        private void GivenSelfAssessmentHasResultsForFirstCompetency(int relevanceResult, int confidenceResult)
        {
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
