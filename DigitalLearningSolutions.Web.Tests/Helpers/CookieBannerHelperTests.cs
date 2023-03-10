namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;
    public class CookieBannerHelperTests
    {
        [Test]
        public void SetCookieBanner_creates_cookie_with_correct_content()
        {
            // Given
            const int adminId = 42;
            var testDate = new DateTime(2021, 8, 23);
            var expectedExpiry = testDate.AddHours(24);
            var cookies = A.Fake<IResponseCookies>();

            // When
            cookies.SetDLSBannerCookie("Yes", testDate);

            // Then
            A.CallTo(
                () => cookies.Append(
                    SystemNotificationCookieHelper.CookieName,
                    "42",
                    A<CookieOptions>.That.Matches(co => co.Expires == expectedExpiry)
                )
            ).MustHaveHappened();
        }     
    }
}
