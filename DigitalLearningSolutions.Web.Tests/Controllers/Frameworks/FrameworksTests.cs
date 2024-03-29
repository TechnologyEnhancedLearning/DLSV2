﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.Frameworks
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;    
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers.Frameworks;
    using DigitalLearningSolutions.Web.ViewModels.Frameworks;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public partial class FrameworkControllerTests
    {
        [Test]
        public void ViewFrameworks_Mine_action_should_return_view_result()
        {
            // Given
            var dashboardFrameworks = new[]
            {
                FrameworksHelper.CreateDefaultBrandedFramework(),
                FrameworksHelper.CreateDefaultBrandedFramework(),
            };
            A.CallTo(() => frameworkService.GetFrameworksForAdminId(AdminId)).Returns(dashboardFrameworks);
            SearchSortFilterAndPaginateTestHelper
                .GivenACallToSearchSortFilterPaginateServiceReturnsResult<BrandedFramework>(
                    searchSortFilterPaginateService
                );

            // When
            var result = controller.ViewFrameworks(null, "CreatedDate", "Descending", 1, "Mine");

            // Then
            var allFrameworksViewModel = new AllFrameworksViewModel(
                new SearchSortFilterPaginationResult<BrandedFramework>(
                    new PaginationResult<BrandedFramework>(new List<BrandedFramework>(), 1, 1, 12, 0, true),
                    null,
                    "CreatedDate",
                    "Descending",
                    null
                )
            );
            var myFrameworksViewModel = new MyFrameworksViewModel(
                new SearchSortFilterPaginationResult<BrandedFramework>(
                    new PaginationResult<BrandedFramework>(dashboardFrameworks, 1, 1, 12, 2, true),
                    null,
                    "CreatedDate",
                    "Descending",
                    null
                ),
                true
            );
            var expectedModel = new FrameworksViewModel(
                true,
                false,
                myFrameworksViewModel,
                allFrameworksViewModel,
                FrameworksTab.MyFrameworks
            );
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void ViewFrameworks_All_action_should_return_view_result()
        {
            // Given
            var dashboardFrameworks = new[]
            {
                FrameworksHelper.CreateDefaultBrandedFramework(),
                FrameworksHelper.CreateDefaultBrandedFramework(),
                FrameworksHelper.CreateDefaultBrandedFramework(),
            };
            A.CallTo(() => frameworkService.GetAllFrameworks(AdminId)).Returns(dashboardFrameworks);
            SearchSortFilterAndPaginateTestHelper
                .GivenACallToSearchSortFilterPaginateServiceReturnsResult<BrandedFramework>(
                    searchSortFilterPaginateService
                );

            // When
            var result = controller.ViewFrameworks();

            // Then
            var allFrameworksViewModel = new AllFrameworksViewModel(
                new SearchSortFilterPaginationResult<BrandedFramework>(
                    new PaginationResult<BrandedFramework>(dashboardFrameworks, 1, 1, 12, 3, true),
                    null,
                    "FrameworkName",
                    "Ascending",
                    null
                )
            );
            var myFrameworksViewModel = new MyFrameworksViewModel(
                new SearchSortFilterPaginationResult<BrandedFramework>(
                    new PaginationResult<BrandedFramework>(new List<BrandedFramework>(), 1, 1, 12, 0, true),
                    null,
                    "FrameworkName",
                    "Ascending",
                    null
                ),
                true
            );
            var expectedModel = new FrameworksViewModel(
                true,
                false,
                myFrameworksViewModel,
                allFrameworksViewModel,
                FrameworksTab.AllFrameworks
            );
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
