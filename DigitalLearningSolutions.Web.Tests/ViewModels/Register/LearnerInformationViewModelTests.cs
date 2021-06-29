namespace DigitalLearningSolutions.Web.Tests.ViewModels.Register
{
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FluentAssertions;
    using NUnit.Framework;

    public class LearnerInformationViewModelTests
    {
        private const string FirstName = "Test";
        private const string LastName = "User";
        private const string Email = "test@email.com";
        private const int CentreId = 5;
        private const int JobGroupId = 10;
        private const string PasswordHash = "password hash";
        private const string Answer1 = "a1";
        private const string Answer2 = "a2";
        private const string Answer3 = "a3";
        private const bool IsCentreSpecificRegistration = true;

        [Test]
        public void LearnerInformation_constructor_using_data_populates_viewmodel_correctly()
        {
            // Given
            var data = SampleRegistrationData();

            // When
            var result = new LearnerInformationViewModel(data);

            // Then
            result.JobGroup.Should().Be(JobGroupId);
        }

        [Test]
        public void LearnerInformation_constructor_using_delegate_data_populates_viewmodel_correctly()
        {
            // Given
            var data = SampleDelegateRegistrationData();

            // When
            var result = new LearnerInformationViewModel(data);

            // Then
            result.JobGroup.Should().Be(JobGroupId);
            result.Answer1.Should().Be(Answer1);
            result.Answer2.Should().Be(Answer2);
            result.Answer3.Should().Be(Answer3);
        }

        private static RegistrationData SampleRegistrationData()
        {
            return new RegistrationData
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Centre = CentreId,
                JobGroup = JobGroupId,
                PasswordHash = PasswordHash
            };
        }

        private static DelegateRegistrationData SampleDelegateRegistrationData()
        {
            return new DelegateRegistrationData
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Centre = CentreId,
                JobGroup = JobGroupId,
                PasswordHash = PasswordHash,
                Answer1 = Answer1,
                Answer2 = Answer2,
                Answer3 = Answer3,
                IsCentreSpecificRegistration = IsCentreSpecificRegistration
            };
        }
    }
}
