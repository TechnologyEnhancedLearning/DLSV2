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
            Map(adminUser => adminUser.Surname).ToColumn("Surname");
            Map(adminUser => adminUser.EmailAddress).ToColumn("Email");
            Map(adminUser => adminUser.Password).ToColumn("Password");
            Map(adminUser => adminUser.ResetPasswordId).ToColumn("ResetPasswordID");

            // Columns specific to AdminUser class
            Map(adminUser => adminUser.CentreAdmin).ToColumn("CentreAdmin");
            Map(adminUser => adminUser.IsCentreManager).ToColumn("IsCentreManager");
            Map(adminUser => adminUser.ContentCreator).ToColumn("ContentCreator");
            Map(adminUser => adminUser.ContentManager).ToColumn("ContentManager");
            Map(adminUser => adminUser.PublishToAll).ToColumn("PublishToAll");
            Map(adminUser => adminUser.SummaryReports).ToColumn("SummaryReports");
            Map(adminUser => adminUser.UserAdmin).ToColumn("UserAdmin");
            Map(adminUser => adminUser.CategoryId).ToColumn("CategoryID");
            Map(adminUser => adminUser.Supervisor).ToColumn("Supervisor");
            Map(adminUser => adminUser.Trainer).ToColumn("Trainer");
            Map(adminUser => adminUser.IsFrameworkDeveloper).ToColumn("IsFrameworkDeveloper");
        }
    }
}
