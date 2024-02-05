namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202401311625)]
    public class DeleteDeprecatedSPs : Migration
    {
        public override void Up()
        {
            string[] currentProcedureNames = RenameDeprecatedStoredProcs.GetProcedureNames();
            foreach (string currentProcedureName in currentProcedureNames)
            {
                string newProcedureName = currentProcedureName + "_deprecated";
                string dropQuery = $"DROP PROCEDURE IF EXISTS dbo.{newProcedureName};";

                Execute.Sql(dropQuery);
            }
        }

        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_3664_RestoreDroppedSPs);
        }
    }
}
