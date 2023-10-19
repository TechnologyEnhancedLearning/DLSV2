namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202310191439)]
    public class AddAssessmentTypesPrimaryKey : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("AssessmentTypes").Constraint("PK_AssessmentTypes").Exists())
            {
                Create.PrimaryKey("PK_AssessmentTypes").OnTable("AssessmentTypes").Column("AssessmentTypeID");
            }
        }
        public override void Down()
        {
            Delete.PrimaryKey("PK_AssessmentTypes").FromTable("AssessmentTypes");
        }
    }
}
