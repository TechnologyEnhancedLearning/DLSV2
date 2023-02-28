namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202109141530)]
    public class SupervisorDelegateAddInviteHashColumn : Migration
    {
        public override void Up()
        {
            Alter.Table("SupervisorDelegates").AddColumn("InviteHash").AsGuid().NotNullable().WithDefault(SystemMethods.NewGuid);
        }
        public override void Down()
        {
            Delete.Column("InviteHash").FromTable("SupervisorDelegates");
        }
    }
}
