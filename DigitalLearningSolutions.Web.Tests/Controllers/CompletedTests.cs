namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public partial class LearningPortalControllerTests
    {
        [Test]
        public void Completed_action_should_return_view_result()
        {
            // Given
            var completedCourses = new[]
            {
                new Course { Id = 1, Name = "Course 1" },
                new Course { Id = 2, Name = "Course 2" }
            };
            A.CallTo(() => courseService.GetCompletedCourses()).Returns(completedCourses);

            // When
            var result = controller.Completed();

            // Then
            var expectedModel = new CompletedViewModel(completedCourses, "");
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Completed_action_should_have_banner_text()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var completedViewModel = CompletedCourseHelper.CompletedViewModelFromController(controller);

            // Then
            completedViewModel.BannerText.Should().Be(bannerText);
        }
    }
}
