namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202111231505)]
    public class AddFieldsToSelfassessment : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments")
                .AddColumn("QuestionLabel").AsString(50).Nullable()
                .AddColumn("DescriptionLabel").AsString(50).Nullable();

        }

        public override void Down()
        {
            Delete.Column("QuestionLabel").FromTable("SelfAssessments");
            Delete.Column("DescriptionLabel").FromTable("SelfAssessments");
        }
    }
}
