namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre
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
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private ISystemNotificationsDataService systemNotificationsDataService = null!;
        private SystemNotificationsController controller = null!;

        [SetUp]
        public void Setup()
        {
            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            systemNotificationsDataService = A.Fake<ISystemNotificationsDataService>();
            controller =
                new SystemNotificationsController(systemNotificationsDataService, clockService)
                    .WithMockHttpContext(httpRequest, response: httpResponse)
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

        [Test]
        public void SkipNotifications_does_not_acknowledge_notifications()
        {
            // Given
            var testDate = new DateTime(2021, 8, 23);
            A.CallTo(() => clockService.UtcNow).Returns(testDate);

            // When
            controller.SkipNotifications();

            // Then
            A.CallTo(() => systemNotificationsDataService.AcknowledgeNotification(A<int>._, A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Post_acknowledges_notification_and_redirects()
        {
            // When
            var result = controller.AcknowledgeNotification(1, 1);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => systemNotificationsDataService.AcknowledgeNotification(1, 7)).MustHaveHappened();
                result.Should().BeRedirectToActionResult().WithControllerName("SystemNotifications").WithActionName("Index");
            }
        }

        [Test]
        public void Post_deletes_cookie_if_one_exists_for_user()
        {
            // Given
            A.CallTo(() => httpRequest.Cookies).Returns(
                ControllerContextHelper.SetUpFakeRequestCookieCollection(SystemNotificationCookieHelper.CookieName, "7")
            );

            // When
            controller.AcknowledgeNotification(1, 1);

            // Then
            A.CallTo(() => httpResponse.Cookies.Delete(SystemNotificationCookieHelper.CookieName)).MustHaveHappened();
        }

        [Test]
        public void Post_does_not_delete_cookie_if_one_exists_for_someone_other_than_current_user()
        {
            // Given
            A.CallTo(() => httpRequest.Cookies).Returns(
                ControllerContextHelper.SetUpFakeRequestCookieCollection(SystemNotificationCookieHelper.CookieName, "8")
            );

            // When
            controller.AcknowledgeNotification(1, 1);

            // Then
            A.CallTo(() => httpResponse.Cookies.Delete(SystemNotificationCookieHelper.CookieName)).MustNotHaveHappened();
        }

        [Test]
        public void Post_does_not_delete_cookie_if_one_does_not_exist()
        {
            // Given
            A.CallTo(() => httpRequest.Cookies).Returns(
                A.Fake<IRequestCookieCollection>()
            );

            // When
            controller.AcknowledgeNotification(1, 1);

            // Then
            A.CallTo(() => httpResponse.Cookies.Delete(SystemNotificationCookieHelper.CookieName)).MustNotHaveHappened();
        }
    }
}
