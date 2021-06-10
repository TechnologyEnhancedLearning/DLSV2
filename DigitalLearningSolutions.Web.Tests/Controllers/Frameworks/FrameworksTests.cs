namespace DigitalLearningSolutions.Web.Tests.Controllers.Frameworks
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers.Frameworks;
    using DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;
    public partial class FrameworkControllerTests
    {
        [Test]
        public void ViewFrameworks_action_should_return_view_result()
        {
            // Given
            var dashboardFrameworks = new[]
            {
                FrameworksHelper.CreateDefaultBrandedFramework(),
                FrameworksHelper.CreateDefaultBrandedFramework()
            };
            A.CallTo(() => frameworkService.GetFrameworksForAdminId(AdminId)).Returns(dashboardFrameworks);

            // When
            var result = controller.ViewFrameworks();

            // Then
            var expectedModel = new MyFrameworksViewModel(
                dashboardFrameworks,
                null,
                "Created Date",
                "Descending",
                1,
                true
            );
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }
        [Test]
        public void FrameworksViewAll_action_should_return_view_result()
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
            var result = controller.FrameworksViewAll();

            // Then
            var expectedModel = new AllFrameworksViewModel(
                dashboardFrameworks,
                null,
                "Framework Name",
                "Ascending",
                1
            );
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
