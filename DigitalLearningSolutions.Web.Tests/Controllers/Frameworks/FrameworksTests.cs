namespace DigitalLearningSolutions.Web.Tests.Controllers.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Tests.TestHelpers.Frameworks;
    using DigitalLearningSolutions.Web.ViewModels.Frameworks;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;
    using System.Collections.Generic;

    public partial class FrameworkControllerTests
    {
        [Test]
        public void ViewFrameworks_Mine_action_should_return_view_result()
        {
            // Given
            var dashboardFrameworks = new[]
            {
                FrameworksHelper.CreateDefaultBrandedFramework(),
                FrameworksHelper.CreateDefaultBrandedFramework()
            };
            A.CallTo(() => frameworkService.GetFrameworksForAdminId(AdminId)).Returns(dashboardFrameworks);

            // When
            var result = controller.ViewFrameworks(null, "Created Date", "Descending", 1, "Mine");

            // Then
            var allFrameworksViewModel = new AllFrameworksViewModel(
                new List<BrandedFramework>(),
                null,
                "Created Date",
                "Descending",
                1
            );
            var myFrameworksViewModel = new MyFrameworksViewModel(
                dashboardFrameworks,
                null,
                "CreatedDate",
                "Descending",
                1,
                true);
            var expectedModel = new FrameworksViewModel(
                true,
                false,
                myFrameworksViewModel,
                allFrameworksViewModel
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
                FrameworksHelper.CreateDefaultBrandedFramework()
            };
            A.CallTo(() => frameworkService.GetAllFrameworks(AdminId)).Returns(dashboardFrameworks);

            // When
            var result = controller.ViewFrameworks();

            // Then
            var allFrameworksViewModel = new AllFrameworksViewModel(
                dashboardFrameworks,
                null,
                "FrameworkName",
                "Ascending",
                1
            );
            var myFrameworksViewModel = new MyFrameworksViewModel(
                new List<BrandedFramework>(),
                null,
                "Framework Name",
                "Ascending",
                1,
                true);
            var expectedModel = new FrameworksViewModel(
                true,
                false,
                myFrameworksViewModel,
                allFrameworksViewModel
            );
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
