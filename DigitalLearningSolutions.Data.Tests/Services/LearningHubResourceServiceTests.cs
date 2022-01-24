namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class LearningHubResourceServiceTests
    {
        private const int SingleResourceReferenceId = 1;
        private ILearningHubApiClient learningHubApiClient = null!;
        private LearningHubResourceService learningHubResourceService = null!;
        private ILearningResourceReferenceDataService learningResourceReferenceDataService = null!;

        [SetUp]
        public void SetUp()
        {
            learningHubApiClient = A.Fake<ILearningHubApiClient>();
            learningResourceReferenceDataService = A.Fake<ILearningResourceReferenceDataService>();
            learningHubResourceService = new LearningHubResourceService(
                learningHubApiClient,
                learningResourceReferenceDataService
            );
        }

        [Test]
        public async Task GetResourceByReferenceId_returns_API_data_if_retrieved()
        {
            // Given
            var resource = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = SingleResourceReferenceId).Build();
            A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(SingleResourceReferenceId))
                .Returns(resource);

            // When
            var result = (await learningHubResourceService.GetResourceByReferenceId(SingleResourceReferenceId))!;

            // Then
            using (new AssertionScope())
            {
                result.resource.Should().BeEquivalentTo(resource);
                result.sourcedFromFallbackData.Should().BeFalse();
                A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(SingleResourceReferenceId))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(
                        () => learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(
                            A<IEnumerable<int>>._
                        )
                    )
                    .MustNotHaveHappened();
            }
        }

        [Test]
        public async Task GetResourceByReferenceId_returns_fallback_data_if_API_throws_exception()
        {
            // Given
            var resource = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = SingleResourceReferenceId).Build();
            A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(SingleResourceReferenceId))
                .Throws(new LearningHubResponseException("exception", HttpStatusCode.Unauthorized));
            A.CallTo(
                () => learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(
                    A<IEnumerable<int>>._
                )
            ).Returns(new List<ResourceReferenceWithResourceDetails> { resource });

            // When
            var result = (await learningHubResourceService.GetResourceByReferenceId(SingleResourceReferenceId))!;

            // Then
            using (new AssertionScope())
            {
                result.resource.Should().BeEquivalentTo(resource);
                result.sourcedFromFallbackData.Should().BeTrue();
                A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(A<int>._))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(
                        () => learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(
                            A<IEnumerable<int>>.That.IsSameSequenceAs(new[] { SingleResourceReferenceId })
                        )
                    )
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public async Task GetBulkResourcesByReferenceIds_returns_API_data_if_retrieved()
        {
            // Given
            var resource1 = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = 1).Build();
            var resource2 = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = 2).Build();
            var resourceReferenceIdsToRetrieve = new[] { 1, 2, 3 };
            var bulkResources = new BulkResourceReferences
            {
                ResourceReferences = new List<ResourceReferenceWithResourceDetails>
                {
                    resource1,
                    resource2,
                },
                UnmatchedResourceReferenceIds = new List<int> { 3 },
            };
            A.CallTo(() => learningHubApiClient.GetBulkResourcesByReferenceIds(A<IEnumerable<int>>._))
                .Returns(bulkResources);

            // When
            var result =
                (await learningHubResourceService.GetBulkResourcesByReferenceIds(resourceReferenceIdsToRetrieve))!;

            // Then
            using (new AssertionScope())
            {
                result.bulkResourceReferences.Should().BeEquivalentTo(bulkResources);
                result.sourcedFromFallbackData.Should().BeFalse();
                A.CallTo(
                        () => learningHubApiClient.GetBulkResourcesByReferenceIds(
                            A<IEnumerable<int>>.That.IsSameSequenceAs(resourceReferenceIdsToRetrieve)
                        )
                    )
                    .MustHaveHappenedOnceExactly();
                A.CallTo(
                        () => learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(
                            A<IEnumerable<int>>._
                        )
                    )
                    .MustNotHaveHappened();
            }
        }

        [Test]
        public async Task GetBulkResourcesByReferenceIds_returns_fallback_data_if_API_throws_exception()
        {
            // Given
            var resource1 = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = 1).Build();
            var resource2 = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = 2).Build();
            var resourceReferenceIdsToRetrieve = new[] { 1, 2, 3 };
            var bulkResources = new BulkResourceReferences
            {
                ResourceReferences = new List<ResourceReferenceWithResourceDetails>
                {
                    resource1,
                    resource2,
                },
                UnmatchedResourceReferenceIds = new List<int> { 3 },
            };

            A.CallTo(() => learningHubApiClient.GetBulkResourcesByReferenceIds(A<IEnumerable<int>>._))
                .Throws(new LearningHubResponseException("exception", HttpStatusCode.Unauthorized));
            A.CallTo(
                () => learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(
                    A<IEnumerable<int>>._
                )
            ).Returns(new List<ResourceReferenceWithResourceDetails> { resource1, resource2 });

            // When
            var result =
                (await learningHubResourceService.GetBulkResourcesByReferenceIds(resourceReferenceIdsToRetrieve))!;

            // Then
            using (new AssertionScope())
            {
                result.bulkResourceReferences.Should().BeEquivalentTo(bulkResources);
                result.sourcedFromFallbackData.Should().BeTrue();
                A.CallTo(
                        () => learningHubApiClient.GetBulkResourcesByReferenceIds(
                            A<IEnumerable<int>>.That.IsSameSequenceAs(resourceReferenceIdsToRetrieve)
                        )
                    )
                    .MustHaveHappenedOnceExactly();
                A.CallTo(
                        () => learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(
                            A<IEnumerable<int>>.That.IsSameSequenceAs(resourceReferenceIdsToRetrieve)
                        )
                    )
                    .MustHaveHappenedOnceExactly();
            }
        }
    }
}
