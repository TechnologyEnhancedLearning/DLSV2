namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202510021000)]
    public class AddIsdeletedSelfAssessmentCollaborators : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessmentCollaborators").AddColumn("IsDeleted").AsBoolean().WithDefaultValue(false);
        }
        public override void Down()
        {
            Delete.Column("IsDeleted").FromTable("SelfAssessmentCollaborators");
        }
    }
}
