namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningContent
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class LearningContentControllerTests
    {
        private const string CookieName = "BrandCoursesFilter";
        private IBrandsService brandsService = null!;
        private LearningContentController controller = null!;
        private ICourseService courseService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private ITutorialService tutorialService = null!;

        [SetUp]
        public void Setup()
        {
            brandsService = A.Fake<IBrandsService>();
            courseService = A.Fake<ICourseService>();
            tutorialService = A.Fake<ITutorialService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();

            const string cookieValue = "ActiveStatus|Active|false";

            controller = new LearningContentController(
                    brandsService,
                    tutorialService,
                    courseService,
                    searchSortFilterPaginateService
                ).WithMockHttpContext(httpRequest, CookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void Index_returns_not_found_with_null_brand()
        {
            // Given
            A.CallTo(() => brandsService.GetPublicBrandById(A<int>._)).Returns(null);

            // When
            var result = controller.Index(1);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => tutorialService.GetPublicTutorialSummariesForBrand(A<int>._)).MustNotHaveHappened();
                A.CallTo(() => courseService.GetApplicationsThatHaveSectionsByBrandId(A<int>._)).MustNotHaveHappened();
                A.CallTo(
                    () => searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                        A<IEnumerable<ApplicationWithSections>>._,
                        A<SearchSortFilterAndPaginateOptions>._
                    )
                ).MustNotHaveHappened();
                result.Should().BeNotFoundResult();
            }
        }

        [Test]
        public void Index_calls_expected_methods_and_returns_view()
        {
            // Given
            const int brandId = 1;
            var brand = new BrandDetail
            {
                BrandID = brandId,
                BrandName = "Brand Name",
            };
            A.CallTo(() => brandsService.GetPublicBrandById(brandId)).Returns(brand);

            var applications = Builder<ApplicationWithSections>.CreateListOfSize(10).All()
                .With(a => a.Sections = new List<Section>()).Build();
            A.CallTo(
                () => courseService.GetApplicationsThatHaveSectionsByBrandId(A<int>._)
            ).Returns(applications);

            // When
            var result = controller.Index(brandId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => brandsService.GetPublicBrandById(1)).MustHaveHappened();
                A.CallTo(() => courseService.GetApplicationsThatHaveSectionsByBrandId(1)).MustHaveHappened();
                result.Should().BeViewResult().WithDefaultViewName();
            }
        }
    }
}
