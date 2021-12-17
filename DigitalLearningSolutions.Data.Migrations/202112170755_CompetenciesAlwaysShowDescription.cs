namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202112170755)]
    public class CompetenciesAlwaysShowDescription : Migration
    {
        public override void Down()
        {
            Alter.Table("Competencies").AddColumn("AlwaysShowDescription").AsBoolean().WithDefaultValue(false);
        }

        public override void Up()
        {
            Delete.Column("AlwaysShowDescription").FromTable("Competencies");
        }
    }
}
