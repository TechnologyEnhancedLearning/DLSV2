﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;    
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class SystemNotificationControllerTests
    {
        private readonly IClockUtility clockUtility = A.Fake<IClockUtility>();
        private SystemNotificationsController controller = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private ISystemNotificationsService systemNotificationsService = null!;

        [SetUp]
        public void Setup()
        {
            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            systemNotificationsService = A.Fake<ISystemNotificationsService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
            controller =
                new SystemNotificationsController(
                        systemNotificationsService,
                        clockUtility,
                        searchSortFilterPaginateService
                    )
                    .WithMockHttpContext(httpRequest, response: httpResponse)
                    .WithMockUser(true)
                    .WithMockServices()
                    .WithMockTempData();
        }

        [Test]
        public void Index_sets_cookie_when_unacknowledged_notifications_exist()
        {
            // Given
            var testDate = new DateTime(2021, 8, 23);
            A.CallTo(() => clockUtility.UtcNow).Returns(testDate);
            var expectedExpiry = testDate.AddHours(24);
            A.CallTo(() => systemNotificationsService.GetUnacknowledgedSystemNotifications(A<int>._))
                .Returns(new List<SystemNotification> { SystemNotificationTestHelper.GetDefaultSystemNotification() });

            // When
            var result = controller.Index();

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().WithDefaultViewName();
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
        public void Post_acknowledges_notification_and_redirects()
        {
            // When
            var result = controller.AcknowledgeNotification(1, 1);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => systemNotificationsService.AcknowledgeNotification(1, 7)).MustHaveHappened();
                result.Should().BeRedirectToActionResult().WithControllerName("SystemNotifications")
                    .WithActionName("Index");
            }
        }

        [Test]
        public void Index_deletes_cookie_if_one_exists_for_user_and_no_unacknowledged_notifications_exist()
        {
            // Given
            A.CallTo(() => httpRequest.Cookies).Returns(
                ControllerContextHelper.SetUpFakeRequestCookieCollection(SystemNotificationCookieHelper.CookieName, "7")
            );
            A.CallTo(() => systemNotificationsService.GetUnacknowledgedSystemNotifications(A<int>._))
                .Returns(new List<SystemNotification>());

            // When
            controller.Index();

            // Then
            A.CallTo(() => httpResponse.Cookies.Delete(SystemNotificationCookieHelper.CookieName)).MustHaveHappened();
        }

        [Test]
        public void Index_does_not_delete_cookie_if_one_exists_for_someone_other_than_current_user()
        {
            // Given
            A.CallTo(() => httpRequest.Cookies).Returns(
                ControllerContextHelper.SetUpFakeRequestCookieCollection(SystemNotificationCookieHelper.CookieName, "8")
            );
            A.CallTo(() => systemNotificationsService.GetUnacknowledgedSystemNotifications(A<int>._))
                .Returns(new List<SystemNotification>());

            // When
            controller.Index();

            // Then
            A.CallTo(() => httpResponse.Cookies.Delete(SystemNotificationCookieHelper.CookieName))
                .MustNotHaveHappened();
        }

        [Test]
        public void Index_does_not_delete_cookie_if_one_does_not_exist()
        {
            // Given
            A.CallTo(() => httpRequest.Cookies).Returns(
                A.Fake<IRequestCookieCollection>()
            );
            A.CallTo(() => systemNotificationsService.GetUnacknowledgedSystemNotifications(A<int>._))
                .Returns(new List<SystemNotification>());

            // When
            controller.Index();

            // Then
            A.CallTo(() => httpResponse.Cookies.Delete(SystemNotificationCookieHelper.CookieName))
                .MustNotHaveHappened();
        }
    }
}
