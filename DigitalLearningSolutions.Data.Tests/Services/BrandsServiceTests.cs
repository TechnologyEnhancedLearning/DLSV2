namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Models.Enums;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class BrandsServiceTests
    {
        private IBrandsDataService brandsDataService = null!;
        private BrandsService brandsService = null!;

        [SetUp]
        public void Setup()
        {
            brandsDataService = A.Fake<IBrandsDataService>();
            brandsService = new BrandsService(brandsDataService);
        }

        [Test]
        public void GetAllPublicBrands_calls_data_service_method_and_returns_public_results()
        {
            // Given
            var expectedBrands = Builder<BrandDetail>.CreateListOfSize(5).All().With(b => b.IncludeOnLanding = true)
                .Build();
            A.CallTo(() => brandsDataService.GetAllBrands())
                .Returns(expectedBrands);

            // When
            var result = brandsService.GetPublicBrandsDetails();

            // Then
            result.Should().Equal(expectedBrands);
        }

        [Test]
        public void GetAllPublicBrands_calls_data_service_method_and_does_not_returns_private_results()
        {
            // Given
            var expectedBrands = Builder<BrandDetail>.CreateListOfSize(5).All().With(b => b.IncludeOnLanding = false)
                .Build();
            A.CallTo(() => brandsDataService.GetAllBrands())
                .Returns(expectedBrands);
            var emptyList = Enumerable.Empty<BrandDetail>();

            // When
            var result = brandsService.GetPublicBrandsDetails();

            // Then
            result.Should().Equal(emptyList);
        }

        [Test]
        public void GetAllPublicBrands_returns_empty_when_data_service_returns_null()
        {
            // Given
            var emptyList = Enumerable.Empty<BrandDetail>();

            A.CallTo(() => brandsDataService.GetAllBrands())
                .Returns(emptyList);

            // When
            var result = brandsService.GetPublicBrandsDetails();

            // Then
            result.Should().BeEmpty();
            A.CallTo(() => brandsDataService.GetAllBrands())
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetAllPublicBrandsById_calls_data_service_method_and_returns_expected_result()
        {
            var expectedBrand = Builder<BrandDetail>.CreateNew().Build();
            A.CallTo(() => brandsDataService.GetPublicBrandById(1))
                .Returns(expectedBrand);

            // When
            var result = brandsService.GetPublicBrandById(1);

            // Then
            result.Should().BeEquivalentTo(expectedBrand);
        }

        [Test]
        public void GetAllPublicBrandsById_returns_empty_when_data_service_returns_null()
        {
            A.CallTo(() => brandsDataService.GetPublicBrandById(1))
                .Returns(null);

            // When
            var result = brandsService.GetPublicBrandById(1);

            // Then
            result.Should().BeNull();
            A.CallTo(() => brandsDataService.GetPublicBrandById(1))
                .MustHaveHappenedOnceExactly();
        }
    }
}
