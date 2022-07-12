namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre.Dashboard
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Dashboard;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class DashboardControllerTests
    {
        private readonly HttpRequest httpRequest = A.Fake<HttpRequest>();
        private readonly HttpResponse httpResponse = A.Fake<HttpResponse>();

        private DashboardController dashboardController = null!;
        private IDashboardInformationService dashboardInformationService = null!;
        private ISystemNotificationsDataService systemNotificationsDataService = null!;

        [SetUp]
        public void Setup()
        {
            dashboardInformationService = A.Fake<IDashboardInformationService>();
            systemNotificationsDataService = A.Fake<ISystemNotificationsDataService>();
            dashboardController = new DashboardController(
                    dashboardInformationService,
                    systemNotificationsDataService
                ).WithMockHttpContext(httpRequest, response: httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void Index_redirects_to_Notifications_page_when_unacknowledged_notifications_have_not_been_skipped()
        {
            // Given
            A.CallTo(() => systemNotificationsDataService.GetUnacknowledgedSystemNotifications(A<int>._))
                .Returns(new List<SystemNotification> { SystemNotificationTestHelper.GetDefaultSystemNotification() });

            // When
            var result = dashboardController.Index();

            // Then
            using (new AssertionScope())
            {
                result.Should().BeRedirectToActionResult().WithControllerName("SystemNotifications")
                    .WithActionName("Index");
                A.CallTo(() => dashboardInformationService.GetDashboardInformationForCentre(A<int>._, A<int>._))
                    .MustNotHaveHappened();
            }
        }

        [Test]
        public void Index_goes_to_Index_page_when_unacknowledged_notifications_have_been_skipped()
        {
            // Given
            A.CallTo(() => systemNotificationsDataService.GetUnacknowledgedSystemNotifications(A<int>._))
                .Returns(new List<SystemNotification> { SystemNotificationTestHelper.GetDefaultSystemNotification() });
            A.CallTo(() => httpRequest.Cookies).Returns(
                ControllerContextHelper.SetUpFakeRequestCookieCollection(SystemNotificationCookieHelper.CookieName, "7")
            );
            A.CallTo(() => dashboardInformationService.GetDashboardInformationForCentre(A<int>._, A<int>._)).Returns(
                new CentreDashboardInformation(
                    CentreTestHelper.GetDefaultCentre(),
                    UserTestHelper.GetDefaultAdminUser(),
                    1,
                    1,
                    1,
                    1,
                    1
                )
            );

            // When
            var result = dashboardController.Index();

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void Index_returns_not_found_when_dashboard_information_is_null()
        {
            // Given
            A.CallTo(() => systemNotificationsDataService.GetUnacknowledgedSystemNotifications(A<int>._))
                .Returns(new List<SystemNotification>());
            A.CallTo(() => dashboardInformationService.GetDashboardInformationForCentre(A<int>._, A<int>._)).Returns(
                null
            );

            // When
            var result = dashboardController.Index();

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_goes_to_Index_page_when_no_unacknowledged_notifications_exist()
        {
            // Given
            A.CallTo(() => systemNotificationsDataService.GetUnacknowledgedSystemNotifications(A<int>._))
                .Returns(new List<SystemNotification>());
            A.CallTo(() => dashboardInformationService.GetDashboardInformationForCentre(A<int>._, A<int>._)).Returns(
                new CentreDashboardInformation(
                    CentreTestHelper.GetDefaultCentre(),
                    UserTestHelper.GetDefaultAdminUser(),
                    1,
                    1,
                    1,
                    1,
                    1
                )
            );

            // When
            var result = dashboardController.Index();

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }
    }
}
