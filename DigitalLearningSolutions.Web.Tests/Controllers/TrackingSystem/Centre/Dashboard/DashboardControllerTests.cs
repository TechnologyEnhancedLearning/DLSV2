namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre.Dashboard
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Dashboard;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class DashboardControllerTests
    {
        private readonly ICentresDataService centresDataService = A.Fake<ICentresDataService>();
        private readonly ICentresService centresService = A.Fake<ICentresService>();
        private readonly ICourseDataService courseDataService = A.Fake<ICourseDataService>();
        private readonly HttpRequest httpRequest = A.Fake<HttpRequest>();
        private readonly HttpResponse httpResponse = A.Fake<HttpResponse>();

        private readonly ISystemNotificationsDataService systemNotificationsDataService =
            A.Fake<ISystemNotificationsDataService>();

        private readonly ISupportTicketDataService ticketDataService = A.Fake<ISupportTicketDataService>();
        private readonly IUserDataService userDataService = A.Fake<IUserDataService>();
        private DashboardController dashboardController = null!;

        [SetUp]
        public void Setup()
        {
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(UserTestHelper.GetDefaultAdminUser());
            A.CallTo(() => centresDataService.GetCentreDetailsById(A<int>._))
                .Returns(CentreTestHelper.GetDefaultCentre());
            A.CallTo(() => userDataService.GetNumberOfApprovedDelegatesAtCentre(A<int>._)).Returns(1);
            A.CallTo(() => courseDataService.GetNumberOfActiveCoursesAtCentreFilteredByCategory(A<int>._, A<int>._))
                .Returns(1);
            A.CallTo(() => userDataService.GetNumberOfActiveAdminsAtCentre(A<int>._)).Returns(1);
            A.CallTo(() => ticketDataService.GetNumberOfUnarchivedTicketsForCentreId(A<int>._)).Returns(1);
            A.CallTo(() => centresService.GetCentreRankForCentre(A<int>._)).Returns(1);

            dashboardController = new DashboardController(
                    userDataService,
                    centresDataService,
                    courseDataService,
                    ticketDataService,
                    centresService,
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
            result.Should().BeRedirectToActionResult().WithControllerName("SystemNotifications")
                .WithActionName("Index");
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

            // When
            var result = dashboardController.Index();

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void Index_goes_to_Index_page_when_no_unacknowledged_notifications_exist()
        {
            // Given
            A.CallTo(() => systemNotificationsDataService.GetUnacknowledgedSystemNotifications(A<int>._))
                .Returns(new List<SystemNotification>());

            // When
            var result = dashboardController.Index();

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }
    }
}
