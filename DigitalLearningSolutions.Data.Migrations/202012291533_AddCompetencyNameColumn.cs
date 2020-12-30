namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202012291533)]
    public class AddCompetencyNameColumn : Migration
    {
        public override void Up()
        {
            Alter.Table("Competencies").AddColumn("Name").AsString(500).Nullable();
            Execute.Sql("UPDATE Competencies SET Name = SUBSTRING ( Description ,1 , 500 ) FROM Competencies");
        }
        public override void Down()
        {
            Delete.Column("Name").FromTable("Competencies");
        }
    }
}
