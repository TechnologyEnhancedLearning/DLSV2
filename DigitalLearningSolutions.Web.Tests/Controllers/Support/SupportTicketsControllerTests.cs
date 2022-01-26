namespace DigitalLearningSolutions.Web.Tests.Controllers.Support
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Controllers.Support;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class SupportTicketsControllerTests
    {
        private IFeatureManager featureManager = null!;
        private IConfiguration configuration = null!;

        [SetUp]
        public void Setup()
        {
            featureManager = A.Fake<IFeatureManager>();
            configuration = A.Fake<IConfiguration>();
            A.CallTo(() => featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem))
                .Returns(true);
        }

        [Test]
        public void Index_page_should_be_shown_by_index_method()
        {
            // Given
            var controller = new SupportTicketsController(featureManager, configuration)
                .WithDefaultContext();

            // When
            var result = controller.Index(DlsSubApplication.TrackingSystem);

            // Then
            result.Should().BeViewResult().WithViewName("Index");
        }
    }
}
