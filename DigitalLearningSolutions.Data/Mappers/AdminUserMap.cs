namespace DigitalLearningSolutions.Data.Mappers
{
    using Dapper.FluentMap.Mapping;
    using DigitalLearningSolutions.Data.Models.User;

    public class AdminUserMap : EntityMap<AdminUser>
    {
        public AdminUserMap()
        {
            Map(adminUser => adminUser.Id).ToColumn("AdminID");
            Map(adminUser => adminUser.FirstName).ToColumn("Forename");
            Map(adminUser => adminUser.Surname).ToColumn("Surname");
            Map(adminUser => adminUser.EmailAddress).ToColumn("Email");
            Map(adminUser => adminUser.ResetPasswordId).ToColumn("ResetPasswordID");
        }
    }
}
