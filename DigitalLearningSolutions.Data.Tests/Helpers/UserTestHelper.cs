namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using DigitalLearningSolutions.Data.Models.User;

    public static class UserTestHelper
    {
        public static DelegateUser GetDefaultDelegateUser
        (
            int id = 1,
            string firstName = "Forename",
            string surname = "Surname",
            string emailAddress = "recipient@example.com",
            int? resetPasswordId = null
        )
        {
            return new DelegateUser
            {
                Id = id,
                FirstName = firstName,
                Surname = surname,
                EmailAddress = emailAddress,
                ResetPasswordId = resetPasswordId
            };
        }

        public static AdminUser GetDefaultAdminUser
        (
            int id = 1,
            string firstName = "Forename",
            string surname = "Surname",
            string emailAddress = "recipient@example.com",
            int? resetPasswordId = null
        )
        {
            return new AdminUser
            {
                Id = id,
                FirstName = firstName,
                Surname = surname,
                EmailAddress = emailAddress,
                ResetPasswordId = resetPasswordId
            };
        }
    }
}
