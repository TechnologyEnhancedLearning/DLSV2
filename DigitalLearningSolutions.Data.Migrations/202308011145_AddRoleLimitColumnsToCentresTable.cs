namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    
    [Migration(202308011145)]
    public class AddRoleLimitColumnsToCentresTable : Migration
    {
        public override void Up()
        {
            Alter.Table("Centres").AddColumn("RoleLimitCMSAdministrators").AsInt32().Nullable().WithDefaultValue(null);
            Alter.Table("Centres").AddColumn("RoleLimitCMSManagers").AsInt32().Nullable().WithDefaultValue(null);
            Alter.Table("Centres").AddColumn("RoleLimitCCLicenses").AsInt32().Nullable().WithDefaultValue(null);
            Alter.Table("Centres").AddColumn("RoleLimitCustomCourses").AsInt32().Nullable().WithDefaultValue(null);
            Alter.Table("Centres").AddColumn("RoleLimitTrainers").AsInt32().Nullable().WithDefaultValue(null);
        }

        public override void Down()
        {
            Delete.Column("RoleLimitCMSAdministrators").FromTable("Centres");
            Delete.Column("RoleLimitCMSManagers").FromTable("Centres");
            Delete.Column("RoleLimitCCLicenses").FromTable("Centres");
            Delete.Column("RoleLimitCustomCourses").FromTable("Centres");
            Delete.Column("RoleLimitTrainers").FromTable("Centres");
        }
    }
}
