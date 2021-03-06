﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.Support
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Controllers.Support;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class SupportControllerTests
    {
        private IFeatureManager featureManager = null!;

        [SetUp]
        public void Setup()
        {
            featureManager = A.Fake<IFeatureManager>();
            A.CallTo(() => featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem))
                .Returns(true);
        }

        [Test]
        public async Task Frameworks_Support_page_should_be_shown_for_valid_claims()
        {
            // Given
            var controller = new SupportController(featureManager)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: false, isFrameworkDeveloper: true);

            // When
            var result = await controller.Index("Frameworks");

            // Then
            result.Should().BeViewResult().WithViewName("Support");
        }

        [Test]
        public async Task Invalid_application_name_should_redirect_to_404_page()
        {
            // Given
            var controller = new SupportController(featureManager)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: true, isFrameworkDeveloper: true);

            // When
            var result = await controller.Index("Main");

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public async Task Home_page_should_be_shown_when_accessing_tracking_system_support_without_appropriate_claims()
        {
            // Given
            var controller = new SupportController(featureManager)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: false, isFrameworkDeveloper: true);

            // When
            var result = await controller.Index("TrackingSystem");

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task Home_page_should_be_shown_when_accessing_tracking_system_with_refactored_tracking_system_disabled()
        {
            // Given
            A.CallTo(() => featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem))
                .Returns(false);
            var controller = new SupportController(featureManager)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: false, isFrameworkDeveloper: true);

            // When
            var result = await controller.Index("TrackingSystem");

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task Home_page_should_be_shown_when_accessing_frameworks_support_without_appropriate_claims()
        {
            // Given
            var controller = new SupportController(featureManager)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: true, isFrameworkDeveloper: false);

            // When
            var result = await controller.Index("Frameworks");

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }
    }
}
