namespace DigitalLearningSolutions.Web.Tests.Controllers.MyAccount
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class NotificationPreferencesControllerTests
    {
        private NotificationPreferencesController controller = null!;
        private INotificationPreferencesService notificationPreferencesService = null!;

        [SetUp]
        public void SetUp()
        {
            notificationPreferencesService = A.Fake<INotificationPreferencesService>();

            controller = new NotificationPreferencesController(notificationPreferencesService).WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void SaveNotificationPreferences_redirects_to_expected_page_on_success()
        {
            // Given
            A.CallTo(
                () => notificationPreferencesService.SetNotificationPreferencesForUser(
                    UserType.AdminUser,
                    A<int?>._,
                    A<IEnumerable<int>>._
                )
            ).DoesNothing();
            var parameterName = typeof(MyAccountController).GetMethod("Index")?.GetParameters()
                .SingleOrDefault(p => p.ParameterType == typeof(DlsSubApplication))?.Name;

            // When
            var result = controller.SaveNotificationPreferences(
                UserType.AdminUser,
                new List<int>(),
                DlsSubApplication.Default
            );

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index").WithRouteValue(
                parameterName,
                DlsSubApplication.Default.UrlSegment
            );
        }
    }
}
