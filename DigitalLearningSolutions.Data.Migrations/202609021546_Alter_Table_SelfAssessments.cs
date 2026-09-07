namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202609021546)]
    public class Alter_Table_SelfAssessments : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments")
                .AlterColumn("ManageOptionalCompetenciesPrompt")
                .AsString(2000)
                .Nullable();
        }

        public override void Down()
        {
            Alter.Table("SelfAssessments")
                .AlterColumn("ManageOptionalCompetenciesPrompt")
                .AsString(1000)
                .Nullable();
        }
    }
}
