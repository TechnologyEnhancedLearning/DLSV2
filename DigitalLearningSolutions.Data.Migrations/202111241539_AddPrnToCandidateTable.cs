namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202111241539)]
    public class AddPrnToCandidateTable : Migration
    {
        public override void Up()
        {
            Alter.Table("Candidates")
                .AddColumn("HasBeenPromptedForPrn").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("ProfessionalRegistrationNumber").AsString(32).Nullable();
        }

        public override void Down()
        {
            Delete.Column("HasBeenPromptedForPrn").FromTable("Candidates");
            Delete.Column("ProfessionalRegistrationNumber").FromTable("Candidates");
        }
    }
}
