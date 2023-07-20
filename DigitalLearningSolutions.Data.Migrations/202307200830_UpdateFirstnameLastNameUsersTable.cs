namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202307200830)]
    public class UpdateFirstnameLastNameUsersTable : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"Update Users set FirstName = LTRIM(RTRIM(FirstName)), LastName = LTRIM(RTRIM(LastName))");
        }
        public override void Down()
        {
        }
    }
}
