namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202206061100)]
    public class AddCompetencyFlagTables : Migration
    {
        public override void Up()
        {
            Create.Table("Flags")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("FrameworkID").AsInt32().NotNullable().ForeignKey("Frameworks", "ID")
                .WithColumn("FlagName").AsString(200).NotNullable()
                .WithColumn("FlagGroup").AsString(100).Nullable()
                .WithColumn("FlagTagClass").AsString(100).NotNullable();

            Create.Table("CompetencyFlags")
                .WithColumn("CompetencyID").AsInt32().NotNullable().PrimaryKey().ForeignKey("Competencies", "ID")
                .WithColumn("FlagID").AsInt32().NotNullable().PrimaryKey().ForeignKey("Flags", "ID")
                .WithColumn("Selected").AsBoolean().NotNullable().WithDefaultValue(false);
        }

        public override void Down()
        {
            Delete.Table("Flags");
            Delete.Table("CompetencyFlags");
        }
    }
}
