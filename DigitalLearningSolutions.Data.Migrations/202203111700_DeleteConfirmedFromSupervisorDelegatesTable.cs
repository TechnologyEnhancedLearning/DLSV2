namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202203111700)]
    public class DeleteConfirmedFromSupervisorDelegatesTable : Migration
    {
        public override void Up()
        {
            Delete.Column("Confirmed").FromTable("SupervisorDelegates"); 
        }
        public override void Down()
        {
            Alter.Table("SupervisorDelegates").AddColumn("Confirmed").AsDateTime().Nullable();
        }
    }
}
