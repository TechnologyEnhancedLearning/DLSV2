namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class NotificationPreferencesDataServiceTests
    {
        private NotificationPreferencesDataService service;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            service = new NotificationPreferencesDataService(connection);
        }

        [Test]
        public void Get_notification_preferences_for_admin_should_return_correct_list_of_notifications()
        {
            // when
            var result = service.GetNotificationPreferencesForAdmin(10).ToList();

            // then
            result.Count().Should().Be(7);

            var first = result.First();

            first.NotificationId.Should().Be(1);
            first.NotificationName.Should().Be("System notification added");
            first.Description.Should().Be(@"<p>Triggered when the central ISG team add a notification to the system. These are used to issue important system information such as:</p>
<ul>
<li>changes to the system</li>
<li>notification of downtime / unavailability of services</li>
<li>notification of known issues</li>
</ul>
<p>These notifications will also appear on your in-tray when you log in to the system until you acknowledge them.</p>");
            first.Accepted.Should().BeTrue();

            var fourth = result[3];

            fourth.NotificationId.Should().Be(4);
            fourth.NotificationName.Should().Be("Delegate registration requires approval");
            fourth.Description.Should().Be(@"<p>Triggered when a delegate registers for your centre from outside of your network and requires approval.
</p>");
            fourth.Accepted.Should().BeFalse();
        }

        [Test]
        public void Get_notification_preferences_for_admin_should_return_empty_list_for_null_id()
        {
            // when
            var result = service.GetNotificationPreferencesForAdmin(null);

            // then
            result.Should().BeEmpty();
        }

        [Test]
        public void Get_notification_preferences_for_delegate_should_return_correct_list_of_notifications()
        {
            // when
            var result = service.GetNotificationPreferencesForDelegate(7).ToList();

            // then
            result.Count().Should().Be(7);

            var first = result.First();

            first.NotificationId.Should().Be(9);
            first.NotificationName.Should().Be("Course completion reminder");
            first.Description.Should().Be(@"<p>Triggered when the completion due date for a course is approaching and once it has passed.
</p>");
            first.Accepted.Should().BeFalse();

            var seventh = result[6];

            seventh.NotificationId.Should().Be(17);
            seventh.NotificationName.Should().Be("Supervisor completed verification");
            seventh.Description.Should().Be(@"<p>Triggered when your supervisor completes verification of your progress against an activity.</p>");
            seventh.Accepted.Should().BeTrue();
        }

        [Test]
        public void Get_notification_preferences_for_delegate_should_return_empty_list_for_null_id()
        {
            // when
            var result = service.GetNotificationPreferencesForDelegate(null);

            // then
            result.Should().BeEmpty();
        }
    }
}
