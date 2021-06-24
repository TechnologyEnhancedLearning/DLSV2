namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202106211416)]
    public class AddSupervisorTables : Migration
    {
        public override void Up()
        {
            Create.Table("SupervisorDelegates").WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                 .WithColumn("SupervisorAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID")
                 .WithColumn("UserEmail").AsString(255).NotNullable()
                 .WithColumn("CandidateID").AsInt32().Nullable().ForeignKey("Candidates", "CandidateID")
                 .WithColumn("Added").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                 .WithColumn("NotificationSent").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                 .WithColumn("Confirmed").AsDateTime().Nullable()
                 .WithColumn("Removed").AsDateTime().Nullable();
        }
        public override void Down()
        {
            Delete.Table("SupervisorDelegates");
        }
    }
}
