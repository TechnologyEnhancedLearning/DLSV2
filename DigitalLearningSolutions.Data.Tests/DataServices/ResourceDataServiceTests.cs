namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class ResourceDataServiceTests
    {
        private IResourceDataService resourceDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            resourceDataService = new ResourceDataService(connection);
        }

        [Test]
        public void GetAllResources_gets_expected_downloads()
        {
            // When
            var result = resourceDataService.GetAllResources().ToList();

            // Then
            result.Should().HaveCount(96);
            result.Should().ContainEquivalentOf(
                new Resource
                {
                    Category = "Case Studies",
                    Description = "Template file for case studies",
                    FileName = "casestudytemplate.pub",
                    FileSize = 205312,
                    Tag = "casestudytemplate",
                    UploadDateTime = new DateTime(2015, 05, 05, 09, 22, 36),
                }
            );
        }
    }
}
