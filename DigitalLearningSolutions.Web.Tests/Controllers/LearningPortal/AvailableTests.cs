//namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningPortal
//{
//    using DigitalLearningSolutions.Web.Tests.TestHelpers;
//    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available;
//    using FakeItEasy;
//    using FluentAssertions;
//    using FluentAssertions.AspNetCore.Mvc;
//    using NUnit.Framework;

//    public partial class LearningPortalControllerTests
//    {
//        [Test]
//        public void Available_action_should_return_view_result()
//        {
//            // Given
//            var availableCourses = new[]
//            {
//                AvailableCourseHelper.CreateDefaultAvailableCourse(),
//                AvailableCourseHelper.CreateDefaultAvailableCourse()
//            };
//            A.CallTo(() => courseDataService.GetAvailableCourses(CandidateId, CentreId)).Returns(availableCourses);

//            // When
//            var result = controller.Available();

//            // Then
//            var expectedModel = new AvailablePageViewModel(
//                availableCourses,
//                null,
//                "Name",
//                "Ascending",
//                "",
//                1
//            );
//            result.Should().BeViewResult()
//                .Model.Should().BeEquivalentTo(expectedModel);
//        }

//        [Test]
//        public void Available_action_should_have_banner_text()
//        {
//            // Given
//            const string bannerText = "Banner text";
//            A.CallTo(() => centresDataService.GetBannerText(CentreId)).Returns(bannerText);

//            // When
//            var availableViewModel = AvailableCourseHelper.AvailableViewModelFromController(controller);

//            // Then
//            availableViewModel.BannerText.Should().Be(bannerText);
//        }
//    }
//}
