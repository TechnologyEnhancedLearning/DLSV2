namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;
    [Migration(202501280929)]
    public class CreateCompetencyAssessmentTables : Migration
    {
        public override void Up()
        {
            Create.Table("SelfAssessmentFrameworks")
               .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
               .WithColumn("SelfAssessmentId").AsInt32().NotNullable().ForeignKey("SelfAssessments", "ID")
               .WithColumn("FrameworkId").AsInt32().NotNullable().ForeignKey("Frameworks", "ID")
               .WithColumn("CreatedDate").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
               .WithColumn("CreatedByUserId").AsInt32().NotNullable().ForeignKey("Users", "ID")
               .WithColumn("RemovedDate").AsDateTime().Nullable()
               .WithColumn("RemovedByUserId").AsInt32().Nullable().ForeignKey("Users", "ID")
               .WithColumn("AmendedDate").AsDateTime().Nullable()
               .WithColumn("AmendedByUserId").AsInt32().Nullable().ForeignKey("Users", "ID");
            Create.Table("SelfAssessmentTaskStatus")
               .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
               .WithColumn("SelfAssessmentId").AsInt32().NotNullable().ForeignKey("SelfAssessments", "ID").Unique()
               .WithColumn("IntroductoryTextTaskStatus").AsBoolean().Nullable()
               .WithColumn("BrandingTaskStatus").AsBoolean().Nullable()
               .WithColumn("VocabularyTaskStatus").AsBoolean().Nullable()
               .WithColumn("WorkingGroupTaskStatus").AsBoolean().Nullable()
               .WithColumn("NationalRoleProfileTaskStatus").AsBoolean().Nullable()
               .WithColumn("FrameworkLinksTaskStatus").AsBoolean().Nullable()
               .WithColumn("SelectCompetenciesTaskStatus").AsBoolean().Nullable()
               .WithColumn("OptionalCompetenciesTaskStatus").AsBoolean().Nullable()
               .WithColumn("RoleRequirementsTaskStatus").AsBoolean().Nullable()
               .WithColumn("SupervisorRolesTaskStatus").AsBoolean().Nullable()
               .WithColumn("SelfAssessmentOptionsTaskStatus").AsBoolean().Nullable()
               .WithColumn("ReviewTaskStatus").AsBoolean().Nullable();
            Execute.Sql($@"INSERT INTO SelfAssessmentFrameworks (SelfAssessmentId, FrameworkId, CreatedByUserId)
                            SELECT sa.ID, fc.FrameworkID, aa.UserID
                            FROM   SelfAssessments AS sa INNER JOIN
                                         SelfAssessmentStructure AS sas ON sa.ID = sas.SelfAssessmentID INNER JOIN
                                         FrameworkCompetencies AS fc ON sas.CompetencyID = fc.CompetencyID INNER JOIN
                                         AdminAccounts AS aa ON sa.CreatedByAdminID = aa.ID
                            GROUP BY sa.ID, fc.FrameworkID, aa.UserID
                            ");
            Execute.Sql($@"INSERT INTO SelfAssessmentTaskStatus (SelfAssessmentId, IntroductoryTextTaskStatus, BrandingTaskStatus, VocabularyTaskStatus, WorkingGroupTaskStatus, NationalRoleProfileTaskStatus, FrameworkLinksTaskStatus, SelectCompetenciesTaskStatus, OptionalCompetenciesTaskStatus, RoleRequirementsTaskStatus, SupervisorRolesTaskStatus, SelfAssessmentOptionsTaskStatus)
                            SELECT ID, 1,1,1,1,1,1,1,1,1,1,1
                            FROM   SelfAssessments AS sa
                            ");
        }

        public override void Down()
        {
            Delete.Table("SelfAssessmentFrameworks");
            Delete.Table("SelfAssessmentTaskStatus");
        }

    }
}
