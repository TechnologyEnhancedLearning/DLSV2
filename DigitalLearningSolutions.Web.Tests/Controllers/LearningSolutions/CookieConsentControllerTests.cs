using DigitalLearningSolutions.Data.Utilities;
using DigitalLearningSolutions.Web.Controllers.LearningSolutions;
using DigitalLearningSolutions.Web.ViewModels.Common;
using DigitalLearningSolutions.Web.ViewModels.LearningSolutions;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;


namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningSolutions
{
    public class CookieConsentControllerTests
    {
        private IClockUtility clockUtility = null!;
        private CookieConsentController controller = null!;

        [SetUp]
        public void Setup()
        {
            var logger = A.Fake<ILogger<CookieConsentController>>();
            clockUtility = A.Fake<IClockUtility>();
            controller = new CookieConsentController(clockUtility, logger);
        }

        [Test]
        public void Cookie_page_consent_should_match()
        {
            // When
            var result = controller.CookiePolicy();

            // Then

            var expectedModel = new CookieConsentViewModel();
            result.Should().BeViewResult()
                .ModelAs<CookieConsentViewModel>().UserConsent.Should().Be(expectedModel.UserConsent);
        }


        [Test]
        public void Cookie_banner_consent_should_redirect_back_correct_view()
        {
            // Given
            const string consent = "true";
            const string path = "/Home/Welcome";

            // When
            var result = controller.CookieConsentConfirmation(consent,path);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home")
               .WithActionName("Welcome");
        }

        [Test]
        public void Cookie_policy_consent_should_redirect_correct_view()
        {
            // Given
            var expectedModel = new CookieConsentViewModel();

            // When
            var result = controller.CookiePolicy(expectedModel);

            // Then
            result.Should().BeViewResult().WithViewName("CookieConfirmation");
        }
    }
}

