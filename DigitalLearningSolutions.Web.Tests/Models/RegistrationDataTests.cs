namespace DigitalLearningSolutions.Web.Tests.Models
{
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FluentAssertions;
    using NUnit.Framework;

    internal class RegistrationDataTests
    {
        private const string FirstName = "Test";
        private const string LastName = "User";
        private const string PrimaryEmail = "test@email.com";
        private const int CentreId = 5;
        private const int JobGroupId = 10;

        [Test]
        public void SetPersonalInformation_sets_data_correctly()
        {
            // Given
            var model = new PersonalInformationViewModel
            {
                FirstName = FirstName,
                LastName = LastName,
                Centre = CentreId,
                PrimaryEmail = PrimaryEmail
            };
            var data = new RegistrationData();

            // When
            data.SetPersonalInformation(model);

            // Then
            data.FirstName.Should().Be(FirstName);
            data.LastName.Should().Be(LastName);
            data.PrimaryEmail.Should().Be(PrimaryEmail);
            data.Centre.Should().Be(CentreId);
        }

        [Test]
        public void SetLearnerInformation_sets_data_correctly()
        {
            // Given
            var model = new LearnerInformationViewModel
            {
                JobGroup = JobGroupId
            };
            var data = new RegistrationData();

            // When
            data.SetLearnerInformation(model);

            // Then
            data.JobGroup.Should().Be(JobGroupId);
        }
    }
}
