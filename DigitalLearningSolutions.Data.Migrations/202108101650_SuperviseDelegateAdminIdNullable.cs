namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202108101650)]
    public class SuperviseDelegateAdminIdNullable : Migration
    {
        public override void Up()
        {
            Delete.ForeignKey("FK_SupervisorDelegates_SupervisorAdminID_AdminUsers_AdminID").OnTable("SupervisorDelegates");
            Alter.Table("SupervisorDelegates").AlterColumn("SupervisorAdminID").AsInt32().Nullable().ForeignKey("AdminUsers", "AdminID");
        }
        public override void Down()
        {
            Delete.ForeignKey("FK_SupervisorDelegates_SupervisorAdminID_AdminUsers_AdminID").OnTable("SupervisorDelegates");
            Alter.Table("SupervisorDelegates").AlterColumn("SupervisorAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID");
        }
    }
}
