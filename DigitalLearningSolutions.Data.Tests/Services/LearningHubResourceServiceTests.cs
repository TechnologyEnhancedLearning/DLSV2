namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class LearningHubResourceServiceTests
    {
        private const int SingleResourceReferenceId = 1;
        private ILearningHubApiClient learningHubApiClient = null!;
        private LearningHubResourceService learningHubResourceService = null!;
        private ILearningResourceReferenceDataService learningResourceReferenceDataService = null!;
        private ILogger<LearningHubResourceService> logger = null!;

        [SetUp]
        public void SetUp()
        {
            learningHubApiClient = A.Fake<ILearningHubApiClient>();
            learningResourceReferenceDataService = A.Fake<ILearningResourceReferenceDataService>();
            logger = A.Fake<ILogger<LearningHubResourceService>>();
            learningHubResourceService = new LearningHubResourceService(
                learningHubApiClient,
                learningResourceReferenceDataService,
                logger
            );
        }

        [Test]
        public async Task GetResourceByReferenceId_returns_API_data_if_retrieved()
        {
            // Given
            var resource = GenerateGenericResource(SingleResourceReferenceId);
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
            var resource = GenerateGenericResource(SingleResourceReferenceId);
            A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(SingleResourceReferenceId))
                .Throws<HttpRequestException>();
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
            var resource1 = GenerateGenericResource(1);
            var resource2 = GenerateGenericResource(2);
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
            var resource1 = GenerateGenericResource(1);
            var resource2 = GenerateGenericResource(2);
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
                .Throws<HttpRequestException>();
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

        private static ResourceReferenceWithResourceDetails GenerateGenericResource(int resourceReferenceId)
        {
            var catalogue = Builder<Catalogue>.CreateNew()
                .With(c => c.Id = 10)
                .And(c => c.Name = "Catalogue")
                .And(c => c.IsRestricted = false).Build();

            return Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = resourceReferenceId)
                .And(r => r.ResourceId = 1)
                .And(r => r.Title = "Title")
                .And(r => r.Description = "Description")
                .And(r => r.Link = "Link")
                .And(r => r.ResourceType = "Resource Type")
                .And(r => r.Rating = 3)
                .And(r => r.Catalogue = catalogue)
                .Build();
        }
    }
}
