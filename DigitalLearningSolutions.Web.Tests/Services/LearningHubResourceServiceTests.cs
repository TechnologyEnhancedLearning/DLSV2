namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using DigitalLearningSolutions.Web.Services;
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
        private ILogger<ILearningHubResourceService> logger = null!;

        [SetUp]
        public void SetUp()
        {
            learningHubApiClient = A.Fake<ILearningHubApiClient>();
            learningResourceReferenceDataService = A.Fake<ILearningResourceReferenceDataService>();
            logger = A.Fake<ILogger<ILearningHubResourceService>>();
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
            var resource = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = SingleResourceReferenceId)
                .And(r => r.AbsentInLearningHub = false)
                .Build();
            A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(SingleResourceReferenceId))
                .Returns(resource);

            // When
            var result = (await learningHubResourceService.GetResourceByReferenceId(SingleResourceReferenceId))!;

            // Then
            using (new AssertionScope())
            {
                result.resource.Should().BeEquivalentTo(resource);
                result.resource!.AbsentInLearningHub.Should().BeFalse();
                result.apiIsAccessible.Should().BeTrue();
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
        public async Task GetResourceByReferenceIdAndPopulateDeletedDetailsFromDatabase_returns_API_data_if_retrieved()
        {
            // Given
            var resource = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = SingleResourceReferenceId)
                .And(r => r.AbsentInLearningHub = false)
                .Build();
            A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(SingleResourceReferenceId))
                .Returns(resource);

            // When
            var result =
                (await learningHubResourceService.GetResourceByReferenceIdAndPopulateDeletedDetailsFromDatabase(
                    SingleResourceReferenceId
                ))!;

            // Then
            using (new AssertionScope())
            {
                result.resource.Should().BeEquivalentTo(resource);
                result.resource!.AbsentInLearningHub.Should().BeFalse();
                result.apiIsAccessible.Should().BeTrue();
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
                .With(r => r.RefId = SingleResourceReferenceId)
                .And(r => r.AbsentInLearningHub = false)
                .Build();
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
                result.resource!.AbsentInLearningHub.Should().BeFalse();
                result.apiIsAccessible.Should().BeFalse();
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
        public async Task
            GetResourceByReferenceIdAndPopulateDeletedDetailsFromDatabase_returns_fallback_data_if_API_throws_exception()
        {
            // Given
            var resource = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = SingleResourceReferenceId)
                .And(r => r.AbsentInLearningHub = false)
                .Build();
            A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(SingleResourceReferenceId))
                .Throws(new LearningHubResponseException("exception", HttpStatusCode.Unauthorized));
            A.CallTo(
                () => learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(
                    A<IEnumerable<int>>._
                )
            ).Returns(new List<ResourceReferenceWithResourceDetails> { resource });

            // When
            var result =
                (await learningHubResourceService.GetResourceByReferenceIdAndPopulateDeletedDetailsFromDatabase(
                    SingleResourceReferenceId
                ))!;

            // Then
            using (new AssertionScope())
            {
                result.resource.Should().BeEquivalentTo(resource);
                result.resource!.AbsentInLearningHub.Should().BeFalse();
                result.apiIsAccessible.Should().BeFalse();
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
        public async Task GetResourceByReferenceId_returns_null_if_API_throws_Not_found_exception()
        {
            // Given
            A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(SingleResourceReferenceId))
                .Throws(new LearningHubResponseException("exception", HttpStatusCode.NotFound));

            // When
            var result = (await learningHubResourceService.GetResourceByReferenceId(SingleResourceReferenceId))!;

            // Then
            using (new AssertionScope())
            {
                result.resource.Should().BeNull();
                result.apiIsAccessible.Should().BeTrue();
                A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(A<int>._))
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
        public async Task
            GetResourceByReferenceIdAndPopulateDeletedDetailsFromDatabase_returns_fallback_data_if_API_throws_Not_found_exception()
        {
            // Given
            A.CallTo(() => learningHubApiClient.GetResourceByReferenceId(SingleResourceReferenceId))
                .Throws(new LearningHubResponseException("exception", HttpStatusCode.NotFound));
            var resource = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = SingleResourceReferenceId)
                .And(r => r.AbsentInLearningHub = false)
                .Build();
            A.CallTo(
                () => learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(
                    A<IEnumerable<int>>._
                )
            ).Returns(new List<ResourceReferenceWithResourceDetails> { resource });

            // When
            var result =
                (await learningHubResourceService.GetResourceByReferenceIdAndPopulateDeletedDetailsFromDatabase(
                    SingleResourceReferenceId
                ))!;

            // Then
            using (new AssertionScope())
            {
                result.resource.Should().BeEquivalentTo(
                    resource,
                    options => options.Excluding(r => r.AbsentInLearningHub)
                );
                result.apiIsAccessible.Should().BeTrue();
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
                .With(r => r.RefId = 1).And(r => r.AbsentInLearningHub = false).Build();
            var resource2 = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = 2).And(r => r.AbsentInLearningHub = false).Build();
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
                result.apiIsAccessible.Should().BeTrue();
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
        public async Task
            GetBulkResourcesByReferenceIdsAndPopulateDeletedDetailsFromDatabase_returns_API_data_and_fetches_fallback_data_for_unmatched_reference_ids()
        {
            // Given
            var resource1 = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = 1).And(r => r.AbsentInLearningHub = false).Build();
            var resource2 = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = 2).And(r => r.AbsentInLearningHub = false).Build();
            var resource3 = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = 3).And(r => r.AbsentInLearningHub = false).Build();
            var resourceReferenceIdsToRetrieve = new[] { 1, 2, 3, 4 };
            var initiallyUnmatchedResourceReferenceIds = new List<int> { 3, 4 };
            var bulkResources = new BulkResourceReferences
            {
                ResourceReferences = new List<ResourceReferenceWithResourceDetails>
                {
                    resource1,
                    resource2,
                },
                UnmatchedResourceReferenceIds = initiallyUnmatchedResourceReferenceIds,
            };
            A.CallTo(() => learningHubApiClient.GetBulkResourcesByReferenceIds(A<IEnumerable<int>>._))
                .Returns(bulkResources);
            A.CallTo(
                    () => learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(
                        A<IEnumerable<int>>._
                    )
                )
                .Returns(new List<ResourceReferenceWithResourceDetails> { resource3 });

            // When
            var (bulkResourceReferences, apiIsAccessible) =
                (await learningHubResourceService.GetBulkResourcesByReferenceIdsAndPopulateDeletedDetailsFromDatabase(
                    resourceReferenceIdsToRetrieve
                ))!;

            // Then
            using (new AssertionScope())
            {
                bulkResourceReferences.ResourceReferences[0].Should().BeEquivalentTo(resource1);
                bulkResourceReferences.ResourceReferences[0].AbsentInLearningHub.Should().BeFalse();
                bulkResourceReferences.ResourceReferences[1].Should().BeEquivalentTo(resource2);
                bulkResourceReferences.ResourceReferences[1].AbsentInLearningHub.Should().BeFalse();
                bulkResourceReferences.ResourceReferences[2].Should().BeEquivalentTo(
                    resource3,
                    options => options.Excluding(r => r.AbsentInLearningHub)
                );
                bulkResourceReferences.ResourceReferences[2].AbsentInLearningHub.Should().BeTrue();
                bulkResourceReferences.UnmatchedResourceReferenceIds.Should().BeEquivalentTo(new List<int> { 4 });
                apiIsAccessible.Should().BeTrue();
                A.CallTo(
                        () => learningHubApiClient.GetBulkResourcesByReferenceIds(
                            A<IEnumerable<int>>.That.IsSameSequenceAs(resourceReferenceIdsToRetrieve)
                        )
                    )
                    .MustHaveHappenedOnceExactly();
                A.CallTo(
                        () => learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(
                            A<IEnumerable<int>>.That.IsSameSequenceAs(initiallyUnmatchedResourceReferenceIds)
                        )
                    )
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public async Task GetBulkResourcesByReferenceIds_returns_fallback_data_if_API_throws_exception()
        {
            // Given
            var resource1 = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = 1).And(r => r.AbsentInLearningHub = false).Build();
            var resource2 = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = 2).And(r => r.AbsentInLearningHub = false).Build();
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
                result.apiIsAccessible.Should().BeFalse();
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

        [Test]
        public async Task
            GetBulkResourcesByReferenceIdsAndPopulateDeletedDetailsFromDatabase_returns_fallback_data_without_marking_resources_as_deleted_if_API_throws_exception()
        {
            // Given
            var resource1 = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = 1).And(r => r.AbsentInLearningHub = false).Build();
            var resource2 = Builder<ResourceReferenceWithResourceDetails>.CreateNew()
                .With(r => r.RefId = 2).And(r => r.AbsentInLearningHub = false).Build();
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
            var (bulkResourceReferences, apiIsAccessible) =
                (await learningHubResourceService.GetBulkResourcesByReferenceIdsAndPopulateDeletedDetailsFromDatabase(
                    resourceReferenceIdsToRetrieve
                ))!;

            // Then
            using (new AssertionScope())
            {
                bulkResourceReferences.Should().BeEquivalentTo(bulkResources);
                bulkResourceReferences.ResourceReferences[0].AbsentInLearningHub.Should().BeFalse();
                bulkResourceReferences.ResourceReferences[1].AbsentInLearningHub.Should().BeFalse();
                apiIsAccessible.Should().BeFalse();
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
