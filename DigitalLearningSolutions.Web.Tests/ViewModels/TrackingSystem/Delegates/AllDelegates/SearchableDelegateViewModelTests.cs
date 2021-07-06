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
        [Test]
        public void SearchableDelegateViewModel_sets_active_tag_name_correctly()
        {
            var activeModel = new SearchableDelegateViewModel(
                new DelegateUserCard { Active = true },
                new List<CustomFieldViewModel>()
            );
            var inactiveModel = new SearchableDelegateViewModel(
                new DelegateUserCard { Active = false },
                new List<CustomFieldViewModel>()
            );

            activeModel.ActiveTagName.Should().Be("Active");
            inactiveModel.ActiveTagName.Should().Be("Inactive");
        }

        [Test]
        public void SearchableDelegateViewModel_sets_password_tag_name_correctly()
        {
            var pwSetModel = new SearchableDelegateViewModel(
                new DelegateUserCard { Password = "pw" },
                new List<CustomFieldViewModel>()
            );
            var pwNotSetModel = new SearchableDelegateViewModel(
                new DelegateUserCard { Password = null },
                new List<CustomFieldViewModel>()
            );

            pwSetModel.PasswordTagName.Should().Be("Password set");
            pwNotSetModel.PasswordTagName.Should().Be("Password not set");
        }

        [Test]
        public void SearchableDelegateViewModel_sets_regstatus_tag_name_correctly()
        {
            var selfRegModel = new SearchableDelegateViewModel(
                new DelegateUserCard { SelfReg = true, ExternalReg = false },
                new List<CustomFieldViewModel>()
            );
            var selfRegExternalModel = new SearchableDelegateViewModel(
                new DelegateUserCard { SelfReg = true, ExternalReg = true },
                new List<CustomFieldViewModel>()
            );
            var centreRegModel = new SearchableDelegateViewModel(
                new DelegateUserCard { SelfReg = false },
                new List<CustomFieldViewModel>()
            );

            selfRegModel.RegStatusTagName.Should().Be("Self registered");
            selfRegExternalModel.RegStatusTagName.Should().Be("Self registered (External)");
            centreRegModel.RegStatusTagName.Should().Be("Registered by centre");
        }

        [Test]
        public void SearchableDelegateViewModel_sets_reg_date_string_correctly()
        {
            var model = new SearchableDelegateViewModel(
                new DelegateUserCard { DateRegistered = new DateTime(2021, 05, 13) },
                new List<CustomFieldViewModel>()
            );

            model.RegistrationDate.Should().Be("13/05/2021");
        }
    }
}
