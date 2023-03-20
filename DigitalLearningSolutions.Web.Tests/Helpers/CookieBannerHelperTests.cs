namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using MailKit.Net.Smtp;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;
    public class CookieBannerHelperTests
    {
        private const string CookieName = "Dls-cookie-consent";

        [Test]
        public void SetDLSCookieBannerCookie_creates_cookie_with_correct_content()
        {
            // Given
            var testDate = new DateTime(2021, 8, 23);
            var expectedExpiry = testDate.AddDays(365);
            var cookies = A.Fake<IResponseCookies>();

            // When
            cookies.SetDLSBannerCookie(CookieName, "Yes", testDate);

            // Then
            A.CallTo(
                () => cookies.Append(
                    CookieName,
                    "Yes",
                    A<CookieOptions>._
                )
            )
            .MustHaveHappened();
        }
        [Test]
        public void HasDLSCookieBannerCookie_is_true_when_cookie_exists_with_matching_ID()
        {
            // Given
            const string cookieValue = "true";
            var cookies = ControllerContextHelper.SetUpFakeRequestCookieCollection(
                CookieName,
                "true"
            );

            // When
            var result = cookies.HasDLSBannerCookie(CookieName,cookieValue);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void HasDLSCookieBannerCookie_is_false_when_cookie_exists_with_non_matching_ID()
        {
            // Given
            const string cookieValue = "false";
            var cookies = ControllerContextHelper.SetUpFakeRequestCookieCollection(
                CookieName,
                "randomvalue"
            );

            // When
            var result = cookies.HasDLSBannerCookie(CookieName, cookieValue);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void HasDLSCookieBannerCookie_is_false_when_no_cookie_exists()
        {
            // Given
            const string cookieValue = "";
            var cookies = A.Fake<IRequestCookieCollection>();

            // When
            var result = cookies.HasDLSBannerCookie(CookieName, cookieValue);

            // Then
            result.Should().BeFalse();
        }
    }
}
