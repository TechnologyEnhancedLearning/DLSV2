namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningContent
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class LearningContentControllerTests
    {
        private const double PopularityRating = 1;

        private static readonly ApplicationDetails ApplicationDetail = new ApplicationDetails
        {
            ApplicationId = 1,
            ApplicationName = "Application Name",
            CategoryName = "Category Name",
            CourseTopic = "Course Topic",
            CourseTopicId = 1,
            DiagAssess = true,
            PLAssess = true,
            SearchableName = "Searchable Name",
        };

        private static readonly Tutorial Tutorial = new Tutorial
        {
            TutorialId = 1,
            TutorialName = "Tutorial Name",
            Status = true,
            DiagStatus = true,
            OverrideTutorialMins = 0,
            AverageTutMins = 1,
        };

        private static readonly IEnumerable<Tutorial> Tutorials = new List<Tutorial> { Tutorial };

        private static readonly Section Section = new Section
        {
            SectionId = 1,
            SectionName = "Section Name",
            Tutorials = Tutorials,
        };

        private static readonly IEnumerable<Section> Sections = new List<Section> { Section };

        private readonly IEnumerable<ApplicationWithSections> applications = new List<ApplicationWithSections>
        {
            new ApplicationWithSections(ApplicationDetail, Sections, PopularityRating),
        };

        private readonly BrandDetail brand = new BrandDetail
        {
            Active = true,
            BrandDescription = null,
            BrandID = 1,
            BrandImage = null,
            BrandLogo = null,
            BrandName = "Brand Name",
            ContactEmail = null,
            ImageFileType = null,
            IncludeOnLanding = true,
            OrderByNumber = 1,
        };

        private const string CookieName = "BrandCoursesFilter";
        private IBrandsService brandsService = null!;
        private LearningContentController controller = null!;
        private ICourseService courseService = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private ITutorialService tutorialService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;

        [SetUp]
        public void Setup()
        {
            brandsService = A.Fake<IBrandsService>();
            courseService = A.Fake<ICourseService>();
            tutorialService = A.Fake<ITutorialService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();

            A.CallTo(
                () => brandsService.GetPublicBrandById(A<int>._)
            ).Returns(brand);

            A.CallTo(
                () => courseService.GetApplicationsThatHaveSectionsByBrandId(A<int>._)
            ).Returns(applications);

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
                .WithMockTempData(); ;
        }

        [Test]
        public void Index_returns_not_found_with_null_brand()
        {
            // Given
            A.CallTo(() => brandsService.GetPublicBrandById(1)).Returns(null);

            // When
            var result = controller.Index(1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_calls_expected_methods_and_returns_view()
        {
            // When
            var result = controller.Index(1);

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
