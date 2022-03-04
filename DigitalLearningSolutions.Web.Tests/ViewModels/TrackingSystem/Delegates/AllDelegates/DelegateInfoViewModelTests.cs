namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;
    using FluentAssertions;
    using NUnit.Framework;

    public class DelegateInfoViewModelTests
    {
        [Test]
        public void DelegateInfoViewModel_sets_reg_date_string_correctly()
        {
            // Given
            var date = new DateTime(2021, 05, 13);
            var user = new DelegateUserCard { DateRegistered = date };

            // When
            var model = new DelegateInfoViewModel(user, new List<DelegateRegistrationPrompt>());

            // Then
            model.RegistrationDate.Should().Be("13/05/2021");
        }
    }
}
