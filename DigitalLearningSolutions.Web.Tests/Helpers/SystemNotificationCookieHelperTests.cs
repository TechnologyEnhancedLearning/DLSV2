namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class SystemNotificationCookieHelperTests
    {
        [Test]
        public void SetSkipSystemNotificationCookie_creates_cookie_with_correct_content()
        {
            // Given
            const int adminId = 42;
            var testDate = new DateTime(2021, 8, 23);
            var expectedExpiry = testDate.AddHours(24);
            var cookies = A.Fake<IResponseCookies>();

            // When
            cookies.SetSkipSystemNotificationCookie(adminId, testDate);

            // Then
            A.CallTo(
                () => cookies.Append(
                    SystemNotificationCookieHelper.CookieName,
                    "42",
                    A<CookieOptions>.That.Matches(co => co.Expires == expectedExpiry)
                )
            ).MustHaveHappened();
        }

        [Test]
        public void HasSkippedNotificationsCookie_is_true_when_cookie_exists_with_matching_ID()
        {
            // Given
            const int adminId = 42;
            var cookies = ControllerContextHelper.SetUpFakeRequestCookieCollection(
                SystemNotificationCookieHelper.CookieName,
                "42"
            );

            // When
            var result = cookies.HasSkippedNotificationsCookie(adminId);

            // Then
            Assert.IsTrue(result);
        }

        [Test]
        public void HasSkippedNotificationsCookie_is_false_when_cookie_exists_with_non_matching_ID()
        {
            // Given
            const int adminId = 42;
            var cookies = ControllerContextHelper.SetUpFakeRequestCookieCollection(
                SystemNotificationCookieHelper.CookieName,
                "1"
            );

            // When
            var result = cookies.HasSkippedNotificationsCookie(adminId);

            // Then
            Assert.IsFalse(result);
        }

        [Test]
        public void HasSkippedNotificationsCookie_is_false_when_no_cookie_exists()
        {
            // Given
            const int adminId = 42;
            var cookies = A.Fake<IRequestCookieCollection>();

            // When
            var result = cookies.HasSkippedNotificationsCookie(adminId);

            // Then
            Assert.IsFalse(result);
        }
    }
}
