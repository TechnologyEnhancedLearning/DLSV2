namespace DigitalLearningSolutions.Data.Migrations
{
    using DigitalLearningSolutions.Data.Migrations.Properties;
    using FluentMigrator;

    // Can't CREATE DATABASE in a transaction, so disable for this migration
    [Migration(202204071100, TransactionBehavior.None)]
    public class SnapshotData : Migration
    {
        public override void Up()
        {
            Execute.Sql(Resources.UAR_858_SnapshotData_UP);
        }

        public override void Down()
        {
            // Intentionally empty. If things go wrong later, we will manually restore the snapshot
        }
    }
}
