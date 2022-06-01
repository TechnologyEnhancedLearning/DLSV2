namespace DigitalLearningSolutions.Web.Tests.ViewModels.Register
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FluentAssertions;
    using NUnit.Framework;

    public class PersonalInformationViewModelTests
    {
        [Test]
        public void PersonalInformation_constructor_using_data_populates_viewmodel_correctly()
        {
            // Given
            var data = RegistrationDataHelper.SampleRegistrationData();

            // When
            var result = new PersonalInformationViewModel(data);

            // Then
            result.FirstName.Should().Be(data.FirstName);
            result.LastName.Should().Be(data.LastName);
            result.PrimaryEmail.Should().Be(data.PrimaryEmail);
            result.Centre.Should().Be(data.Centre);
        }

        [Test]
        public void PersonalInformation_constructor_using_delegate_data_populates_viewmodel_correctly()
        {
            // Given
            var data = RegistrationDataHelper.SampleDelegateRegistrationData();

            // When
            var result = new PersonalInformationViewModel(data);

            // Then
            result.FirstName.Should().Be(data.FirstName);
            result.LastName.Should().Be(data.LastName);
            result.PrimaryEmail.Should().Be(data.PrimaryEmail);
            result.Centre.Should().Be(data.Centre);
            result.IsCentreSpecificRegistration.Should().Be(data.IsCentreSpecificRegistration);
        }
    }
}
