namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
{
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
            var activeModel = new SearchableDelegateViewModel(new DelegateInfoViewModel(activeUser, customFields));
            var inactiveModel = new SearchableDelegateViewModel(new DelegateInfoViewModel(inactiveUser, customFields));

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
            var pwSetModel = new SearchableDelegateViewModel(new DelegateInfoViewModel(pwSetUser, customFields));
            var pwNotSetModel = new SearchableDelegateViewModel(new DelegateInfoViewModel(pwNotSetUser, customFields));

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
            var selfRegModel = new SearchableDelegateViewModel(new DelegateInfoViewModel(selfRegUser, customFields));
            var selfRegExternalModel =
                new SearchableDelegateViewModel(new DelegateInfoViewModel(selfRegExternalUser, customFields));
            var centreRegModel =
                new SearchableDelegateViewModel(new DelegateInfoViewModel(centreRegUser, customFields));

            // Then
            selfRegModel.RegStatusTagName.Should().Be("Self registered");
            selfRegExternalModel.RegStatusTagName.Should().Be("Self registered (External)");
            centreRegModel.RegStatusTagName.Should().Be("Registered by centre");
        }
    }
}
