namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202111241539)]

    public class AddPrnToCandidateTable: Migration {

        public override void Up()
        {
            Alter.Table("Candidates")
                .AddColumn("HasBeenPromptedForPrn").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("ProfessionalRegistrationNumber").AsString(32).Nullable();
        }

        public override void Down()
        {
            Alter.Table("Candidates")
                .AddColumn("HasBeenPromptedForPrn").AsBoolean().NotNullable().WithDefault(0)
                .AddColumn("ProfessionalRegistrationNumber").AsString(32).Nullable();
        }
    }
}
