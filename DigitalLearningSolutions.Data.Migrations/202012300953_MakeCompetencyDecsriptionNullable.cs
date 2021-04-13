namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202012300953)]
    public class MakeCompetencyDecsriptionNullable : Migration
    {
        public override void Up()
        {
            Alter.Column("Description").OnTable("Competencies").AsString(int.MaxValue).Nullable();
            Execute.Sql("UPDATE Competencies SET Description = NULL WHERE Description = ''");
        }
        public override void Down()
        {
            Execute.Sql("UPDATE Competencies SET Description = '' WHERE Description IS NULL");
            Alter.Column("Description").OnTable("Competencies").AsString(int.MaxValue).NotNullable();
        }
    }
}
