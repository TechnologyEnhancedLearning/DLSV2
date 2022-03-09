namespace DigitalLearningSolutions.Web.Tests.ViewComponents
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.ViewComponents;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewComponents;
    using NUnit.Framework;

    public class CurrentFiltersViewComponentTests
    {
        private ViewComponentContext viewComponentContext = null!;

        [SetUp]
        public void Setup()
        {
            var httpContext = new DefaultHttpContext();

            var viewContext = new ViewContext { HttpContext = httpContext };

            viewComponentContext = new ViewComponentContext
            {
                ViewContext = viewContext,
            };
        }

        [Test]
        public void CurrentFiltersViewComponent_selects_expected_filters_to_display()
        {
            // Given
            var viewComponent = new CurrentFiltersViewComponent { ViewComponentContext = viewComponentContext };
            var categories = new[] { "Word", "Excel" };
            const string searchString = "test";

            var availableFilters = new List<FilterModel>
            {
                new FilterModel("Role", "Role", AdministratorsViewModelFilterOptions.RoleOptions),
                new FilterModel(
                    "CategoryName",
                    "Category",
                    AdministratorsViewModelFilterOptions.GetCategoryOptions(categories)
                ),
                new FilterModel(
                    "AccountStatus",
                    "Account Status",
                    AdministratorsViewModelFilterOptions.AccountStatusOptions
                ),
            };

            var inputViewModel = new CentreAdministratorsViewModel(
                1,
                new SearchSortFilterPaginateResult<AdminUser>(
                    new PaginateResult<AdminUser>(new List<AdminUser>(), 1, 1, 10, 0),
                    searchString,
                    null,
                    null,
                    "CategoryName|CategoryName|Word╡Role|IsCentreAdmin|true"
                ),
                availableFilters,
                UserTestHelper.GetDefaultAdminUser()
            );
            var expectedAppliedFilters = new List<AppliedFilterViewModel>
            {
                new AppliedFilterViewModel(
                    AdminRoleFilterOptions.CentreAdministrator.DisplayText,
                    "Role",
                    AdminRoleFilterOptions.CentreAdministrator.FilterValue
                ),
                new AppliedFilterViewModel("Word", "Category", "CategoryName|CategoryName|Word"),
            };

            var expectedFilterViewModel = new CurrentFiltersViewModel(
                expectedAppliedFilters,
                searchString,
                new Dictionary<string, string>()
            );

            // When
            var model = viewComponent.Invoke(inputViewModel).As<ViewViewComponentResult>().ViewData.Model
                .As<CurrentFiltersViewModel>();

            // Then
            model.Should().BeEquivalentTo(expectedFilterViewModel);
        }
    }
}
