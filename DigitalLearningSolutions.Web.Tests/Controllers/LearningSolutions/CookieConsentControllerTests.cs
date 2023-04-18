using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Utilities;
using DigitalLearningSolutions.Web.Controllers.LearningSolutions;
using DigitalLearningSolutions.Web.ViewModels.Common;
using DigitalLearningSolutions.Web.ViewModels.LearningSolutions;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;


namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningSolutions
{
    public class CookieConsentControllerTests
    {
        private IConfigDataService configDataService = null!;
        private IConfiguration configuration = null!;
        private IClockUtility clockUtility = null!;
        private CookieConsentController controller = null!;
        private IConfiguration config = null!;

        [SetUp]
        public void Setup()
        {
            var logger = A.Fake<ILogger<CookieConsentController>>();
            configDataService = A.Fake<IConfigDataService>();            
            clockUtility = A.Fake<IClockUtility>();
            configuration = A.Fake<IConfiguration>();
            controller = new CookieConsentController(configDataService, configuration, clockUtility, logger);          
        }

        [Test]
        public void Cookie_page_consent_should_match()
        {
            // When
            var result = controller.CookiePolicy();

            // Then

            var expectedModel = new CookieConsentViewModel(string.Empty);
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
            var result = controller.CookieConsentConfirmation(consent, path);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home")
               .WithActionName("Welcome");
        }

        [Test]
        public void Cookie_policy_consent_should_redirect_correct_view()
        {
            // Given
            var expectedModel = new CookieConsentViewModel(string.Empty);

            // When
            var result = controller.CookiePolicy(expectedModel);

            // Then
            result.Should().BeViewResult().WithViewName("CookieConfirmation");
        }
    }
}

