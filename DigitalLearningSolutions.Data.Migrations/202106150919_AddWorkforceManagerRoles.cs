namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202106150919)]
    public class AddWorkforceManagerRoles : Migration
    {
        public override void Up()
        {
            Alter.Table("AdminUsers").AddColumn("IsWorkforceManager").AsBoolean().WithDefaultValue(false);
            Alter.Table("AdminUsers").AddColumn("IsWorkforceContributor").AsBoolean().WithDefaultValue(false);
            Alter.Table("AdminUsers").AddColumn("IsLocalWorkforceManager").AsBoolean().WithDefaultValue(false);
        }
        public override void Down()
        {
            Delete.Column("IsWorkforceManager").FromTable("AdminUsers");
            Delete.Column("IsWorkforceContributor").FromTable("AdminUsers");
            Delete.Column("IsLocalWorkforceManager").FromTable("AdminUsers");
        }
    }
}
