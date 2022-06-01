namespace DigitalLearningSolutions.Web.Tests.Models
{
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FluentAssertions;
    using NUnit.Framework;

    internal class DelegateRegistrationDataTests
    {
        private const string FirstName = "Test";
        private const string LastName = "User";
        private const string Email = "test@email.com";
        private const int CentreId = 5;
        private const int JobGroupId = 10;
        private const string Answer1 = "a1";
        private const string Answer2 = "a2";
        private const string Answer3 = "a3";
        private const string Answer4 = "a4";
        private const string Answer5 = "a5";
        private const string Answer6 = "a6";

        [Test]
        public void SetPersonalInformation_sets_data_correctly()
        {
            // Given
            var model = new PersonalInformationViewModel
            {
                FirstName = FirstName,
                LastName = LastName,
                Centre = CentreId,
                PrimaryEmail = Email
            };
            var data = new DelegateRegistrationData();

            // When
            data.SetPersonalInformation(model);

            // Then
            data.FirstName.Should().Be(FirstName);
            data.LastName.Should().Be(LastName);
            data.PrimaryEmail.Should().Be(Email);
            data.Centre.Should().Be(CentreId);
        }

        [Test]
        public void SetLearnerInformation_sets_data_correctly()
        {
            // Given
            var model = new LearnerInformationViewModel
            {
                JobGroup = JobGroupId,
                Answer1 = Answer1,
                Answer2 = Answer2,
                Answer3 = Answer3,
                Answer4 = Answer4,
                Answer5 = Answer5,
                Answer6 = Answer6
            };
            var data = new DelegateRegistrationData();

            // When
            data.SetLearnerInformation(model);

            // Then
            data.JobGroup.Should().Be(JobGroupId);
            data.Answer1.Should().Be(Answer1);
            data.Answer2.Should().Be(Answer2);
            data.Answer3.Should().Be(Answer3);
            data.Answer4.Should().Be(Answer4);
            data.Answer5.Should().Be(Answer5);
            data.Answer6.Should().Be(Answer6);
        }

        [Test]
        public void ClearCustomPromptAnswers_clears_data_correctly()
        {
            // Given
            var data = new DelegateRegistrationData
            {
                Answer1 = Answer1, Answer2 = Answer2, Answer3 = Answer3, Answer4 = Answer4, Answer5 = Answer5,
                Answer6 = Answer6
            };

            // When
            data.ClearCustomPromptAnswers();

            // Then
            data.Answer1.Should().BeNull();
            data.Answer2.Should().BeNull();
            data.Answer3.Should().BeNull();
            data.Answer4.Should().BeNull();
            data.Answer5.Should().BeNull();
            data.Answer6.Should().BeNull();
        }
    }
}
