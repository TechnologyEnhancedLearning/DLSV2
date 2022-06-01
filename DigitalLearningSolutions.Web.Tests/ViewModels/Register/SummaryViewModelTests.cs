namespace DigitalLearningSolutions.Web.Tests.ViewModels.Register
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FluentAssertions;
    using NUnit.Framework;

    public class SummaryViewModelTests
    {
        [Test]
        public void Summary_constructor_using_data_populates_viewmodel_correctly()
        {
            // Given
            var data = RegistrationDataHelper.SampleRegistrationData();

            // When
            var result = new SummaryViewModel(data);

            // Then
            result.FirstName.Should().Be(data.FirstName);
            result.LastName.Should().Be(data.LastName);
            result.PrimaryEmail.Should().Be(data.PrimaryEmail);
        }

        [Test]
        public void Summary_constructor_using_delegate_data_populates_viewmodel_correctly()
        {
            // Given
            var data = RegistrationDataHelper.SampleDelegateRegistrationData();

            // When
            var result = new SummaryViewModel(data);

            // Then
            result.FirstName.Should().Be(data.FirstName);
            result.LastName.Should().Be(data.LastName);
            result.PrimaryEmail.Should().Be(data.PrimaryEmail);
        }
    }
}
