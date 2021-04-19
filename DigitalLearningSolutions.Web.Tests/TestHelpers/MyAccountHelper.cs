namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.User;

    internal class MyAccountHelper
    {
        internal static DelegateUser GetDefaultDelegateUser
        (
            int id = 1,
            int centreId = 2,
            string centreName = "North West Boroughs Healthcare NHS Foundation Trust",
            string firstName = "Firstname",
            string lastName = "Test",
            string emailAddress = "email@test.com",
            string password = "password",
            int? resetPasswordId = null,
            bool approved = true,
            string candidateNumber = "SV1234"
        )
        {
            return new DelegateUser
            {
                Id = id,
                CentreId = centreId,
                CentreName = centreName,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                Password = password,
                ResetPasswordId = resetPasswordId,
                Approved = approved,
                CandidateNumber = candidateNumber
            };
        }

        internal static AdminUser GetDefaultAdminUser
        (
            int id = 1,
            int centreId = 2,
            string centreName = "North West Boroughs Healthcare NHS Foundation Trust",
            string firstName = "forename",
            string lastName = "surname",
            string emailAddress = "test@gmail.com",
            string password = "Password",
            int? resetPasswordId = null,
            bool isCentreAdmin = true,
            bool isCentreManager = true,
            bool isContentCreator = false,
            bool isContentManager = true,
            bool publishToAll = true,
            bool summaryReports = false,
            bool isUserAdmin = true,
            int categoryId = 1,
            bool isSupervisor = true,
            bool isTrainer = true,
            bool isFrameworkDeveloper = true
        )
        {
            return new AdminUser
            {
                Id = id,
                CentreId = centreId,
                CentreName = centreName,
                FirstName = firstName,
                LastName = lastName,
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

        internal static string GetDefaultCentreName(string centreName = "North West Boroughs Healthcare NHS Foundation Trust")
        {
            return centreName;
        }
    }
}
