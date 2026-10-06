using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202610061035)]
    public class Delete_Duplicate_NRPRoles : Migration
    {
        public override void Up()
        {
            Execute.Sql("DELETE FROM NRPRoles WHERE ID IN (339,362)");
            Execute.Sql("UPDATE NRPRoles SET RoleProfile = 'Physician Associate Entry Level' WHERE ID = 283");
            Execute.Sql("UPDATE NRPRoles SET RoleProfile = 'Practice Education Facilitator' WHERE ID = 340");

        }
        public override void Down()
        {
            Execute.Sql("UPDATE NRPRoles SET RoleProfile = 'Physican Assocate Entry Level' WHERE ID = 283");
            Execute.Sql("UPDATE NRPRoles SET RoleProfile = 'Practic Education Facilitator' WHERE ID = 340");
        }
    }
}
