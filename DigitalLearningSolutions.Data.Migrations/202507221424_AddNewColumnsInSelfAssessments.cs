namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202507221424)]
    public class AddNewColumnsInSelfAssessments : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments")
               .AddColumn("RetirementDate").AsDateTime().Nullable()
               .AddColumn("EnrolmentCutoffDate").AsDateTime().Nullable()
               .AddColumn("RetirementReason").AsString(2000).Nullable();
        }

        public override void Down()
        {
            Delete.Column("RetirementDate").FromTable("SelfAssessments");
            Delete.Column("EnrolmentCutoffDate").FromTable("SelfAssessments");
            Delete.Column("RetirementReason").FromTable("SelfAssessments");
        }
    }
}
