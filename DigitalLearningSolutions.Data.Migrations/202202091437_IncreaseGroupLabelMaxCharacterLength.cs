namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202202091437)]
    public class IncreaseGroupLabelMaxCharacterLength : Migration
    {
        public override void Up()
        {
            Alter.Column("GroupLabel")
                .OnTable("Groups")
                .AsString(255)
                .NotNullable();
        }

        public override void Down()
        {
            Alter.Column("GroupLabel")
                .OnTable("Groups")
                .AsString(100)
                .NotNullable();
        }
    }
}
