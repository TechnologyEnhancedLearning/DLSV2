namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using DigitalLearningSolutions.Web.Controllers;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class HomeControllerTests
    {
        private HomeController controller;


        [SetUp]
        public void SetUp()
        {
            controller = new HomeController();
        }

        [Test]
        public void Index_action_should_return_view_result()
        {
            // When
            var result = controller.Index();

            // Then
            result.Should().BeViewResult();
        }
    }
}
