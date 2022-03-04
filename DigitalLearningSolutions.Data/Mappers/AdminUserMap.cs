namespace DigitalLearningSolutions.Data.Mappers
{
    using Dapper.FluentMap.Mapping;
    using DigitalLearningSolutions.Data.Models.User;

    public class AdminUserMap : EntityMap<AdminUser>
    {
        public AdminUserMap()
        {
            // Columns derived from User abstract class
            Map(adminUser => adminUser.Id).ToColumn("AdminID");
            Map(adminUser => adminUser.CentreId).ToColumn("CentreID");
            Map(adminUser => adminUser.CentreName).ToColumn("CentreName");
            Map(adminUser => adminUser.FirstName).ToColumn("Forename");
            Map(adminUser => adminUser.LastName).ToColumn("Surname");
            Map(adminUser => adminUser.EmailAddress).ToColumn("Email");
            Map(adminUser => adminUser.Password).ToColumn("Password");
            Map(adminUser => adminUser.ResetPasswordId).ToColumn("ResetPasswordID");

            // Columns specific to AdminUser class
            Map(adminUser => adminUser.IsCentreAdmin).ToColumn("IsCentreAdmin");
            Map(adminUser => adminUser.IsCentreManager).ToColumn("IsCentreManager");
            Map(adminUser => adminUser.IsContentCreator).ToColumn("IsContentCreator");
            Map(adminUser => adminUser.IsContentManager).ToColumn("IsContentManager");
            Map(adminUser => adminUser.PublishToAll).ToColumn("PublishToAll");
            Map(adminUser => adminUser.SummaryReports).ToColumn("SummaryReports");
            Map(adminUser => adminUser.IsUserAdmin).ToColumn("IsUserAdmin");
            Map(adminUser => adminUser.CategoryId).ToColumn("CategoryID");
            Map(adminUser => adminUser.IsSupervisor).ToColumn("IsSupervisor");
            Map(adminUser => adminUser.IsNominatedSupervisor).ToColumn("IsNominatedSupervisor");
            Map(adminUser => adminUser.IsTrainer).ToColumn("IsTrainer");
            Map(adminUser => adminUser.IsFrameworkDeveloper).ToColumn("IsFrameworkDeveloper");
        }
    }
}
