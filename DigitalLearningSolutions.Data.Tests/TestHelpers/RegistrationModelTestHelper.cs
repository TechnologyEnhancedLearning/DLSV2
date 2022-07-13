namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.Register;

    public static class RegistrationModelTestHelper
    {
        public const int Centre = 2;
        public const string PasswordHash = "hash";

        public static AdminAccountRegistrationModel GetDefaultCentreManagerAccountRegistrationModel(
            int userId = 4046,
            string centreSpecificEmail = "centre@email.com",
            int centre = Centre,
            bool active = true,
            int? categoryId = null,
            bool isCentreAdmin = true,
            bool isCentreManager = true,
            bool isContentCreator = false,
            bool isContentManager = false,
            bool isTrainer = false,
            bool importOnly = false,
            bool isSupervisor = false,
            bool isNominatedSupervisor = false
        )
        {
            return new AdminAccountRegistrationModel(
                userId,
                centreSpecificEmail,
                centre,
                categoryId,
                isCentreAdmin,
                isCentreManager,
                isContentManager,
                isContentCreator,
                isTrainer,
                importOnly,
                isSupervisor,
                isNominatedSupervisor,
                active
            );
        }

        public static AdminRegistrationModel GetDefaultCentreManagerRegistrationModel(
            string firstName = "Test",
            string lastName = "User",
            string email = "testuser@email.com",
            int centre = Centre,
            string? centreSpecificEmail = null,
            string? passwordHash = PasswordHash,
            bool active = true,
            bool approved = true,
            string? professionalRegistrationNumber = "PRN1234",
            int jobGroupId = 0,
            int categoryId = 0,
            bool isCentreAdmin = true,
            bool isCentreManager = true,
            bool isSupervisor = false,
            bool isNominatedSupervisor = false,
            bool isTrainer = false,
            bool isContentCreator = false,
            bool isCmsAdmin = false,
            bool isCmsManager = false
        )
        {
            return new AdminRegistrationModel(
                firstName,
                lastName,
                email,
                centreSpecificEmail,
                centre,
                passwordHash,
                active,
                approved,
                professionalRegistrationNumber,
                jobGroupId,
                categoryId,
                isCentreAdmin,
                isCentreManager,
                isSupervisor,
                isNominatedSupervisor,
                isTrainer,
                isContentCreator,
                isCmsAdmin,
                isCmsManager
            );
        }

        public static AdminRegistrationModel GetDefaultAdminRegistrationModel(
            string firstName = "Test",
            string lastName = "User",
            string email = "testuser@email.com",
            int centre = Centre,
            string? centreSpecificEmail = null,
            string? passwordHash = PasswordHash,
            bool active = true,
            bool approved = true,
            string? professionalRegistrationNumber = "PRN1234",
            int jobGroupId = 0,
            int categoryId = 0,
            bool isCentreAdmin = true,
            bool isCentreManager = true,
            bool isSupervisor = true,
            bool isNominatedSupervisor = true,
            bool isTrainer = true,
            bool isContentCreator = true,
            bool isCmsAdmin = true,
            bool isCmsManager = false
        )
        {
            return new AdminRegistrationModel(
                firstName,
                lastName,
                email,
                centreSpecificEmail,
                centre,
                passwordHash,
                active,
                approved,
                professionalRegistrationNumber,
                jobGroupId,
                categoryId,
                isCentreAdmin,
                isCentreManager,
                isSupervisor,
                isNominatedSupervisor,
                isTrainer,
                isContentCreator,
                isCmsAdmin,
                isCmsManager
            );
        }

        public static DelegateRegistrationModel GetDefaultDelegateRegistrationModel(
            string firstName = "Test",
            string lastName = "User",
            string primaryEmail = "testuser@email.com",
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
            DateTime? notifyDate = null,
            bool active = true,
            bool activeUser = true,
            bool approved = false,
            string? professionalRegistrationNumber = "PRN1234",
            string? centreSpecificEmail = "testuser@weekends.com"
        )
        {
            return new DelegateRegistrationModel(
                firstName,
                lastName,
                primaryEmail,
                centreSpecificEmail,
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
                active,
                activeUser,
                professionalRegistrationNumber,
                approved,
                notifyDate
            );
        }

        public static InternalDelegateRegistrationModel GetDefaultInternalDelegateRegistrationModel(
            int centre = Centre,
            string? answer1 = "answer1",
            string? answer2 = "answer2",
            string? answer3 = "answer3",
            string? answer4 = "answer4",
            string? answer5 = "answer5",
            string? answer6 = "answer6",
            string? centreSpecificEmail = "testuser@weekends.com"
        )
        {
            return new InternalDelegateRegistrationModel(
                centre,
                centreSpecificEmail,
                answer1,
                answer2,
                answer3,
                answer4,
                answer5,
                answer6
            );
        }
    }
}
