namespace DigitalLearningSolutions.Web.Tests.ViewModels.Support
{
    using System;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.ViewModels.Support.Resources;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class ResourcesItemViewModelTests
    {
        [Test]
        public void ResourcesItemViewModel_populates_properties_as_expected()
        {
            // Given
            var resource = new Resource
            {
                Category = "category",
                Description = "description",
                UploadDateTime = new DateTime(2021, 1, 1),
                FileSize = 120,
                Tag = "tag",
                FileName = "file.pdf",
            };

            // When
            var result = new ResourceViewModel(resource, "www.test.com");

            // Then
            using (new AssertionScope())
            {
                result.Resource.Should().Be("description (.pdf)");
                result.Date.Should().BeEquivalentTo("01/01/2021");
                result.Size.Should().Be("120B");
                result.DownloadUrl.Should().Be("www.test.com/tracking/download?content=tag");
            }
        }
    }
}
