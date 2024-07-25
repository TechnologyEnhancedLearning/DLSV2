using FluentMigrator;
namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202407121718)]
    public class UpdateProgressForEnrollmentMethod : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_4223_Alter_GroupCustomisation_Add_V2_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_4223_Alter_GroupCustomisation_Add_V2_Down);
        }
    }
}
