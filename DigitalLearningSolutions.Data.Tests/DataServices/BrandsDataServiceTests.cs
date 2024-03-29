namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class BrandsDataServiceTests
    {
        private BrandsDataService brandsDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            brandsDataService = new BrandsDataService(connection);
        }

        [Test]
        public void GetAllBrands_should_return_expected_items()
        {
            // Given
            var expectedBrand = new BrandDetail
            {
                BrandID = 6,
                BrandName = "Local content",
                BrandDescription = "Content hosted by your organisation on the DLS Tracking System.",
                BrandImage = null,
                ImageFileType = null,
                IncludeOnLanding = false,
                ContactEmail = null,
                OwnerOrganisationId = 0,
                Active = true,
                OrderByNumber = 6,
                BrandLogo = null,
                PopularityHigh = 177,
            };

            var expectedIndexes = new[] { 1, 2, 3, 4, 6, 8, 9 };

            // When
            var result = brandsDataService.GetAllBrands().ToList();

            // Then
            result.Select(b => b.BrandID).Should().BeEquivalentTo(expectedIndexes);
            result.SingleOrDefault(b => b.BrandID == 6).Should().BeEquivalentTo(expectedBrand);
        }

        [Test]
        public void GetBrandById_should_return_expected_item()
        {
            // When
            var result = brandsDataService.GetBrandById(1);

            // Then
            result!.BrandName.Should().BeEquivalentTo("IT Skills Pathway");
        }

        [Test]
        public void GetBrandById_should_return_null_if_id_does_not_exist()
        {
            // When
            var result = brandsDataService.GetBrandById(5);

            // Then
            result.Should().BeNull();
        }
    }
}
