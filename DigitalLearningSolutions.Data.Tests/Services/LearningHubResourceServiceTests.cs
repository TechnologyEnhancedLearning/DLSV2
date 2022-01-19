namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
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
                result.ResourceReferenceWithResourceDetails.Should().BeEquivalentTo(resource);
                result.SourcedFromFallbackData.Should().BeFalse();
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
                .Throws<Exception>();
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
                result.ResourceReferenceWithResourceDetails.Should().BeEquivalentTo(resource);
                result.SourcedFromFallbackData.Should().BeTrue();
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
                result.BulkResourceReferences.Should().BeEquivalentTo(bulkResources);
                result.SourcedFromFallbackData.Should().BeFalse();
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
                .Throws<Exception>();
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
                result.BulkResourceReferences.Should().BeEquivalentTo(bulkResources);
                result.SourcedFromFallbackData.Should().BeTrue();
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

        private ResourceReferenceWithResourceDetails GenerateGenericResource(int resourceReferenceId)
        {
            var catalogue = new Catalogue
            {
                Id = 10,
                Name = "Catalogue",
                IsRestricted = false,
            };

            return new ResourceReferenceWithResourceDetails
            {
                RefId = resourceReferenceId,
                ResourceId = 1,
                Title = "Title",
                Description = "Description",
                Link = "Link",
                ResourceType = "Resource Type",
                Rating = 3,
                Catalogue = catalogue,
            };
        }
    }
}
