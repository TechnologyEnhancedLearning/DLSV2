namespace DigitalLearningSolutions.Web.Tests.ViewComponents
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
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
                ViewContext = viewContext
            };
        }

        [Test]
        public void CurrentFiltersViewComponent_selects_expected_filters_to_display()
        {
            // Given
            var viewComponent = new CurrentFiltersViewComponent { ViewComponentContext = viewComponentContext };
            var categories = new[] { "Word", "Excel" };
            const string searchString = "test";
            var inputViewModel = new CentreAdministratorsViewModel(
                1,
                new List<AdminUser>(),
                categories,
                searchString,
                "SearchableName",
                "Ascending",
                $"CategoryName|Word\r\n{AdminFilterOptions.CentreAdministrator.Filter}",
                1
            );
            var expectedAppliedFilters = new List<AppliedFilterViewModel>
            {
                new AppliedFilterViewModel(AdminFilterOptions.CentreAdministrator.DisplayText, "Role"),
                new AppliedFilterViewModel("Word", "Category")
            };

            var expectedFilterViewModel = new CurrentFiltersViewModel(expectedAppliedFilters, searchString);

            // When
            var model = viewComponent.Invoke(inputViewModel).As<ViewViewComponentResult>().ViewData.Model
                .As<CurrentFiltersViewModel>();

            // Then
            model.Should().BeEquivalentTo(expectedFilterViewModel);
        }
    }
}
