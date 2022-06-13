namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Web.Models;

    public static class RegistrationDataHelper
    {
        private const string FirstName = "Test";
        private const string LastName = "User";
        private const string PrimaryEmail = "test@email.com";
        private const string CentreSpecificEmail = "centre@email.com";
        private const int CentreId = 5;
        private const int JobGroupId = 10;
        private const string PasswordHash = "password hash";
        private const string Answer1 = "a1";
        private const string Answer2 = "a2";
        private const string Answer3 = "a3";
        private const bool IsCentreSpecificRegistration = true;

        public static DelegateRegistrationData SampleDelegateRegistrationData()
        {
            return new DelegateRegistrationData
            {
                FirstName = FirstName,
                LastName = LastName,
                PrimaryEmail = PrimaryEmail,
                CentreSpecificEmail = CentreSpecificEmail,
                Centre = CentreId,
                JobGroup = JobGroupId,
                PasswordHash = PasswordHash,
                Answer1 = Answer1,
                Answer2 = Answer2,
                Answer3 = Answer3,
                IsCentreSpecificRegistration = IsCentreSpecificRegistration,
            };
        }

        public static RegistrationData SampleRegistrationData()
        {
            return new RegistrationData
            {
                FirstName = FirstName,
                LastName = LastName,
                PrimaryEmail = PrimaryEmail,
                CentreSpecificEmail = CentreSpecificEmail,
                Centre = CentreId,
                JobGroup = JobGroupId,
                PasswordHash = PasswordHash,
            };
        }

        public static DelegateRegistrationData GetDefaultDelegateRegistrationData(
            string? firstName = "Test",
            string? lastName = "Name",
            string? primaryEmail = "test@email.com",
            string? centreSpecificEmail = "centre@email.com",
            int? centre = 2,
            int? jobGroup = 1,
            string? passwordHash = "hash",
            bool isCentreSpecificRegistration = false,
            int? supervisorDelegateId = 1,
            string? answer1 = "answer1",
            string? answer2 = "answer2",
            string? answer3 = "answer3",
            string? answer4 = "answer4",
            string? answer5 = "answer5",
            string? answer6 = "answer6"
        )
        {
            return new DelegateRegistrationData
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                PrimaryEmail = primaryEmail,
                CentreSpecificEmail = centreSpecificEmail,
                Centre = centre,
                JobGroup = jobGroup,
                PasswordHash = passwordHash,
                IsCentreSpecificRegistration = isCentreSpecificRegistration,
                SupervisorDelegateId = supervisorDelegateId,
                Answer1 = answer1,
                Answer2 = answer2,
                Answer3 = answer3,
                Answer4 = answer4,
                Answer5 = answer5,
                Answer6 = answer6,
            };
        }

        public static InternalDelegateRegistrationData GetDefaultInternalDelegateRegistrationData(
            string? email = "test@email.com",
            int? centre = 2,
            bool isCentreSpecificRegistration = false,
            int? supervisorDelegateId = 1,
            string? answer1 = "answer1",
            string? answer2 = "answer2",
            string? answer3 = "answer3",
            string? answer4 = "answer4",
            string? answer5 = "answer5",
            string? answer6 = "answer6"
        )
        {
            return new InternalDelegateRegistrationData
            {
                Id = Guid.NewGuid(),
                Email = email,
                Centre = centre,
                IsCentreSpecificRegistration = isCentreSpecificRegistration,
                SupervisorDelegateId = supervisorDelegateId,
                Answer1 = answer1,
                Answer2 = answer2,
                Answer3 = answer3,
                Answer4 = answer4,
                Answer5 = answer5,
                Answer6 = answer6
            };
        }

        public static DelegateRegistrationByCentreData GetDefaultDelegateRegistrationByCentreData(
            string? firstName = "Test",
            string? lastName = "Name",
            string? primaryEmail = "test@email.com",
            int? centre = 2,
            int? jobGroup = 1,
            string? passwordHash = "hash",
            bool isCentreSpecificRegistration = false,
            int? supervisorDelegateId = 1,
            string? answer1 = "answer1",
            string? answer2 = "answer2",
            string? answer3 = "answer3",
            string? answer4 = "answer4",
            string? answer5 = "answer5",
            string? answer6 = "answer6",
            string? aliasId = "alias",
            DateTime? welcomeEmailDate = null
        )
        {
            return new DelegateRegistrationByCentreData
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                PrimaryEmail = primaryEmail,
                Centre = centre,
                JobGroup = jobGroup,
                PasswordHash = passwordHash,
                IsCentreSpecificRegistration = isCentreSpecificRegistration,
                SupervisorDelegateId = supervisorDelegateId,
                Answer1 = answer1,
                Answer2 = answer2,
                Answer3 = answer3,
                Answer4 = answer4,
                Answer5 = answer5,
                Answer6 = answer6,
                Alias = aliasId,
                WelcomeEmailDate = welcomeEmailDate,
            };
        }
    }
}
