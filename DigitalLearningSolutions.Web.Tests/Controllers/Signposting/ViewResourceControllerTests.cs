namespace DigitalLearningSolutions.Web.Tests.Controllers.Signposting
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.Signposting;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class ViewResourceControllerTests
    {
        private ViewResourceController controller = null!;
        private ILearningResourceReferenceDataService resourceDataService = null!;

        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            var securityService = A.Fake<ILearningHubSsoSecurityService>();
            var guidService = A.Fake<IGuidService>();
            var config = A.Fake<IConfiguration>();

            A.CallTo(() => config[ConfigHelper.LearningHubAuthApiClientCode]).Returns("test");
            A.CallTo(() => config[ConfigHelper.LearningHubAuthApiBaseUrl]).Returns("www.example.com");
            A.CallTo(() => securityService.GenerateHash(A<string>._)).Returns("hash_brown");
            A.CallTo(() => guidService.NewGuid()).Returns(Guid.Empty);

            userService = A.Fake<IUserService>();
            resourceDataService = A.Fake<ILearningResourceReferenceDataService>();
            controller = new ViewResourceController(
                    userService,
                    resourceDataService,
                    securityService,
                    guidService,
                    config
                ).WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void ViewResourceController_Index_returns_redirect_to_login_result_if_user_linked()
        {
            // Given
            A.CallTo(() => userService.GetDelegateUserLearningHubAuthId(A<int>._)).Returns(1);
            A.CallTo(() => resourceDataService.GetLearningHubResourceLinkById(5)).Returns("De/Humani/Corporis/Fabrica");

            // When
            var result = controller.Index(5);

            // Then
            result.Should().BeRedirectResult().WithUrl(
                "www.example.com/login?clientcode=test&userid=1&hash=hash_brown&endclientUrl=De%2fHumani%2fCorporis%2fFabrica"
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

            // When
            var result = controller.Index(5);

            // Then
            result.Should().BeRedirectResult().WithUrl(
                $"www.example.com/create-user?clientcode=test&state={Guid.Empty}_refId:5&hash=hash_brown"
            );
        }
    }
}
