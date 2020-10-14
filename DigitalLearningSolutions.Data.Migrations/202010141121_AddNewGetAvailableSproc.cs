namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202010141121)]
    public class AddNewGetAvailableSproc : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.GetActiveAvailableV5);
        }

        public override void Down()
        {
            Execute.Sql(Properties.Resources.DropGetActiveAvailableV5);
        }
    }
}
