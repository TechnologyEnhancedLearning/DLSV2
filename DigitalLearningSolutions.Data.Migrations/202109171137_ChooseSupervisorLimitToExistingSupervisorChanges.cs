namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202109171137)]
    public class ChooseSupervisorLimitToExistingSupervisorChanges : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AddColumn("CategoryID").AsInt32().NotNullable().ForeignKey("CourseCategories", "CourseCategoryID").WithDefaultValue(1);
            Execute.Sql(@"DELETE FROM   CandidateAssessmentSupervisors
                            FROM   CandidateAssessmentSupervisors INNER JOIN
                                     SupervisorDelegates ON CandidateAssessmentSupervisors.SupervisorDelegateId = SupervisorDelegates.ID
                            WHERE(SupervisorDelegates.SupervisorAdminID IS NULL)");
            Execute.Sql("DELETE FROM SupervisorDelegates WHERE SupervisorAdminID IS NULL");
            Alter.Table("SupervisorDelegates").AlterColumn("SupervisorAdminID").AsInt32().NotNullable();
        }
        public override void Down()
        {
            Delete.ForeignKey("FK_SelfAssessments_CategoryID_CourseCategories_CourseCategoryID");
            Delete.Column("CategoryID").FromTable("SelfAssessments");
            Alter.Table("SupervisorDelegates").AlterColumn("SupervisorAdminID").AsInt32().Nullable();
        }

    }
}
