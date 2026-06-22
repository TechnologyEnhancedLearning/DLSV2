namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;
    [Migration(202606081112)]
    public class AddPublishStatus : Migration
    {
        public override void Up()
        {

            Insert.IntoTable("PublishStatus").Row(new { Status = "Scheduled for retirement" });
            Insert.IntoTable("PublishStatus").Row(new { Status = "Retired" });
        }

        public override void Down()
        {
            Execute.Sql(@"DELETE FROM PublishStatus WHERE Status = 'Scheduled for retirement' OR Status = 'Retired'");
        }

    }
}
