namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class ResourcesServiceTests
    {
        private IDownloadDataService downloadDataService = null!;
        private IResourcesService resourcesService = null!;

        [SetUp]
        public void SetUp()
        {
            downloadDataService = A.Fake<IDownloadDataService>();
            resourcesService = new ResourcesService(downloadDataService);
        }

        [Test]
        public void GetAllResources_maps_downloads_to_resource_categories_correctly()
        {
            // Given
            var downloads = Builder<Download>.CreateListOfSize(10)
                .TheFirst(3).With(d => d.Category = "Category 1")
                .TheNext(5).With(d => d.Category = "Category 2")
                .TheRest().With(d => d.Category = "Category 3")
                .Build();
            A.CallTo(() => downloadDataService.GetAllDownloads()).Returns(downloads);

            // When
            var result = resourcesService.GetAllResources().ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(3);
                result[0].Resources.Count().Should().Be(3);
                result[1].Resources.Count().Should().Be(5);
                result[2].Resources.Count().Should().Be(2);
            }
        }

        [Test]
        public void GetAllResources_orders_resource_categories_correctly()
        {
            // Given
            var downloads = Builder<Download>.CreateListOfSize(10)
                .TheFirst(3).With(d => d.Category = "B Category")
                .TheNext(5).With(d => d.Category = "C Category")
                .TheRest().With(d => d.Category = "A Category")
                .Build();
            A.CallTo(() => downloadDataService.GetAllDownloads()).Returns(downloads);

            // When
            var result = resourcesService.GetAllResources().ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(3);
                result[0].CategoryName.Should().Be("A Category");
                result[1].CategoryName.Should().Be("B Category");
                result[2].CategoryName.Should().Be("C Category");
            }
        }

        [Test]
        public void GetAllResources_orders_resources_correctly()
        {
            // Given
            var downloads = new[]
            {
                new Download { UploadDate = new DateTime(2021, 10, 1) },
                new Download { UploadDate = new DateTime(2021, 9, 1) },
                new Download { UploadDate = new DateTime(2021, 8, 1) },
            };
            A.CallTo(() => downloadDataService.GetAllDownloads()).Returns(downloads);

            // When
            var result = resourcesService.GetAllResources().ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(1);
                result.First().Resources.First().UploadDate.Should().Be(new DateTime(2021, 8, 1));
                result.First().Resources.Last().UploadDate.Should().Be(new DateTime(2021, 10, 1));
            }
        }
    }
}
