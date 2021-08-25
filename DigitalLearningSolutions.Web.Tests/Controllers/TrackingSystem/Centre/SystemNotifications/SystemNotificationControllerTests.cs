namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre.SystemNotifications
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class SystemNotificationControllerTests
    {
        private readonly IClockService clockService = A.Fake<IClockService>();
        private readonly HttpRequest httpRequest = A.Fake<HttpRequest>();
        private readonly HttpResponse httpResponse = A.Fake<HttpResponse>();

        private readonly ISystemNotificationsDataService systemNotificationsDataService =
            A.Fake<ISystemNotificationsDataService>();

        private SystemNotificationsController controller = null!;

        [SetUp]
        public void Setup()
        {
            controller =
                new SystemNotificationsController(systemNotificationsDataService, clockService)
                    .WithMockHttpContextWithCookie(httpRequest, "testCookie", "testValue", httpResponse)
                    .WithMockUser(true)
                    .WithMockServices();
        }

        [Test]
        public void SkipNotifications_sets_cookie_and_redirects_to_dashboard()
        {
            // Given
            var testDate = new DateTime(2021, 8, 23);
            A.CallTo(() => clockService.UtcNow).Returns(testDate);
            var expectedExpiry = testDate.AddHours(24);

            // When
            var result = controller.SkipNotifications();

            // Then
            using (new AssertionScope())
            {
                result.Should().BeRedirectToActionResult().WithControllerName("Dashboard").WithActionName("Index");
                A.CallTo(
                    () => httpResponse.Cookies.Append(
                        SystemNotificationCookieHelper.CookieName,
                        "7",
                        A<CookieOptions>.That.Matches(co => co.Expires == expectedExpiry)
                    )
                ).MustHaveHappened();
            }
        }
    }
}
