namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202009150805)]
    public class AddSelfAssessmentUseApiFlag : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments")
                .AddColumn("UseFilteredApi").AsBoolean().NotNullable().WithDefaultValue(false);
        }

        public override void Down()
        {
            Delete.Column("UseFilteredApi").FromTable("SelfAssessments");
        }
    }
}
