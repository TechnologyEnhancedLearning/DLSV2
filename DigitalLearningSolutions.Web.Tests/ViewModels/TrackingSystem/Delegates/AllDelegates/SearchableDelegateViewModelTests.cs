namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using FluentAssertions;
    using NUnit.Framework;

    public class SearchableDelegateViewModelTests
    {
        private readonly List<CustomFieldViewModel> customFields = new List<CustomFieldViewModel>();

        [Test]
        public void SearchableDelegateViewModel_sets_active_tag_name_correctly()
        {
            // Given
            var activeUser = new DelegateUserCard { Active = true };
            var inactiveUser = new DelegateUserCard { Active = false };

            // When
            var activeModel = new SearchableDelegateViewModel(activeUser, customFields);
            var inactiveModel = new SearchableDelegateViewModel(inactiveUser, customFields);

            // Then
            activeModel.ActiveTagName.Should().Be("Active");
            inactiveModel.ActiveTagName.Should().Be("Inactive");
        }

        [Test]
        public void SearchableDelegateViewModel_sets_password_tag_name_correctly()
        {
            // Given
            var pwSetUser = new DelegateUserCard { Password = "pw" };
            var pwNotSetUser = new DelegateUserCard { Password = null };

            // When
            var pwSetModel = new SearchableDelegateViewModel(pwSetUser, customFields);
            var pwNotSetModel = new SearchableDelegateViewModel(pwNotSetUser, customFields);

            // Then
            pwSetModel.PasswordTagName.Should().Be("Password set");
            pwNotSetModel.PasswordTagName.Should().Be("Password not set");
        }

        [Test]
        public void SearchableDelegateViewModel_sets_regstatus_tag_name_correctly()
        {
            // Given
            var selfRegUser = new DelegateUserCard { SelfReg = true, ExternalReg = false };
            var selfRegExternalUser = new DelegateUserCard { SelfReg = true, ExternalReg = true };
            var centreRegUser = new DelegateUserCard { SelfReg = false };

            // When
            var selfRegModel = new SearchableDelegateViewModel(selfRegUser, customFields);
            var selfRegExternalModel = new SearchableDelegateViewModel(selfRegExternalUser, customFields);
            var centreRegModel = new SearchableDelegateViewModel(centreRegUser, customFields);

            // Then
            selfRegModel.RegStatusTagName.Should().Be("Self registered");
            selfRegExternalModel.RegStatusTagName.Should().Be("Self registered (External)");
            centreRegModel.RegStatusTagName.Should().Be("Registered by centre");
        }

        [Test]
        public void SearchableDelegateViewModel_sets_reg_date_string_correctly()
        {
            // Given
            var date = new DateTime(2021, 05, 13);
            var user = new DelegateUserCard { DateRegistered = date };

            // When
            var model = new SearchableDelegateViewModel(user, customFields);

            // Then
            model.RegistrationDate.Should().Be("13/05/2021");
        }
    }
}
