namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.Register;

    public static class RegistrationModelTestHelper
    {
        public const int Centre = 2;
        public const string PasswordHash = "hash";

        public static AdminRegistrationModel GetDefaultCentreManagerRegistrationModel(
            string firstName = "Test",
            string lastName = "User",
            string email = "testuser@email.com",
            int centre= Centre,
            string? passwordHash= PasswordHash,
            bool active=true,
            bool approved=true,
            bool isCentreAdmin=true,
            bool isCentreManager=true,
            bool isSupervisor= false,
            bool isTrainer= false,
            bool isContentCreator= false,
            bool isCmsAdmin= false,
            bool isCmsManager= false
            )
        {
            return new AdminRegistrationModel(
                firstName,
                lastName,
                email,
                centre,
                passwordHash,
                active,
                approved,
                isCentreAdmin,
                isCentreManager,
                isSupervisor,
                isTrainer,
                isContentCreator,
                isCmsAdmin,
                isCmsManager
            );
        }

        public static DelegateRegistrationModel GetDefaultDelegateRegistrationModel(
            string firstName = "Test",
            string lastName = "User",
            string email = "testuser@email.com",
            int centre = Centre,
            int jobGroup = 1,
            string? passwordHash = PasswordHash,
            string? answer1 = "answer1",
            string? answer2 = "answer2",
            string? answer3 = "answer3",
            string? answer4 = "answer4",
            string? answer5 = "answer5",
            string? answer6 = "answer6",
            bool isSelfRegistered = true,
            string? aliasId = null,
            DateTime? notifyDate = null,
            bool active = true,
            bool approved = false)
        {
            return new DelegateRegistrationModel(
                firstName,
                lastName,
                email,
                centre,
                jobGroup,
                passwordHash,
                answer1,
                answer2,
                answer3,
                answer4,
                answer5,
                answer6,
                isSelfRegistered,
                aliasId,
                notifyDate,
                active,
                approved
            );
        }
    }
}
