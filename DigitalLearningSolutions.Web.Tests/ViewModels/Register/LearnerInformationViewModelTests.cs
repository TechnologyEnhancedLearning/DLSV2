namespace DigitalLearningSolutions.Web.Tests.ViewModels.Register
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FluentAssertions;
    using NUnit.Framework;

    public class LearnerInformationViewModelTests
    {
        [Test]
        public void LearnerInformation_constructor_using_data_populates_viewmodel_correctly()
        {
            // Given
            var data = RegistrationDataHelper.SampleRegistrationData();

            // When
            var result = new LearnerInformationViewModel(data, false);

            // Then
            result.JobGroup.Should().Be(data.JobGroup);
        }

        [Test]
        public void LearnerInformation_constructor_using_delegate_data_populates_viewmodel_correctly()
        {
            // Given
            var data = RegistrationDataHelper.SampleDelegateRegistrationData();

            // When
            var result = new LearnerInformationViewModel(data, false);

            // Then
            result.JobGroup.Should().Be(data.JobGroup);
            result.Answer1.Should().Be(data.Answer1);
            result.Answer2.Should().Be(data.Answer2);
            result.Answer3.Should().Be(data.Answer3);
        }
    }
}
