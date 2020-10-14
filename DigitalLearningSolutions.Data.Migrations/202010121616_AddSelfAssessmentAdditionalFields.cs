namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202010121616)]
    public class AddSelfAssessmentAdditionalFields : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments")
                .AddColumn("BrandID").AsInt32().NotNullable().ForeignKey("Brands", "BrandID").WithDefaultValue(6)
                .AddColumn("CategoryID").AsInt32().NotNullable().ForeignKey("CourseCategories", "CourseCategoryID").WithDefaultValue(1)
                .AddColumn("TopicID").AsInt32().NotNullable().ForeignKey("CourseTopics", "CourseTopicID").WithDefaultValue(1)
                .AddColumn("CreatedDate").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .AddColumn("CreatedByCentreID").AsInt32().NotNullable().ForeignKey("Centres", "CentreID").WithDefaultValue(101)
                .AddColumn("CreatedByAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID").WithDefaultValue(1)
                .AddColumn("ArchivedDate").AsDateTime().Nullable()
                .AddColumn("ArchivedByAdminID").AsInt32().Nullable();
        }
        public override void Down()
        {
            Delete.Column("CreatedByCentreID").FromTable("SelfAssessments");
            Delete.Column("CreatedByAdminID").FromTable("SelfAssessments");
            Delete.Column("CreatedDate").FromTable("SelfAssessments");
            Delete.Column("ArchivedByAdminID").FromTable("SelfAssessments");
            Delete.Column("ArchivedDate").FromTable("SelfAssessments");
            Delete.Column("BrandID").FromTable("SelfAssessments");
            Delete.Column("CategoryID").FromTable("SelfAssessments");
            Delete.Column("TopicID").FromTable("SelfAssessments");
        }
    }
}
