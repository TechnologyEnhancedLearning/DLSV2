namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202101041622)]
    public class ReorderFrameworkCompetenciesAndGroupsSPs : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.CreateOrAlterReorderFrameworkCompetenciesAndGroupsSPs);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.DropReorderFrameworkCompetenciesAndGroupsSPs);
        }
    }
}
