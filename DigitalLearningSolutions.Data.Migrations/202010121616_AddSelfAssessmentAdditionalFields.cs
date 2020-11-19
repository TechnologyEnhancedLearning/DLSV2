namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202010121616)]
    public class AddSelfAssessmentAdditionalFields : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments")
                .AddColumn("BrandID").AsInt32().NotNullable().ForeignKey("FK_SelfAssessments_BrandID_Brands_BrandID", "Brands", "BrandID").WithDefaultValue(6)
                .AddColumn("CategoryID").AsInt32().NotNullable().ForeignKey("FK_SelfAssessments_CategoryID_CourseCategories_CourseCategoryID", "CourseCategories", "CourseCategoryID").WithDefaultValue(1)
                .AddColumn("TopicID").AsInt32().NotNullable().ForeignKey("FK_SelfAssessments_TopicID_CourseTopics_CourseTopicID", "CourseTopics", "CourseTopicID").WithDefaultValue(1)
                .AddColumn("CreatedDate").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .AddColumn("CreatedByCentreID").AsInt32().NotNullable().ForeignKey("FK_SelfAssessments_CreatedByCentreID_Centres_CentreID", "Centres", "CentreID").WithDefaultValue(101)
                .AddColumn("CreatedByAdminID").AsInt32().NotNullable().ForeignKey("FK_SelfAssessments_CreatedByAdminID_AdminUsers_AdminID", "AdminUsers", "AdminID").WithDefaultValue(1)
                .AddColumn("ArchivedDate").AsDateTime().Nullable()
                .AddColumn("ArchivedByAdminID").AsInt32().Nullable();
        }
        public override void Down()
        {
            Delete.ForeignKey("FK_SelfAssessments_CreatedByCentreID_Centres_CentreID").OnTable("SelfAssessments");
            Delete.ForeignKey("FK_SelfAssessments_BrandID_Brands_BrandID").OnTable("SelfAssessments");
            Delete.ForeignKey("FK_SelfAssessments_CategoryID_CourseCategories_CourseCategoryID").OnTable("SelfAssessments");
            Delete.ForeignKey("FK_SelfAssessments_TopicID_CourseTopics_CourseTopicID").OnTable("SelfAssessments");
            Delete.ForeignKey("FK_SelfAssessments_CreatedByAdminID_AdminUsers_AdminID").OnTable("SelfAssessments");
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
