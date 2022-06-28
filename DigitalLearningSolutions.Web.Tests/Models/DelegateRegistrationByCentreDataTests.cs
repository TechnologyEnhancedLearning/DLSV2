namespace DigitalLearningSolutions.Web.Tests.Models
{
    using System;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre;
    using FluentAssertions;
    using NUnit.Framework;

    internal class DelegateRegistrationByCentreDataTests
    {
        private const string FirstName = "Test";
        private const string LastName = "User";
        private const string Email = "test@email.com";
        private const int CentreId = 5;

        [Test]
        public void SetPersonalInformation_sets_data_correctly()
        {
            // Given
            var model = new PersonalInformationViewModel
            {
                FirstName = FirstName,
                LastName = LastName,
                Centre = CentreId,
                PrimaryEmail = Email,
            };
            var data = new DelegateRegistrationByCentreData();

            // When
            data.SetPersonalInformation(model);

            // Then
            data.FirstName.Should().Be(FirstName);
            data.LastName.Should().Be(LastName);
            data.PrimaryEmail.Should().Be(Email);
            data.Centre.Should().Be(CentreId);
        }

        [Test]
        public void SetWelcomeEmail_with_ShouldSendEmail_false_sets_data_correctly()
        {
            // Given
            var model = new WelcomeEmailViewModel { ShouldSendEmail = false, Day = 7, Month = 7, Year = 2200 };
            var data = new DelegateRegistrationByCentreData();

            // When
            data.SetWelcomeEmail(model);

            // Then
            data.ShouldSendEmail.Should().BeFalse();
            data.WelcomeEmailDate.Should().BeNull();
        }

        [Test]
        public void SetWelcomeEmail_with_ShouldSendEmail_true_sets_data_correctly()
        {
            // Given
            var date = new DateTime(2200, 7, 7);
            var model = new WelcomeEmailViewModel
                { ShouldSendEmail = true, Day = date.Day, Month = date.Month, Year = date.Year };
            var data = new DelegateRegistrationByCentreData();

            // When
            data.SetWelcomeEmail(model);

            // Then
            data.ShouldSendEmail.Should().BeTrue();
            data.WelcomeEmailDate.Should().Be(date);
            data.IsPasswordSet.Should().BeFalse();
            data.PasswordHash.Should().BeNull();
        }
    }
}
