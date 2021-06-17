namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningSolutions
{
    using DigitalLearningSolutions.Web.Controllers.LearningSolutions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class SupportControllerTests
    {
        private SupportController controller = null!;

        [SetUp]
        public void SetUp()
        {
            controller = new SupportController();
        }

        [TestCase("TrackingSystem")]
        [TestCase("Frameworks")]
        public void Support_page_should_be_shown_for_valid_application_names(string applicationName)
        {
            // When
            var result = controller.Index(applicationName);

            // Then
            result.Should().BeViewResult().WithViewName("/Views/LearningSolutions/Support.cshtml");
        }

        [Test]
        public void Invalid_application_name_should_redirect_to_404_page()
        {
            // When
            var result = controller.Index("SomeInvalidApplication");

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("StatusCode")
                .WithControllerName("LearningSolutions").WithRouteValue("code",404 );
        }
    }
}
