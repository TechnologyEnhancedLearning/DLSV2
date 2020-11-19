namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningPortal
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed;
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
                CompletedCourseHelper.CreateDefaultCompletedCourse(),
                CompletedCourseHelper.CreateDefaultCompletedCourse()
            };
            var bannerText = "bannerText";
            A.CallTo(() => courseService.GetCompletedCourses(CandidateId)).Returns(completedCourses);
            A.CallTo(() => centresService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var result = controller.Completed();

            // Then
            var expectedModel = new CompletedPageViewModel(
                completedCourses,
                config,
                null,
                "Completed Date",
                "Descending",
                bannerText,
                1
            );
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
