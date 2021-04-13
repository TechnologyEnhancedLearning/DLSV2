namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using DigitalLearningSolutions.Data.Models.User;

    public static class UserTestHelper
    {
        public static DelegateUser GetDefaultDelegateUser
        (
            int id = 1,
            int centreId = 1,
            string centreName = "Centre Name",
            string firstName = "Forename",
            string surname = "Surname",
            string emailAddress = "recipient@example.com",
            string password = "password",
            int? resetPasswordId = null,
            bool approved = true,
            string candidateNumber = "CN123"
        )
        {
            return new DelegateUser
            {
                Id = id,
                CentreId = centreId,
                CentreName = centreName,
                FirstName = firstName,
                Surname = surname,
                EmailAddress = emailAddress,
                Password = password,
                ResetPasswordId = resetPasswordId,
                Approved = approved,
                CandidateNumber = candidateNumber
            };
        }

        public static AdminUser GetDefaultAdminUser
        (
            int id = 1,
            int centreId = 1,
            string centreName = "Centre Name",
            string firstName = "Forename",
            string surname = "Surname",
            string emailAddress = "recipient@example.com",
            string password = "password",
            int? resetPasswordId = null,
            bool isCentreAdmin = false,
            bool isCentreManager = false,
            bool isContentCreator = false,
            bool isContentManager = false,
            bool publishToAll = false,
            bool summaryReports = false,
            bool isUserAdmin = false,
            int categoryId = 1,
            bool isSupervisor = false,
            bool isTrainer = false,
            bool isFrameworkDeveloper = false
        )
        {
            return new AdminUser
            {
                Id = id,
                CentreId = centreId,
                CentreName = centreName,
                FirstName = firstName,
                Surname = surname,
                EmailAddress = emailAddress,
                Password = password,
                ResetPasswordId = resetPasswordId,
                IsCentreAdmin = isCentreAdmin,
                IsCentreManager = isCentreManager,
                IsContentCreator = isContentCreator,
                IsContentManager = isContentManager,
                PublishToAll = publishToAll,
                SummaryReports = summaryReports,
                IsUserAdmin = isUserAdmin,
                CategoryId = categoryId,
                IsSupervisor = isSupervisor,
                IsTrainer = isTrainer,
                IsFrameworkDeveloper = isFrameworkDeveloper
            };
        }
    }
}
