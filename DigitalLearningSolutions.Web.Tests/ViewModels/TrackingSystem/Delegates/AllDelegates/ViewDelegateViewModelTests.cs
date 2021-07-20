namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using FluentAssertions;
    using NUnit.Framework;

    public class ViewDelegateViewModelTests
    {
        private readonly List<CustomFieldViewModel> customFields = new List<CustomFieldViewModel>();

        private readonly List<DelegateCourseInfoViewModel>
            delegateCourseInfos = new List<DelegateCourseInfoViewModel>();

        [Test]
        public void ViewDelegateViewModel_sets_active_tag_name_correctly()
        {
            // Given
            var activeUser = new DelegateUserCard { Active = true };
            var inactiveUser = new DelegateUserCard { Active = false };

            // When
            var activeModel = new ViewDelegateViewModel(
                new DelegateInfoViewModel(activeUser, customFields),
                delegateCourseInfos
            );
            var inactiveModel = new ViewDelegateViewModel(
                new DelegateInfoViewModel(inactiveUser, customFields),
                delegateCourseInfos
            );

            // Then
            activeModel.ActiveTagName.Should().Be("Active");
            inactiveModel.ActiveTagName.Should().Be("Inactive");
        }

        [Test]
        public void ViewDelegateViewModel_sets_password_tag_name_correctly()
        {
            // Given
            var pwSetUser = new DelegateUserCard { Password = "pw" };
            var pwNotSetUser = new DelegateUserCard { Password = null };

            // When
            var pwSetModel = new ViewDelegateViewModel(
                new DelegateInfoViewModel(pwSetUser, customFields),
                delegateCourseInfos
            );
            var pwNotSetModel = new ViewDelegateViewModel(
                new DelegateInfoViewModel(pwNotSetUser, customFields),
                delegateCourseInfos
            );

            // Then
            pwSetModel.PasswordTagName.Should().Be("Password set");
            pwNotSetModel.PasswordTagName.Should().Be("Password not set");
        }

        [Test]
        public void ViewDelegateViewModel_sets_regstatus_tag_name_correctly()
        {
            // Given
            var selfRegUser = new DelegateUserCard { SelfReg = true, ExternalReg = false };
            var selfRegExternalUser = new DelegateUserCard { SelfReg = true, ExternalReg = true };
            var centreRegUser = new DelegateUserCard { SelfReg = false };

            // When
            var selfRegModel = new ViewDelegateViewModel(
                new DelegateInfoViewModel(selfRegUser, customFields),
                delegateCourseInfos
            );
            var selfRegExternalModel = new ViewDelegateViewModel(
                new DelegateInfoViewModel(selfRegExternalUser, customFields),
                delegateCourseInfos
            );
            var centreRegModel =
                new ViewDelegateViewModel(new DelegateInfoViewModel(centreRegUser, customFields), delegateCourseInfos);

            // Then
            selfRegModel.RegStatusTagName.Should().Be("Self registered");
            selfRegExternalModel.RegStatusTagName.Should().Be("Self registered (External)");
            centreRegModel.RegStatusTagName.Should().Be("Registered by centre");
        }
    }
}
