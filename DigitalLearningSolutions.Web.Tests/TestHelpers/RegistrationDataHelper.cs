namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Web.Models;

    public static class RegistrationDataHelper
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

        public static DelegateRegistrationData SampleDelegateRegistrationData()
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

        public static RegistrationData SampleRegistrationData()
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
    }
}
