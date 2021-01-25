namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202101191026)]
    public class AddAssessmentQuestionAdminIDColumn : Migration
    {
        public override void Up()
        {
            Alter.Table("AssessmentQuestions").AddColumn("AddedByAdminId").AsInt32().NotNullable().WithDefaultValue(1).ForeignKey("AdminUsers", "AdminID");
        }

        public override void Down()
        {
            Delete.Column("AddedByAdminId").FromTable("AssessmentQuestions");
        }
    }
}
