namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Web.Helpers;
    using FakeItEasy;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class FilterCookieHelperTests
    {
        private HttpResponse httpResponse = null!;
        private const string CookieName = "Filter";

        [SetUp]
        public void Setup()
        {
            httpResponse = A.Fake<HttpResponse>();
        }

        [Test]
        public void UpdateFilterCookie_sets_cookie_to_string_value_when_not_null()
        {
            // Given
            const string cookieValue = "ActiveStatus|Active|false";

            // When
            httpResponse.UpdateFilterCookie(CookieName, cookieValue);

            // Then
            A.CallTo(
                    () => httpResponse.Cookies.Append(
                        CookieName,
                        cookieValue,
                        A<CookieOptions>._
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void UpdateFilterCookie_sets_cookie_to_empty_cookie_string_when_null()
        {
            // When
            httpResponse.UpdateFilterCookie(CookieName, null);

            // Then
            A.CallTo(
                    () => httpResponse.Cookies.Append(
                        CookieName,
                        FilteringHelper.EmptyFiltersCookieValue,
                        A<CookieOptions>._
                    )
                )
                .MustHaveHappened();
        }
    }
}
