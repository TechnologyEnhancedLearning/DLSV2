namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Collections.Generic;
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
            var brand6 = new BrandDetail
            {
                BrandID = 6,
                BrandName = "Local content",
                BrandDescription = "Content hosted by your organisation on the DLS Tracking System.",
                BrandImage = null,
                ImageFileType = null,
                IncludeOnLanding = false,
                ContactEmail = null,
                OwnerOrganisationID = 0,
                Active = true,
                OrderByNumber = 6,
                BrandLogo = null,
                PopularityHigh = 177,
            };

            var indexes = new int[] { 1, 2, 3, 4, 6, 8, 9 };

            // When
            var result = brandsDataService.GetAllBrands().ToList();
            var resultIds = result.Select(b => b.BrandID);
            var result6 = result.SingleOrDefault(b => b.BrandID == 6);

            // Then
            resultIds.Should().BeEquivalentTo(indexes);
            result6.Should().BeEquivalentTo(brand6);
        }

        [Test]
        public void GetAllPublicBrandById_should_return_expected_item()
        {
            // When
            var result = brandsDataService.GetPublicBrandById(1);

            // Then
            result.Should().NotBeNull();
            result?.BrandName.Should().BeEquivalentTo("IT Skills Pathway");
        }

        [Test]
        public void GetAllPublicBrandById_should_not_return_private_item()
        {
            // When
            var result = brandsDataService.GetPublicBrandById(4);

            // Then
            result.Should().BeNull();
        }
    }


}
