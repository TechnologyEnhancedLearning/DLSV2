using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Web.Tests.ViewModels.MyAccount
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using FluentAssertions;
    using NUnit.Framework;

    class NotificationPreferencesViewModelTests
    {
        [Test]
        [TestCase(UserTypes.Admin)]
        [TestCase(UserTypes.Delegate)]
        public void When_only_one_notification_type_is_present_it_is_marked_to_not_show_as_expandable(string userType)
        {
            // given
            var adminNotifications = new List<NotificationPreference>();
            var delegateNotifications = new List<NotificationPreference>();

            if (userType == UserTypes.Admin)
            {
                adminNotifications.Add(new NotificationPreference());
            } else if (userType == UserTypes.Delegate)
            {
                delegateNotifications.Add(new NotificationPreference());
            }

            // when
            var returnedModel = new NotificationPreferencesViewModel(adminNotifications, delegateNotifications);

            // then
            if (userType == UserTypes.Admin)
            {
                returnedModel.AdminNotifications.ShowAsExpandable.Should().BeFalse();
            }
            else if (userType == UserTypes.Delegate)
            {
                returnedModel.DelegateNotifications.ShowAsExpandable.Should().BeFalse();
            }
        }

        [Test]
        public void When_both_notification_types_are_present_they_are_marked_to_show_as_expandable()
        {
            // given
            var adminNotifications = new List<NotificationPreference>{ new NotificationPreference() };
            var delegateNotifications = new List<NotificationPreference>{ new NotificationPreference() };

            // when
            var returnedModel = new NotificationPreferencesViewModel(adminNotifications, delegateNotifications);

            // then
            returnedModel.AdminNotifications.ShowAsExpandable.Should().BeTrue();
            returnedModel.DelegateNotifications.ShowAsExpandable.Should().BeTrue();
        }
    }
}
