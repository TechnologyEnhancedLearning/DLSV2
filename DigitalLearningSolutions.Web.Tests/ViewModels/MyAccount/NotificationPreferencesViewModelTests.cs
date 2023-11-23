using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Tests.ViewModels.MyAccount
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using FluentAssertions;
    using NUnit.Framework;

    class NotificationPreferencesViewModelTests
    {
        [Test]
        public void When_only_admin_notifications_are_present_they_are_marked_to_not_show_as_expandable()
        {
            // given
            var adminNotifications = new List<NotificationPreference> { new NotificationPreference() };
            var delegateNotifications = new List<NotificationPreference>();

            // when
            var returnedModel = new NotificationPreferencesViewModel(adminNotifications, delegateNotifications, null);

            // then
            returnedModel.AdminNotifications.ShowAsExpandable.Should().BeFalse();
        }

        [Test]
        public void When_only_delegate_notifications_are_present_they_are_marked_to_not_show_as_expandable()
        {
            // given
            var adminNotifications = new List<NotificationPreference>();
            var delegateNotifications = new List<NotificationPreference> { new NotificationPreference() };

            // when
            var returnedModel = new NotificationPreferencesViewModel(adminNotifications, delegateNotifications, null);

            // then
            returnedModel.DelegateNotifications.ShowAsExpandable.Should().BeFalse();
        }

        [Test]
        public void When_both_notification_types_are_present_they_are_marked_to_show_as_expandable()
        {
            // given
            var adminNotifications = new List<NotificationPreference> { new NotificationPreference() };
            var delegateNotifications = new List<NotificationPreference> { new NotificationPreference() };

            // when
            var returnedModel = new NotificationPreferencesViewModel(adminNotifications, delegateNotifications, null);

            // then
            returnedModel.AdminNotifications.ShowAsExpandable.Should().BeTrue();
            returnedModel.DelegateNotifications.ShowAsExpandable.Should().BeTrue();
        }
    }
}
