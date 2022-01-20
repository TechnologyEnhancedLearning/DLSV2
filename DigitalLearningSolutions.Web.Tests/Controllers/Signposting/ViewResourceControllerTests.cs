namespace DigitalLearningSolutions.Web.Tests.Controllers.Signposting
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.Signposting;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class ViewResourceControllerTests
    {
        private ViewResourceController controller = null!;

        private ILearningResourceReferenceDataService resourceDataService = null!;
        private ILearningHubAuthApiUrlService urlService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            urlService = A.Fake<ILearningHubAuthApiUrlService>();
            userService = A.Fake<IUserService>();
            resourceDataService = A.Fake<ILearningResourceReferenceDataService>();

            controller = new ViewResourceController(
                    userService,
                    resourceDataService,
                    urlService
                ).WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void ViewResourceController_Index_returns_redirect_to_login_result_if_user_linked()
        {
            // Given
            var authId = 1;
            var resourceUrl = "De/Humani/Corporis/Fabrica";

            A.CallTo(() => userService.GetDelegateUserLearningHubAuthId(A<int>._)).Returns(authId);
            A.CallTo(() => resourceDataService.GetLearningHubResourceLinkById(5)).Returns(resourceUrl);
            A.CallTo(() => urlService.GetLoginUrlForDelegateAuthIdAndResourceUrl(resourceUrl, authId))
                .Returns("www.example.com/login");

            // When
            var result = controller.Index(5);

            // Then
            result.Should().BeRedirectResult().WithUrl(
                "www.example.com/login"
            );
        }

        [Test]
        public void
            ViewResourceController_Index_returns_not_found_result_if_user_linked_but_no_relevant_resource_entry()
        {
            // Given
            A.CallTo(() => userService.GetDelegateUserLearningHubAuthId(A<int>._)).Returns(1);
            A.CallTo(() => resourceDataService.GetLearningHubResourceLinkById(5)).Returns(null);

            // When
            var result = controller.Index(5);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void ViewResourceController_Index_returns_redirect_result_to_create_user_if_user_not_linked()
        {
            // Given
            A.CallTo(() => userService.GetDelegateUserLearningHubAuthId(A<int>._)).Returns(null);
            A.CallTo(() => urlService.GetLinkingUrlForResource(5)).Returns("www.example.com/link");

            // When
            var result = controller.Index(5);

            // Then
            result.Should().BeRedirectResult().WithUrl(
                "www.example.com/link"
            );
        }
    }
}
