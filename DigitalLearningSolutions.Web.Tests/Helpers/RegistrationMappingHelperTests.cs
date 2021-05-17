namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FluentAssertions;
    using NUnit.Framework;

    public class RegistrationMappingHelperTests
    {
        [Test]
        public void MapToDelegateRegistrationModel_returns_correct_DelegateRegistrationModel()
        {
            // Given
            var firstName = "Test";
            var lastName = "User";
            var email = "test@email.com";
            var centreId = 5;
            var jobGroupId = 10;
            var passwordHash = "password hash";
            var registerViewModel = new RegisterViewModel
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };
            var learnerInformationViewModel = new LearnerInformationViewModel
            {
                Centre = centreId,
                JobGroup = jobGroupId
            };
            var data = new DelegateRegistrationData
            {
                RegisterViewModel = registerViewModel,
                LearnerInformationViewModel = learnerInformationViewModel,
                PasswordHash = passwordHash
            };

            // When
            var result = RegistrationMappingHelper.MapToDelegateRegistrationModel(data);

            // Then
            result.FirstName.Should().Be(firstName);
            result.LastName.Should().Be(lastName);
            result.Email.Should().Be(email);
            result.Centre.Should().Be(centreId);
            result.JobGroup.Should().Be(jobGroupId);
            result.PasswordHash.Should().Be(passwordHash);
        }

        [Test]
        public void MapToSummary_returns_correct_SummaryViewModel()
        {
            // Given
            var firstName = "Test";
            var lastName = "User";
            var email = "test@email.com";
            var centreId = 5;
            var centreName = "A centre";
            var jobGroupId = 10;
            var jobGroupName = "A job group";
            var passwordHash = "password hash";
            var registerViewModel = new RegisterViewModel
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };
            var learnerInformationViewModel = new LearnerInformationViewModel
            {
                Centre = centreId,
                JobGroup = jobGroupId
            };
            var data = new DelegateRegistrationData
            {
                RegisterViewModel = registerViewModel,
                LearnerInformationViewModel = learnerInformationViewModel,
                PasswordHash = passwordHash
            };

            // When
            var result = RegistrationMappingHelper.MapToSummary(data, centreName, jobGroupName);

            // Then
            result.FirstName.Should().Be(firstName);
            result.LastName.Should().Be(lastName);
            result.Email.Should().Be(email);
            result.Centre.Should().Be(centreName);
            result.JobGroup.Should().Be(jobGroupName);
        }
    }
}
