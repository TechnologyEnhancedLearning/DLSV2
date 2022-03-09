namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningPortal
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;
    using AvailableCourseHelper = DigitalLearningSolutions.Web.Tests.TestHelpers.AvailableCourseHelper;

    public partial class LearningPortalControllerTests
    {
        [Test]
        public void Available_action_should_return_view_result()
        {
            // Given
            var availableCourses = new[]
            {
                AvailableCourseHelper.CreateDefaultAvailableCourse(),
                AvailableCourseHelper.CreateDefaultAvailableCourse(),
            };
            A.CallTo(() => courseDataService.GetAvailableCourses(CandidateId, CentreId)).Returns(availableCourses);
            SearchSortFilterAndPaginateTestHelper
                .GivenACallToSearchSortFilterPaginateServiceReturnsResult<AvailableCourse>(
                    searchSortFilterPaginateService
                );

            // When
            var result = controller.Available();

            // Then
            var expectedModel = new AvailablePageViewModel(
                new SearchSortFilterPaginateResult<AvailableCourse>(
                    new PaginateResult<AvailableCourse>(availableCourses, 1, 1, 10, 2),
                    null,
                    "Name",
                    "Ascending",
                    null
                ),
                ""
            );
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Available_action_should_have_banner_text()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresDataService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var availableViewModel = AvailableCourseHelper.AvailableViewModelFromController(controller);

            // Then
            availableViewModel.BannerText.Should().Be(bannerText);
        }
    }
}
