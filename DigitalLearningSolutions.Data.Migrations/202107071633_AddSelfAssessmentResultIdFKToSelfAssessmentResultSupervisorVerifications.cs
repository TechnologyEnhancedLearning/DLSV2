namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202107071633)]
    public class AddSelfAssessmentResultIdFKToSelfAssessmentResultSupervisorVerifications : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessmentResultSupervisorVerifications").AddColumn("SelfAssessmentResultId").AsInt32().NotNullable().ForeignKey("SelfAssessmentResults", "ID");
        }
        public override void Down()
        {
            Delete.Column("SelfAssessmentResultId").FromTable("SelfAssessmentResultSupervisorVerifications");
        }
    }
}
