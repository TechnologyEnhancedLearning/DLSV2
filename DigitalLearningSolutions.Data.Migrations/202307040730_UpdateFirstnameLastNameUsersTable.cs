namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202307040730)]
    public class UpdateFirstnameLastNameUsersTable : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"Update Users set FirstName = TRIM(FirstName), LastName = TRIM(LastName)");
        }
        public override void Down()
        {
        }
    }
}
